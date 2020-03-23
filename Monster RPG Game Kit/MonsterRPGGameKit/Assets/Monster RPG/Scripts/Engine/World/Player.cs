using System;
using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class PlayerSaveData
	{
		public string Name = "";
		public VariableList PlayerTraits = new VariableList();
		public VariableList TrainerTraits = new VariableList();
		public List<CreatureSaveData> Creatures = new List<CreatureSaveData>();
		public List<ItemSaveData> Items = new List<ItemSaveData>();
	}

	[Serializable]
	public class ItemSaveData
	{
		public string ItemPath;
		public int Count;
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Trainer))]
	[RequireComponent(typeof(PlayerController))]
	[HelpURL(MonsterRpg.DocumentationUrl + "player")]
	[AddComponentMenu("PiRho Soft/World/Player")]
	public class Player : SingletonBehaviour<Player>, IVariableStore, IVariableListener
	{
		private const string _missingSpeciesWarning = "(WPMS) The Species at the path {0} for the Player's Creature could not be found";

		[Tooltip("The name of the player")]
		public string Name = "";

		private VariableList _variables = new VariableList();
		private MappedVariableStore _variableStore = new MappedVariableStore();

		public ZoneData Zone { get; internal set; }
		public Mover Mover { get; private set; }
		public Trainer Trainer { get; private set; }
		public PlayerController Controller { get; private set; }
		public IInteractable Interaction { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			Mover = GetComponent<Mover>();
			Trainer = GetComponent<Trainer>();
			Controller = GetComponent<PlayerController>();
		}

		protected virtual void Start()
		{
			Mover.OnWarp += OnSpawn;
			Mover.OnTileEntering += OnTileEntering;
			Mover.OnTileExiting += OnTileExiting;
			Mover.OnTileChanged += OnTileChanged;
		}

		protected virtual void FixedUpdate()
		{
			UpdateInteraction();
		}

		#region Variables

		private static VariableMap _variableMap;
		private static PropertyMap<Player> _propertyMap;

		public MappedVariableStore Variables => _variableStore;

		protected static VariableValue GetZone(Player player) => VariableValue.Create(player.Zone);
		protected static VariableValue GetInteraction(Player player) => VariableValue.Create(player.Interaction);
		protected static VariableValue GetInventory(Player player) => VariableValue.Create(player.Trainer.Inventory);
		protected static VariableValue GetName(Player player) => VariableValue.Create(player.Name);
		protected static SetVariableResult SetName(Player player, VariableValue value) => value.TryGetString(out player.Name) ? SetVariableResult.Success : SetVariableResult.TypeMismatch;

		protected void AddPropertiesToMap<PlayerType>(PropertyMap<PlayerType> map) where PlayerType : Player
		{
			map.Add(nameof(Zone), GetZone, null);
			map.Add(nameof(Interaction), GetInteraction, null);
			map.Add(nameof(Trainer.Inventory), GetInventory, null);
			map.Add(nameof(Name), GetName, SetName);
		}

		protected virtual void SetupVariables(VariableList savedVariables)
		{
			if (_propertyMap == null)
			{
				_propertyMap = new PropertyMap<Player>();
				AddPropertiesToMap(_propertyMap);
			}

			if (_variableMap == null || _variableMap.Version != WorldManager.Instance.World.PlayerSchema.Version)
			{
				_variableMap = new VariableMap(WorldManager.Instance.World.PlayerSchema.Version)
					.Add(_propertyMap)
					.Add(WorldManager.Instance.World.PlayerSchema);
			}

			_variableStore.Setup(_variableMap, new PropertyList<Player>(this, _propertyMap), new VariableListener(this, _variables));
			_variables.Setup(WorldManager.Instance.World.PlayerSchema, this);
			_variables.LoadFrom(savedVariables, VariableDefinition.Saved);
		}

		#endregion

		#region Interaction

		public bool CanInteract => Interaction != null && !Interaction.IsInteracting() && Interaction.CanInteract(_isCurrentTile ? MovementDirection.None : Mover.MovementDirection) && Mover.CanInteract && !Controller.IsFrozen;
		public bool IsInteracting => Interaction != null && Interaction.IsInteracting();

		private bool _isCurrentTile;

		public void Interact()
		{
			if (CanInteract)
				Interaction.Interact();
		}

		public void ForceInteract(Interaction interaction)
		{
			Interaction = interaction;
			interaction.Interact();
		}

		protected void UpdateInteraction()
		{
			if (!IsInteracting)
			{
				_isCurrentTile = false;
				Interaction = WorldManager.Instance.GetInteraction(Mover.CurrentGridPosition + Mover.DirectionVector);

				if (Interaction == null)
				{
					Interaction = WorldManager.Instance.GetInteraction(Mover.CurrentGridPosition);
					_isCurrentTile = true;
				}
			}
		}

		#endregion

		#region World Callbacks

		protected virtual void OnSpawn(Vector2Int postion)
		{
		}

		protected virtual void OnTileEntering(Vector2Int position)
		{
			if (!IsInteracting)
			{
				var tile = WorldManager.Instance.FindTile(position);
				if (tile != null)
				{
					if (tile.HasInstructions)
						tile.Instructions.Entering(Mover.MovementDirection);
				}
			}
		}

		protected virtual void OnTileExiting(Vector2Int position)
		{
			if (!IsInteracting)
			{
				var tile = WorldManager.Instance.FindTile(position);
				if (tile != null)
				{
					if (tile.HasInstructions)
						tile.Instructions.Exiting(Mover.MovementDirection);
				}
			}
		}

		protected virtual void OnTileChanged(Vector2Int previous, Vector2Int current)
		{
			if (!IsInteracting)
			{
				var previousTile = WorldManager.Instance.FindTile(previous);
				var currentTile = WorldManager.Instance.FindTile(current);

				if (previousTile != null)
				{
					if (previousTile.HasZoneTrigger && currentTile != null && currentTile.HasZoneTrigger && !previousTile.IsSameZoneAs(currentTile) && currentTile.Zone.TargetZone == Zone.Zone)
						previousTile.Zone.Exit();

					if (previousTile.HasInstructions)
						previousTile.Instructions.Exit(Mover.MovementDirection);
				}

				if (currentTile != null)
				{
					if (currentTile.HasEncounter && currentTile.Encounter != null)
						currentTile.Encounter.Enter();

					if (currentTile.HasInstructions)
						currentTile.Instructions.Enter(Mover.MovementDirection);

					if (currentTile.HasZoneTrigger && !currentTile.IsSameZoneAs(previousTile))
						currentTile.Zone.Enter();
				}
			}
		}

		#endregion

		#region Persistence

		public virtual void Load(PlayerSaveData saveData)
		{
			SetupVariables(saveData.PlayerTraits);

			// If the save data has creatures or items, anything set on the actual Player needs to be cleared so it
			// isn't duplicated on every save. This means if a new creature or item is added to the Player in the
			// editor existing saves will not include it.
			if (saveData.Creatures.Count > 0) Trainer.Roster.Clear();
			if (saveData.Items.Count > 0) Trainer.Inventory.Items.Clear();
			if (!string.IsNullOrEmpty(saveData.Name)) Name = saveData.Name;

			Trainer.Traits.LoadFrom(saveData.TrainerTraits, VariableDefinition.Saved);

			foreach (var creatureData in saveData.Creatures)
			{
				var creature = Creature.Create(creatureData, Trainer);

				if (creature != null)
					Trainer.Roster.AddCreature(creature);
				else
					Debug.LogWarningFormat(_missingSpeciesWarning, creatureData.SpeciesPath);
			}

			foreach (var itemData in saveData.Items)
			{
				var item = Resources.Load<Item>(itemData.ItemPath);
				Trainer.Inventory.Add(item, itemData.Count);
			}
		}

		public virtual void Save(PlayerSaveData saveData)
		{
			_variables.SaveTo(saveData.PlayerTraits, VariableDefinition.Saved);

			saveData.Name = Name;
			Trainer.Traits.SaveTo(saveData.TrainerTraits, VariableDefinition.Saved);

			foreach (var creature in Trainer.Roster)
				saveData.Creatures.Add(Creature.Save(creature.Creature));

			foreach (var item in Trainer.Inventory.Items)
				saveData.Items.Add(new ItemSaveData { ItemPath = item.Item.Path, Count = item.Count });
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _variableStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _variableStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _variableStore.GetVariableNames();

		#endregion

		#region IVariableListener Implementation

		public void VariableChanged(int index, VariableValue value)
		{
			var variable = _variables.GetVariableName(index);

			foreach (var zone in WorldManager.Instance.LoadedZones)
				zone.VariableChanged(WorldListenerSource.Player, variable);
		}

		#endregion
	}
}
