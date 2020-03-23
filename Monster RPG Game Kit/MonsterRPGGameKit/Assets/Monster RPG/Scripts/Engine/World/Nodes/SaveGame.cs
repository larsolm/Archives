using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Save Game", 100)]
	[HelpURL(MonsterRpg.DocumentationUrl + "save-game")]
	public class SaveGame : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("An optional game object to create and show while the save is occurring")]
		public GameObject SavingIndicator;

		public override Color NodeColor => Colors.ExecutionLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var indicator = CreateIndicator();
			var info = WorldLoader.Save(WorldManager.Instance);

			while (info.State != SaveState.Complete)
			{
				if (info.State == SaveState.Error)
				{
					Debug.LogError(info.Message, this);
					break;
				}
				else
				{
					yield return null;
				}
			}

			yield return new WaitForSecondsRealtime(1.0f);

			if (indicator)
				Destroy(indicator);

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private GameObject CreateIndicator()
		{
			if (SavingIndicator)
			{
				var canvas = FindObjectOfType<Canvas>();
				if (canvas)
					return Instantiate(SavingIndicator, canvas.transform);
			}

			return null;
		}
	}
}
