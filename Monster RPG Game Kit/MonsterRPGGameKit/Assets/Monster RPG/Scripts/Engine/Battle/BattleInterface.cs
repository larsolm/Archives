using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[RequireComponent(typeof(Canvas))]
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "battle-interface")]
	[AddComponentMenu("PiRho Soft/Interface/Battle Interface")]
	public class BattleInterface : Interface
	{
		[Tooltip("Set this to disable the World Camera when the Battle is active")]
		public bool HideWorld = true;

		[Tooltip("Add items to this list to link interface elements with Creatures in the Battle")]
		[ListDisplay(EmptyText = "No displays have been added")]
		public CreatureDisplayList CreatureDisplays =  new CreatureDisplayList();

		protected override void Setup()
		{
			base.Setup();

			if (HideWorld)
			{
				var camera = ComponentHelper.GetComponentInScene<Camera>(WorldManager.Instance.gameObject.scene.buildIndex, false);
				camera.gameObject.SetActive(false);
			}

			foreach (var display in CreatureDisplays)
				display.SetCreature(null);
		}

		protected override void Teardown()
		{
			if (HideWorld)
			{
				var camera = ComponentHelper.GetComponentInScene<Camera>(WorldManager.Instance.gameObject.scene.buildIndex, true);
				camera.gameObject.SetActive(true);
			}

			base.Teardown();
		}

		public CreatureDisplay GetCreatureDisplay(int index)
		{
			return index >= 0 && index < CreatureDisplays.Count ? CreatureDisplays[index] : null;
		}

		private int WrapCreatureIndex(int index)
		{
			return CreatureDisplays.Count > 0 ? MathHelper.Wrap(index, CreatureDisplays.Count) : -1;
		}
	}
}
