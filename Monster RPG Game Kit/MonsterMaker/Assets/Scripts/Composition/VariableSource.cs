using System;

namespace PiRhoSoft.MonsterMaker
{
	public class VariableSourceAttribute : Attribute
	{
		public VariableSourceAttribute(string name)
		{
			Name = name;
		}

		public VariableSourceAttribute(string name, string[] parameters1)
		{
			Name = name;
			Parameters1 = parameters1;
		}

		public VariableSourceAttribute(string name, string[] parameters1, string[] parameters2)
		{
			Name = name;
			Parameters1 = parameters1;
			Parameters2 = parameters2;
		}

		public string Name { get; private set; }
		public string[] Parameters1 { get; private set; }
		public string[] Parameters2 { get; private set; }
	}
}
