using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public static class InstructionBreadcrumbs
	{
		// TODO: handle objects in history having been destroyed (they will compare equal to null)

		public static bool HasEntry()
		{
			return _breadcrumbs.Count > 0;
		}

		public static bool HasRoot()
		{
			return Selection.activeObject is Instruction;
		}

		public static bool HasPreviousEntry()
		{
			return _currentEntry > 0;
		}
		
		public static bool HasNextEntry()
		{
			return _currentEntry < _breadcrumbs.Count - 1;
		}

		public static void Reset()
		{
			if (!_skipReset)
			{
				_currentEntry = 0;
				_breadcrumbs.Clear();
			}

			_skipReset = false;
		}

		public static void Edit(Object obj)
		{
			Push(Selection.activeObject);
			Show(obj);
		}

		public static void Back()
		{
			if (_currentEntry > 0)
			{
				if (_currentEntry == _breadcrumbs.Count)
					_breadcrumbs.Add(Selection.activeObject);

				Show(_breadcrumbs[--_currentEntry]);
			}
		}

		public static void Forward()
		{
			if (_currentEntry < _breadcrumbs.Count - 1)
				Show(_breadcrumbs[++_currentEntry]);
		}

		public static void Root()
		{
			var instruction = Selection.activeObject as Instruction;

			if (instruction != null)
				Edit(instruction.Set);
		}

		private const int _capacity = 10;
		private static int _currentEntry = 0;
		private static List<Object> _breadcrumbs = new List<Object>(_capacity);
		private static bool _skipReset = false;

		private static void Push(Object obj)
		{
			var trailing = _breadcrumbs.Count - _currentEntry;

			if (trailing > 0)
				_breadcrumbs.RemoveRange(_currentEntry, trailing);

			if (_currentEntry == _capacity)
				_breadcrumbs.RemoveAt(0);

			_breadcrumbs.Add(obj);
			_currentEntry = _breadcrumbs.Count;
		}

		private static void Show(Object obj)
		{
			_skipReset = obj is InstructionSet;
			EditorHelper.Edit(obj);
		}
	}
}
