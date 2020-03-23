using PiRhoSoft.CompositionEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class NpcSaveData
	{
		public string Id;
		public Vector2Int Position;
		public MovementDirection Direction;
		public string ControllerData;
		public VariableList NpcTraits = new VariableList();
		public VariableList TrainerTraits = new VariableList();
	}

	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "npc")]
	[AddComponentMenu("PiRho Soft/World/Npc")]
	public class Npc : MonoBehaviour, IVariableStore
	{
		[Tooltip("The name of the character")]
		public string Name = "";

		[HideInInspector] public string Guid;

		public Controller Controller { get; private set; }
		public Trainer Trainer { get; private set; }

		private VariableList _variables = new VariableList();
		private MappedVariableStore _variableStore = new MappedVariableStore();

		public Npc()
		{
			Guid = System.Guid.NewGuid().ToString();
		}

		void Awake()
		{
			Controller = GetComponent<Controller>();
			Trainer = GetComponent<Trainer>();
		}

		#region Variables

		private static VariableMap _variableMap;
		private static PropertyMap<Npc> _propertyMap;

		public MappedVariableStore Variables => _variableStore;
		
		protected static VariableValue GetName(Npc npc) => VariableValue.Create(npc.Name);

		protected void AddPropertiesToMap<NpcType>(PropertyMap<NpcType> map) where NpcType : Npc
		{
			map.Add(nameof(Name), GetName, null);
		}

		protected virtual void SetupVariables()
		{
			if (_propertyMap == null)
			{
				_propertyMap = new PropertyMap<Npc>();
				AddPropertiesToMap(_propertyMap);
			}

			if (_variableMap == null || _variableMap.Version != WorldManager.Instance.World.NpcSchema.Version)
			{
				_variableMap = new VariableMap(WorldManager.Instance.World.NpcSchema.Version)
					.Add(_propertyMap)
					.Add(WorldManager.Instance.World.NpcSchema);
			}

			_variableStore.Setup(_variableMap, new PropertyList<Npc>(this, _propertyMap), _variables);
			_variables.Setup(WorldManager.Instance.World.NpcSchema, this);
		}

		#endregion

		#region Persistence

		public virtual void Load(NpcSaveData saveData)
		{
			SetupVariables();

			if (saveData != null)
			{
				_variables.LoadFrom(saveData.NpcTraits, VariableDefinition.Saved);

				if (Controller)
				{
					Controller.Mover.WarpToPosition(saveData.Position, saveData.Direction, CollisionLayer.None);
					Controller.Load(saveData.ControllerData);
				}

				if (Trainer)
					Trainer.Traits.LoadFrom(saveData.TrainerTraits, VariableDefinition.Saved);
			}
		}

		public virtual void Save(NpcSaveData saveData)
		{
			_variables.SaveTo(saveData.NpcTraits, VariableDefinition.Saved);

			if (Controller)
			{
				saveData.Position = Controller.Mover.CurrentGridPosition;
				saveData.Direction = Controller.Mover.MovementDirection;
				saveData.ControllerData = Controller.Save();
			}

			if (Trainer)
				Trainer.Traits.SaveTo(saveData.TrainerTraits, VariableDefinition.Saved);
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _variableStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _variableStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _variableStore.GetVariableNames();

		#endregion
	}
}
