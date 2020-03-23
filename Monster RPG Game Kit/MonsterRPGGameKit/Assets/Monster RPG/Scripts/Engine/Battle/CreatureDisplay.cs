using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class CreatureDisplayList : SerializedList<CreatureDisplay> { }

	[Serializable] public class AnimationClipDictionary : SerializedDictionary<string, AnimationClip> { }
	[Serializable] public class AudioClipDictionary : SerializedDictionary<string, AudioClip> { }
	[Serializable] public class GameObjectDictionary : SerializedDictionary<string, GameObject> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "creature-display")]
	[AddComponentMenu("PiRho Soft/Interface/Creature Display")]
	public class CreatureDisplay : InterfaceControl
	{
		private const string _missingBattlerPlayerError = "(BCDMBP) The BattlerPlayer has not been set for this display";
		private const string _missingCreatureAnimatorError = "(BCDMCA) The CreatureAnimator has not been set for this display";
		private const string _missingAnimationWarning = "(BCDMA) failed to play animation {0}: the animation could not be found on the {1}";
		private const string _missingSoundWarning = "(BCDMS) failed to play sound {0}: the sound could not be found on the {1}";
		private const string _missingEffectWarning = "(BCDME) failed to play effect {0}: the effect could not be found on the {1}";
		private const string _mountMissingWarning = "(BCDMM) mount point {0} could not be found for effect {1}";

		[Tooltip("The animation player for running battle and ability clips")]
		public AnimationPlayer BattlerPlayer;

		[Tooltip("The audio player for running battle and ability sounds")]
		public AudioPlayer BattlerAudio;

		[Tooltip("The animator that controls creature animation playback")]
		public Animator CreatureAnimator;

		[Tooltip("The animations available to play for this creature")]
		[DictionaryDisplay(AddLabel = "Add Animation", EmptyText = "No Animations to Play", ShowEditButton = true)]
		public AnimationClipDictionary Animations = new AnimationClipDictionary();

		[Tooltip("The sounds available to play for this creature")]
		[DictionaryDisplay(AddLabel = "Add Sound", EmptyText = "No Sounds to Play", ShowEditButton = true)]
		public AudioClipDictionary Sounds = new AudioClipDictionary();

		[Tooltip("The effects available to spawn for this creature")]
		[DictionaryDisplay(AddLabel = "Add Effect", EmptyText = "No Effects to Spawn", ShowEditButton = true)]
		public GameObjectDictionary Effects = new GameObjectDictionary();

		public CreatureContext Creature { get; private set; }

		void Start()
		{
			if (BattlerPlayer)
			{
				var player = BattlerPlayer.GetComponent<BattleAnimationEvents>();
				if (player) player.Display = this;
			}
			else
			{
				Debug.LogError(_missingBattlerPlayerError, this);
			}

			if (CreatureAnimator)
			{
				var animator = CreatureAnimator.GetComponent<BattleAnimationEvents>();
				if (animator) animator.Display = this;
			}
			else
			{
				Debug.LogError(_missingCreatureAnimatorError, this);
			}
		}

		public void SetCreature(CreatureContext creature)
		{
			Creature = creature;

			if (Creature != null)
				CreatureAnimator.runtimeAnimatorController = Creature.Creature.Species.Animations;
			
			if (Creature != null && !IsActive)
				Activate();
			
			if (Creature == null && IsActive)
				Deactivate();

			UpdateCreature(null, null);
		}

		public void UpdateCreature(string group, BindingAnimationStatus status)
		{
			if (Creature != null)
				InterfaceBinding.UpdateBindings(gameObject, Creature.Creature, group, status);
		}

		#region Animation Clips

		public IEnumerator PlayAndWait(BattleAnimationClip animation, IVariableStore variables)
		{
			switch (animation.AnimationTarget)
			{
				case CreatureDisplayTarget.Battler:
				{
					var clip = GetAnimation(animation, variables);

					if (clip)
						yield return BattlerPlayer.PlayAnimationAndWait(clip);
					else
						Debug.LogWarningFormat(this, _missingAnimationWarning, animation.Animation, animation.AnimationSource);

					break;
				}
				case CreatureDisplayTarget.Creature:
				{
					var state = CreatureAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
					var idleState = state;

					CreatureAnimator.SetTrigger(animation.Parameter);

					// wait until the animation enters the new state...

					while (state == idleState)
					{
						yield return null;
						state = CreatureAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
					}

					// then wait until the animation returns back to the beginning state

					while (state != idleState)
					{
						yield return null;
						state = CreatureAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
					}

					break;
				}
			}
		}

		public void Play(BattleAnimationClip animation, IVariableStore variables)
		{
			switch (animation.AnimationTarget)
			{
				case CreatureDisplayTarget.Battler:
				{
					var clip = GetAnimation(animation, variables);

					if (clip)
						BattlerPlayer.PlayAnimation(clip);
					else
						Debug.LogWarningFormat(this, _missingAnimationWarning, animation.Animation, animation.AnimationSource);

					break;
				}
				case CreatureDisplayTarget.Creature:
				{
					CreatureAnimator.SetTrigger(animation.Parameter);
					break;
				}
			}
		}

		private AnimationClip GetAnimation(BattleAnimationClip animation, IVariableStore variables)
		{
			AnimationClip clip = null;

			switch (animation.AnimationSource)
			{
				case BattleAnimationSource.Display: Animations.TryGetValue(animation.Animation, out clip); break;
				case BattleAnimationSource.Variables: variables.GetVariable(animation.Animation).TryGetObject(out clip); break;
			}

			return clip;
		}

		#endregion

		#region Sounds

		public IEnumerator PlayAndWait(BattleAnimationSound animation, IVariableStore variables)
		{
			var sound = GetSound(animation, variables);

			if (sound)
				yield return BattlerAudio.PlaySoundAndWait(sound, 1.0f);
			else
				Debug.LogWarningFormat(this, _missingSoundWarning, animation.Sound, animation.SoundSource);
		}

		public void Play(BattleAnimationSound animation, IVariableStore variables)
		{
			var sound = GetSound(animation, variables);

			if (sound)
				BattlerAudio.PlaySound(sound, 1.0f);
			else
				Debug.LogWarningFormat(this, _missingSoundWarning, animation.Sound, animation.SoundSource);
		}

		private AudioClip GetSound(BattleAnimationSound animation, IVariableStore variables)
		{
			AudioClip sound = null;

			switch (animation.SoundSource)
			{
				case BattleAnimationSource.Display: Sounds.TryGetValue(animation.Sound, out sound); break;
				case BattleAnimationSource.Variables: variables.GetVariable(animation.Sound).TryGetObject(out sound); break;
			}

			return sound;
		}

		#endregion

		#region Effects

		public IEnumerator PlayAndWait(BattleAnimationEffect animation, IVariableStore variables)
		{
			var effect = GetEffect(animation, variables);

			if (effect)
			{
				var parent = GetParent(animation, out bool deleteParent);
				var system = Instantiate(effect, parent);

				if (!animation.FollowParent)
					system.transform.SetParent(null, true);

				yield return null; // wait for awake in the spawned object

				var completionNotifier = system.GetComponentInChildren<ICompletionNotifier>();
				var particles = system.GetComponentInChildren<ParticleSystem>();

				while ((completionNotifier != null && !completionNotifier.IsComplete) || (particles != null && particles.IsAlive()))
					yield return null;

				if (deleteParent)
					Destroy(parent.gameObject);
				
				if (!deleteParent || !animation.FollowParent)
					Destroy(system.gameObject);
			}
			else
			{
				Debug.LogWarningFormat(this, _missingEffectWarning, animation.Effect, animation.EffectSource);
			}
		}

		public void Play(BattleAnimationEffect animation, IVariableStore variables)
		{
			StartCoroutine(PlayAndWait(animation, variables)); // running this as a coroutine so the system gets destroyed when it finishes
		}

		private GameObject GetEffect(BattleAnimationEffect animation, IVariableStore variables)
		{
			GameObject effect = null;

			switch (animation.EffectSource)
			{
				case BattleAnimationSource.Display: Effects.TryGetValue(animation.Effect, out effect); break;
				case BattleAnimationSource.Variables: variables.GetVariable(animation.Effect).TryGetObject(out effect); break;
			}

			return effect;
		}

		private Transform GetParent(BattleAnimationEffect animation, out bool deleteParent)
		{
			deleteParent = false;

			switch (animation.EffectTarget)
			{
				case CreatureDisplayTarget.Battler:
				{
					if (string.IsNullOrEmpty(animation.MountPoint))
						return BattlerPlayer.transform;
			
					var mountObject = BattlerPlayer.transform.Find(animation.MountPoint);
					
					if (mountObject)
						return mountObject;
			
					Debug.LogWarningFormat(this, _mountMissingWarning, animation.Effect, animation.MountPoint);
					return BattlerPlayer.transform;
				}
				case CreatureDisplayTarget.Creature:
				{
					if (string.IsNullOrEmpty(animation.MountPoint))
					{
						return CreatureAnimator.transform;
					}
					else if (Creature.Creature.Species.MountPoints.TryGetValue(animation.MountPoint, out var mount))
					{
						deleteParent = true;
			
						var renderer = CreatureAnimator.GetComponent<SpriteRenderer>();
						var offset = CreatureAnimator.transform.position - renderer.bounds.min;
						var x = mount.X * renderer.bounds.size.x;
						var y = mount.Y * renderer.bounds.size.y;
			
						var mountObject = new GameObject(animation.MountPoint);
						mountObject.transform.parent = CreatureAnimator.transform;
						mountObject.transform.localPosition = new Vector3(x - offset.x, y - offset.y);
						mountObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, mount.Rotation);
						return mountObject.transform;
					}
					else
					{
						Debug.LogWarningFormat(this, _mountMissingWarning, animation.Effect, animation.MountPoint);
						return CreatureAnimator.transform;
					}
				}
			}

			return transform;
		}

		#endregion
	}
}
