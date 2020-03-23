using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class InstructionContext
	{
		private static Pool<StackEntry> StackEntryPool = new Pool<StackEntry>(100, 10);

		public Object Owner { get; private set; }
		public Object Parent { get { return _stack.Count > 0 ? _stack.Peek().Parent : null; } }
		public string Name { get; private set; }
		public VariableStore Variables { get; private set; }
		public bool IsRunning { get { return _stack.Count > 0; } }
		public int FrameCount { get { return _stack.Count; } }

		private Stack<StackEntry> _stack = new Stack<StackEntry>();
		private List<InstructionListener> _listeners;

		public InstructionContext(Object owner, string name, VariableStore variables)
		{
			Owner = owner;
			Name = name;
			Variables = variables ?? new VariableStore();
		}

		public T GetObject<T>(VariableReference variable) where T : Object
		{
			switch (variable.Location)
			{
				case VariableLocation.Owner: return ComponentFinder.GetAsObject<T>(Owner);
				case VariableLocation.Parent: return ComponentFinder.GetAsObject<T>(Parent);
				case VariableLocation.Scene: return ComponentFinder.GetAsObject<T>(GameObject.Find(variable.Name));
			}

			var store = GetStore(variable);

			if (store != null)
			{
				var v = store.GetVariable(variable.Name);

				switch (v.Type)
				{
					case VariableType.Asset: return ComponentFinder.GetAsObject<T>(v.GetAsset());
					case VariableType.GameObject: return ComponentFinder.GetAsObject<T>(v.GetGameObject());
					case VariableType.Other: return ComponentFinder.GetAsObject<T>(v.GetOther() as Object);
				}
			}

			return null;
		}

		public VariableStore GetStore(VariableReference variable)
		{
			switch (variable.Location)
			{
				case VariableLocation.Context: return Variables;
				case VariableLocation.Custom: return GetCustomStore(variable);
			}

			return null;
		}

		public IEnumerator Execute(Instruction instruction, Object parent)
		{
			if (IsRunning)
				yield break;

			Run(instruction, parent);
			yield return null;

			while (IsRunning)
			{
				Update();
				yield return null;
			}
		}

		public int OpenFrame()
		{
			PushStackFrame(StackEntryType.Open, null, null);
			return _stack.Count;
		}

		public void CloseFrame()
		{
			if (_stack.Count > 0 || _stack.Peek().Type == StackEntryType.Open)
				PopStackFrame();
			else
				Debug.LogErrorFormat("error closing frame on the context {0}: the context is not running an open frame", Name);
		}

		public void Run(Instruction instruction, Object parent)
		{
			if (PushStackFrame(StackEntryType.Single, new List<Instruction> { instruction }, parent))
			{
				if (instruction != null)
					Start(_stack.Peek(), 0);
			}
		}

		public void RunInSequence(List<Instruction> instructions, Object parent)
		{
			if (PushStackFrame(StackEntryType.Sequence, instructions, parent))
			{
				if (instructions.Count > 0)
					Start(_stack.Peek(), 0);
			}
		}

		public void RunInParallel(List<Instruction> instructions, Object parent)
		{
			if (PushStackFrame(StackEntryType.Parallel, instructions, parent))
			{
				for (var i = 0; i < instructions.Count; i++)
					Start(_stack.Peek(), i);
			}
		}

		public void Prompt(PromptInstruction instruction, object data)
		{
			var listeners = GetListeners(instruction.Category);
			
			var entry = _stack.Peek();

			if (listeners == null || listeners.Count == 0)
			{
				instruction.OnIgnored(this);
			}
			else
			{
				foreach (var listener in listeners)
					listener.Prompt(instruction, data, this);

				entry.WaitCount = listeners.Count;
				_listeners = listeners;
			}
		}
		
		public void Finish(PromptInstruction instruction)
		{
			if (_listeners != null)
			{
				foreach (var listener in _listeners)
					listener.Finish();

				_listeners = null;
			}
		}

		public void Dismiss(PromptInstruction instruction)
		{
			var entry = _stack.Peek();
			entry.WaitCount--;
			instruction.OnDismissed(this);
		}

		public void Select(PromptInstruction instruction, object selection)
		{
			var entry = _stack.Peek();
			entry.WaitCount--;
			instruction.OnSelected(this, selection);
		}

		public void ProcessInput()
		{
			if (_listeners != null)
			{
				foreach (var listener in _listeners)
					listener.ProcessInput();
			}
		}

		public void Update()
		{
			while (_stack.Count > 0 && Continue(_stack.Peek()))
				PopStackFrame();
		}

		protected abstract VariableStore GetCustomStore(VariableReference variable);
		protected abstract List<InstructionListener> GetListeners(string category);

		private enum StackEntryType
		{
			Empty,
			Single,
			Sequence,
			Parallel,
			Open
		}

		private class StackEntry : IPoolable
		{
			public StackEntryType Type;
			public List<Instruction> Instructions;
			public Object Parent;
			public BitArray Completed;
			public int Index;
			public int StackPointer;
			public int WaitCount;

			public void Reset()
			{
			}
		}

		private bool PushStackFrame(StackEntryType type, List<Instruction> instructions, Object parent)
		{
			if (_stack.Count > 0 && _stack.Peek().WaitCount > 0)
			{
				Debug.LogErrorFormat("error running instructions on the context {0}: the context is waiting for input", Name);
				return false;
			}

			var entry = StackEntryPool.Reserve();
			entry.Type = type != StackEntryType.Open && (instructions == null || instructions.Count == 0) ? StackEntryType.Empty : type;
			entry.Instructions = instructions;
			entry.Parent = parent;
			entry.Completed = type == StackEntryType.Parallel ? new BitArray(instructions.Count, false) : null;
			entry.Index = 0;
			entry.StackPointer = Variables.Variables.Count;
			entry.WaitCount = 0;
			_stack.Push(entry);

			return true;
		}

		private void PopStackFrame()
		{
			var entry = _stack.Pop();
			Variables.Chop(entry.StackPointer);
			StackEntryPool.Release(entry);
		}

		private void MarkCompleted(StackEntry data, int index)
		{
			data.Index++;

			if (data.Type == StackEntryType.Parallel)
				data.Completed.Set(index, true);
		}

		private void Start(StackEntry data, int index)
		{
			if (!Begin(data.Instructions[index]))
				MarkCompleted(data, index);
		}

		private bool Continue(StackEntry data)
		{
			switch (data.Type)
			{
				case StackEntryType.Empty: return true;
				case StackEntryType.Single: return ContinueSingle(data);
				case StackEntryType.Sequence: return ContinueSequence(data);
				case StackEntryType.Parallel: return ContinueParallel(data);
				case StackEntryType.Open: return false;
			}

			return true;
		}

		private bool ContinueSingle(StackEntry data)
		{
			if (data.WaitCount == 0 && data.Index == 0)
			{
				var instruction = data.Instructions[0];

				if (Execute(instruction))
				{
					End(instruction);
					MarkCompleted(data, 0);
					return true;
				}
			}

			return false;
		}

		private bool ContinueSequence(StackEntry data)
		{
			if (data.WaitCount == 0 && data.Index < data.Instructions.Count)
			{
				var instruction = data.Instructions[data.Index];

				if (Execute(instruction))
				{
					End(instruction);
					MarkCompleted(data, data.Index);

					if (data.Index < data.Instructions.Count)
						Start(data, data.Index);
					else
						return true;
				}
			}

			return false;
		}

		private bool ContinueParallel(StackEntry data)
		{
			if (data.WaitCount == 0)
			{
				for (var i = 0; i < data.Instructions.Count; i++)
				{
					if (!data.Completed.Get(i))
					{
						var instruction = data.Instructions[i];

						if (Execute(instruction))
						{
							End(instruction);
							MarkCompleted(data, i);

							return data.Index == data.Instructions.Count;
						}
					}
				}
			}

			return false;
		}

		private bool Begin(Instruction instruction)
		{
			try
			{
				if (instruction != null)
				{
					// TODO: does this need to be RunCount instead of IsRunning so IsRunning isn't wrong when AllowMultipleInstances is true?
					if (instruction.IsRunning && !instruction.AllowMultipleInstances)
					{
						Debug.LogFormat("error running instruction {0} on context {1}: the instruction is already running", instruction.name, Name);
						return false;
					}

					instruction.IsRunning = true;
					instruction.Begin(this);
				}

				return true;
			}
			catch (System.Exception exception)
			{
				instruction.IsRunning = false;
				Debug.LogException(exception, Owner);
				return false;
			}
		}

		private bool Execute(Instruction instruction)
		{
			try
			{
				return instruction == null || instruction.Execute(this);
			}
			catch (System.Exception exception)
			{
				Debug.LogException(exception, Owner);
				return true;
			}
		}

		private void End(Instruction instruction)
		{
			try
			{
				if (instruction != null)
					instruction.End(this);
			}
			catch (System.Exception exception)
			{
				Debug.LogException(exception, Owner);
			}
			finally
			{
				instruction.IsRunning = false;
			}
		}
	}
}
