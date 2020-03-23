using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class Creature : ScriptableObject, ISerializationCallbackReceiver, IVariableListener
	{
		[Tooltip("The species that this creature is.")] public Species Species;
		public VariableStore Traits = new VariableStore();
		public List<Move> Moves = new List<Move>();
		public Dictionary<string, int> LearnedSkills = new Dictionary<string, int>();
		
		private EcosystemInstructionContext _context;

		public static Creature Create(Species species)
		{
			var creature = CreateInstance<Creature>();
			creature.Species = species;
			return creature;
		}

		private void Awake()
		{
			Traits.Subscribe(this, null);
			_context = new EcosystemInstructionContext(this, Species.name, null);
		}

		public string Name
		{
			get { return string.IsNullOrEmpty(_name) ? Species.name : _name; }
			set { _name = value; }
		}

		public IEnumerator GenerateTraits(InstructionContext context)
		{
			return context.Execute(Species.Ecosystem.CreatureGenerationInstructions, this);
		}

		public IEnumerator UpdateTraits(InstructionContext context, string trait)
		{
			Instruction instruction;
			if (Species.Ecosystem.CreatureUpdateInstructions.TryGetValue(trait, out instruction))
			{
				var enumerator = context.Execute(instruction, this);

				while (enumerator.MoveNext())
					yield return null;
			}
		}

		public bool HasLearned(Skill skill)
		{
			return LearnCount(skill) > 0;
		}

		public bool CanLearn(Skill skill)
		{
			return skill.LearnLimit == 0 || LearnCount(skill) < skill.LearnLimit;
		}

		public int LearnCount(Skill skill)
		{
			int count;
			return LearnedSkills.TryGetValue(skill.Name, out count) ? count : 0;
		}

		public IEnumerator TeachSkill(InstructionContext context, Skill skill)
		{
			var count = LearnCount(skill);

			if (count < skill.LearnLimit)
			{
				LearnedSkills[skill.Name] = count + 1;

				var enumerator = skill.Instruction.Run(context, this);

				while (enumerator.MoveNext())
					yield return null;
			}
		}

		public void ForgetSkill(Skill skill)
		{
			LearnedSkills[skill.Name] = 0;
		}

		public IEnumerator UpdateSkills(InstructionContext context, string name)
		{
			// TODO: probably a better way to store this rather than iterating the entire list
			foreach (var skill in Species.Skills)
			{
				if (skill.Triggers.Contains(name))
				{
					var enumerator = TeachSkill(context, skill);

					while (enumerator.MoveNext())
						yield return null;
				}
			}
		}

		public void CleanUpSkills()
		{
			Dictionary<string, int> learned = new Dictionary<string, int>();

			foreach (var learnedSkill in LearnedSkills)
			{
				var skill = Species.GetSkill(learnedSkill.Key);
				if (skill != null)
					learned.Add(learnedSkill.Key, learnedSkill.Value);
			}

			LearnedSkills = learned;
		}

		[Serializable]
		private struct LearnedSkill
		{
			public string Name;
			public int Count;
		}

		public void OnBeforeSerialize()
		{
			_learnedSkills.Clear();

			foreach (var skill in LearnedSkills)
				_learnedSkills.Add(new LearnedSkill { Name = skill.Key, Count = skill.Value });
		}

		public void OnAfterDeserialize()
		{
			LearnedSkills.Clear();

			foreach (var skill in _learnedSkills)
				LearnedSkills.Add(skill.Name, skill.Count);

			_learnedSkills.Clear();
		}

		public void OnVariableAdded(VariableValue variable, object owner)
		{
		}

		public void OnVariableChanged(VariableValue variable, object owner)
		{
			InstructionManager.Instance.StartCoroutine(UpdateTraits(_context, variable.Name));
			InstructionManager.Instance.StartCoroutine(UpdateSkills(_context, variable.Name));
		}

		public void OnVariableRemoved(string name, object owner)
		{
		}

		[SerializeField] private string _name;
		[SerializeField] private List<LearnedSkill> _learnedSkills = new List<LearnedSkill>();
	}
}
