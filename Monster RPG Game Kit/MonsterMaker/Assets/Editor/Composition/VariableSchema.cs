using System;
using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class VariableDefinition
	{
		public string Name;
		public VariableType Type;
	}

	[Serializable]
	public class VariableSchema
	{
		public string Name;
		public List<VariableDefinition> Definitions = new List<VariableDefinition>();

		public VariableSchema(string name)
		{
			Name = name;
		}

		public void AddDefinition(string name, VariableType type)
		{
			Definitions.Add(new VariableDefinition { Name = name, Type = type });
		}

		public bool Contains(string name)
		{
			for (var i = 0; i < Definitions.Count; i++)
			{
				if (Definitions[i].Name == name)
					return true;
			}

			return false;
		}

		public void Apply(VariableStore store, bool removeMissing)
		{
			if (removeMissing)
			{
				for (var i = 0; i < store.Variables.Count; i++)
				{
					if (!Contains(store.Variables[i].Name))
						store.Remove(i--);
				}
			}

			foreach (var definition in Definitions)
			{
				var variable = store.GetVariable(definition.Name);

				if (variable == null)
					store.Add(VariableValue.Create(definition.Name, definition.Type));
				else if (variable.Type != definition.Type)
					variable.ChangeType(definition.Type);
			}
		}
	}
}
