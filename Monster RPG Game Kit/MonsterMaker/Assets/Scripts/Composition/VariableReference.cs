using System;

namespace PiRhoSoft.MonsterMaker
{
	public enum VariableLocation
	{
		Owner,
		Parent,
		Scene,
		Context,
		Custom,
		None
	}

	public class VariableTypeAttribute : Attribute
	{
		public Type Type;
		public Type ParentType;

		public VariableTypeAttribute(Type type)
		{
			Type = type;
		}

		public VariableTypeAttribute(Type type, Type parentType)
		{
			Type = type;
			ParentType = parentType;
		}
	}

	[Serializable]
	public class VariableReference
	{
		public VariableLocation Location = VariableLocation.Context;
		public string CustomSource;
		public int CustomIndex;
		public string Name;
	}
}
