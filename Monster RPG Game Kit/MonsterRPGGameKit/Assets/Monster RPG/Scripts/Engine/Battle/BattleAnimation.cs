using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum CreatureDisplayTarget
	{
		Battler,
		Creature
	}

	public enum BattleAnimationSource
	{
		Display,
		Variables
	}

	[Serializable]
	public class BattleAnimationClip
	{
		[Tooltip("Whether or not to wait for the animation to complete")]
		public bool WaitForCompletion = true;

		[Tooltip("The interface object to trigger the animation on - ether an animation defined by the Battler or as an animation parameter on the Creature's AnimatorController")]
		[EnumButtons(MinimumWidth = 60.0f)]
		public CreatureDisplayTarget AnimationTarget;

		[Tooltip("The location to find the asset associated with the animation - either in the CreatureDisplay's dictionary or on the Creature's VariableStore")]
		[EnumButtons(MinimumWidth = 50.0f)]
		[ConditionalDisplaySelf(nameof(AnimationTarget), EnumValue = (int)CreatureDisplayTarget.Battler)]
		public BattleAnimationSource AnimationSource;

		[Tooltip("The name of the animation to look up from AnimationSource")]
		[ConditionalDisplaySelf(nameof(AnimationTarget), EnumValue = (int)CreatureDisplayTarget.Battler)]
		public string Animation;

		[Tooltip("The name of the parameter to trigger")]
		[ConditionalDisplaySelf(nameof(AnimationTarget), EnumValue = (int)CreatureDisplayTarget.Creature)]
		public string Parameter;
	}

	[Serializable]
	public class BattleAnimationSound
	{
		[Tooltip("Whether or not to wait for the animation to complete")]
		public bool WaitForCompletion = true;

		[Tooltip("The location to find the asset associated with the sound - either in the CreatureDisplay's dictionary or on the Creature's VariableStore")]
		[EnumButtons(MinimumWidth = 50.0f)]
		public BattleAnimationSource SoundSource;

		[Tooltip("The name of the asset as looked up on Source")]
		public string Sound;
	}

	[Serializable]
	public class BattleAnimationEffect
	{
		[Tooltip("Whether or not to wait for the animation to complete")]
		public bool WaitForCompletion = true;

		[Tooltip("The location to find the asset associated with the effect - either in the CreatureDisplay's dictionary or on the Creature's VariableStore")]
		[EnumButtons(MinimumWidth = 50.0f)]
		public BattleAnimationSource EffectSource;

		[Tooltip("The name of the effect to look up from EffectSource")]
		public string Effect;

		[Tooltip("The name of the mount point indicating where to place the effect")]
		public string MountPoint = "";

		[Tooltip("The location to search for a MountPoint - ether as a child of the Battler object or as specified on the Creature's MountPoints dictionary")]
		[EnumButtons(MinimumWidth = 60.0f)]
		[ConditionalDisplaySelf(nameof(MountPoint), StringValue = "", Invert = true)]
		public CreatureDisplayTarget EffectTarget;

		[Tooltip("Whether or not the effect should follow the object its parent object if it moves - useful for particle systems")]
		public bool FollowParent = true;
	}

	[Serializable] public class BattleAnimationClipDictionary : SerializedDictionary<string, BattleAnimationClip> { }
	[Serializable] public class BattleAnimationSoundDictionary : SerializedDictionary<string, BattleAnimationSound> { }
	[Serializable] public class BattleAnimationEffectDictionary : SerializedDictionary<string, BattleAnimationEffect> { }
}
