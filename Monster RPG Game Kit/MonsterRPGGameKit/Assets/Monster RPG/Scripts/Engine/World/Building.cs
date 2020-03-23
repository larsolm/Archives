using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiRhoSoft.MonsterRpgEngine
{
	[SelectionBase]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SortingGroup))]
	[HelpURL(MonsterRpg.DocumentationUrl + "building")]
	[AddComponentMenu("PiRho Soft/World/Building")]
	public class Building : MonoBehaviour
	{
		[Serializable]
		public class Part
		{
			public GameObject GameObject;
			public int OrderOffset;
			public SpriteRenderer Renderer;
			public Rect Bounds;
		}

		[Serializable]
		public class Accessory : Part
		{
			public AudioSource Audio;
			public Animator Animator;
			public SimpleAnimationPlayer Animation;
		}

		[Serializable]
		public class DoorPart : Part
		{
			public AudioSource Audio;
			public Animator Animator;
			public Door Door;
		}

		[Tooltip("The collision layer that this building will be sorted on")]
		public CollisionLayer CollisionLayer = CollisionLayer.One;

		[Tooltip("The bounds of the building (in global space)")]
		public Rect Bounds = new Rect(Vector2.zero, new Vector2(4, 4));

		[Tooltip("The distance from the bottom of the building to use as a sort point")]
		public float SortPoint = 0.45f;

		[Tooltip("The height of the roof on this building")]
		public int RoofHeight = 3;

		[Tooltip("The facade of this building")]
		public Part Roof;

		[Tooltip("The roof of this building")]
		public Part Facade;

		[Tooltip("The doors on this building")]
		public List<DoorPart> Doors = new List<DoorPart>();

		[Tooltip("The accessories on this building")]
		public List<Accessory> Accessories = new List<Accessory>();

		void Awake()
		{
			var sorting = GetComponent<SortingGroup>();
			sorting.sortingOrder = LayerSorting.GetSortingOrder(CollisionLayer);

			SetSortOrder(Roof);
			SetSortOrder(Facade);

			foreach(var door in Doors)
				SetSortOrder(door);

			foreach (var accessory in Accessories)
				SetSortOrder(accessory);
		}

		private void SetSortOrder(Part part)
		{
			part.Renderer.sortingOrder = part.OrderOffset;
		}
	}
}
