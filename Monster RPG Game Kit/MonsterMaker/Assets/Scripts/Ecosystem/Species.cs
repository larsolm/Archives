using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class Species : ScriptableObject
	{
		public Ecosystem Ecosystem;

		[Tooltip("The traits that belong to this species.")] public VariableStore Traits = new VariableStore();
		[Tooltip("The skills that this species can learn.")] public List<Skill> Skills = new List<Skill>();

		public Skill GetSkill(string name)
		{
			foreach (var skill in Skills)
			{
				if (skill.Name == name)
					return skill;
			}

			return null;
		}
	}
}
