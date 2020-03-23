using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public interface IInteractable
	{
		bool IsInteracting();
		bool CanInteract(MovementDirection direction);
		void Interact();
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "interaction")]
	[AddComponentMenu("PiRho Soft/World/Interaction")]
	public class Interaction : MonoBehaviour, IInteractable, IVariableStore
	{
		private const string _missingOccupierWarning = "(WIMO) The Interaction '{0}' needs either a Mover or StaticCollider";

		[Tooltip("The directions that the player can be in order to interact with this object")]
		[EnumButtons]
		public InteractionDirection Directions;

		[Tooltip("The instructions to run when the player interacts with this object")]
		public InstructionCaller Instructions = new InstructionCaller();

		protected virtual void Awake()
		{
			if (GetComponent<Mover>() == null && GetComponent<StaticCollider>() == null)
				Debug.LogWarningFormat(this, _missingOccupierWarning, gameObject.name);
		}

		public virtual bool IsInteracting()
		{
			return Instructions != null && Instructions.IsRunning;
		}

		public virtual bool CanInteract(MovementDirection direction)
		{
			return Direction.Contains(Directions, Direction.Opposite(direction)) && !IsInteracting();
		}

		public virtual void Interact()
		{
			if (Instructions != null && Instructions.Instruction != null)
				WorldManager.Instance.StartCoroutine(DoInteract());
		}

		private IEnumerator DoInteract()
		{
			InstructionManager.Instance.RunInstruction(Instructions, WorldManager.Instance.Context, this);

			while (Instructions.IsRunning)
				yield return null;
		}

		#region IVariableStore Implementation;

		public VariableValue GetVariable(string name) => VariableValue.Empty;
		public SetVariableResult SetVariable(string name, VariableValue value) => SetVariableResult.NotFound;
		public IEnumerable<string> GetVariableNames() => Enumerable.Empty<string>();

		#endregion
	}
}
