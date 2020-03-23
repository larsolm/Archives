using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Battle/Battle Manager")]
	public class BattleManager : Singleton<BattleManager>
	{
		public Battle Battle;
		public BattleData Data;
		public List<Trainer> Trainers;
		private BattleInstructionContext _context;

		void Awake()
		{
			_context = new BattleInstructionContext(this, "Battle", null);
		}

		public BattleData StartBattle(Battle battle, List<Trainer> trainers)
		{
			Battle = battle;
			Data = battle.SetupBattle(trainers);
			Trainers = trainers;

			StartCoroutine(battle.Instructions.Run(_context, null));

			return Data;
		}
	}
}
