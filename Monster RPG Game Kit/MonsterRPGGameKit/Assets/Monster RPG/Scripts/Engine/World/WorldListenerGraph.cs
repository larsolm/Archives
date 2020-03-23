using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateAssetMenu(fileName = nameof(WorldListenerGraph), menuName = "PiRho Soft/Graphs/World Listener", order = 200)]
	[HelpURL(MonsterRpg.DocumentationUrl + "world-listener-graph")]
	public class WorldListenerGraph : InstructionGraph
	{
		private const string _listenerInvalidWarning = "(WWLGLI) Unable to run WorldListenerGraph '{0}': the given variables must be a WorldListener";

		[Tooltip("The node to run when the listener is first loaded")] public InstructionGraphNode OnAwake = null;
		[Tooltip("The node to run when the listener is enabled")] public InstructionGraphNode OnEnabled = null;
		[Tooltip("The node to run when the listener's variable is changed")] public InstructionGraphNode OnVariableChanged = null;

		protected override IEnumerator Run(InstructionStore variables)
		{
			if (variables.This is WorldListener listener)
			{
				switch (listener.State)
				{
					case WorldListener.VariableState.Awake: yield return Run(variables, OnAwake, nameof(OnAwake)); yield break;
					case WorldListener.VariableState.Enabled: yield return Run(variables, OnEnabled, nameof(OnEnabled)); yield break;
					case WorldListener.VariableState.Changed: yield return Run(variables, OnVariableChanged, nameof(OnVariableChanged)); yield break;
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _listenerInvalidWarning, name);
			}
		}
	}
}
