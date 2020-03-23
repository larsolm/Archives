using System.Collections.Generic;
using System.Linq;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum WorldListenerSource
	{
		Zone,
		World,
		Player
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "world-listener")]
	[AddComponentMenu("PiRho Soft/World/World Listener")]
	public class WorldListener : MonoBehaviour, IVariableStore
	{
		public enum VariableState
		{
			Awake,
			Enabled,
			Changed
		}

		[Tooltip("The variable store source of the variable to listen for change on")]
		[EnumButtons]
		public WorldListenerSource Source = WorldListenerSource.Zone;

		[Tooltip("The name of the variable to listen for changes on")]
		public string Variable = "";

		[Tooltip("The instructions to run on awake for this listener (usually a WorldListenerGraph)")]
		public InstructionCaller Instructions = new InstructionCaller();

		public VariableState State { get; private set; }

		void Awake()
		{
			State = VariableState.Awake;
			RunInstructions(Instructions);

			var zone = WorldManager.Instance.GetZone(this);
			zone.Listeners.Add(this);
		}

		void OnDestroy()
		{
			var zone = WorldManager.Instance?.GetZone(this);
			zone?.Listeners.Remove(this);
		}

		void OnEnable()
		{
			if (State != VariableState.Awake)
			{
				State = VariableState.Enabled;
				RunInstructions(Instructions);
			}
		}

		public void OnVariableChanged(WorldListenerSource source, string variable)
		{
			if (State != VariableState.Awake && source == Source && variable == Variable)
			{
				State = VariableState.Changed;
				RunInstructions(Instructions);
			}
		}

		private void RunInstructions(InstructionCaller instructions)
		{
			if (instructions != null && instructions.Instruction != null)
				InstructionManager.Instance.RunInstruction(instructions, WorldManager.Instance.Context, this);
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => VariableValue.Empty;
		public SetVariableResult SetVariable(string name, VariableValue value) => SetVariableResult.NotFound;
		public IEnumerable<string> GetVariableNames() => Enumerable.Empty<string>();

		#endregion
	}
}
