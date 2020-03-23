using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(Building))]
	public class BuildingEditor : Editor
	{
		private static readonly BoolPreference _advancedEditingPreference = new BoolPreference("MonsterRpgGameKit.BuildingEditor.AdvancedEditing", false);
		private static readonly IconButton _addDoorIcon = new IconButton(IconButton.Add, "Add a door to the building");
		private static readonly IconButton _removeDoorIcon = new IconButton(IconButton.Remove, "Remove this door");
		private static readonly IconButton _addAccessoryIcon = new IconButton(IconButton.Add, "Add an accessory to the building");
		private static readonly IconButton _removeAccessoryIcon = new IconButton(IconButton.Remove, "Remove this accessory");

		private static readonly GUIContent _advancedEditingContent = new GUIContent("Advanced Editing", "Show and allow manual editing of the building parts in the hierarchy");
		private static readonly Label _collisionLayerContent = new Label(typeof(Building), nameof(Building.CollisionLayer));
		private static readonly Label _boundsContent = new Label(typeof(Building), nameof(Building.Bounds));
		private static readonly Label _sortPointContent = new Label(typeof(Building), nameof(Building.SortPoint));
		private static readonly Label _roofHeightContent = new Label(typeof(Building), nameof(Building.RoofHeight));
		private static readonly Label _roofContent = new Label(typeof(Building), nameof(Building.Roof));
		private static readonly Label _facadeContent = new Label(typeof(Building), nameof(Building.Facade));
		private static readonly Label _doorsContent = new Label(typeof(Building), nameof(Building.Doors));
		private static readonly Label _accessoriesContent = new Label(typeof(Building), nameof(Building.Accessories));

		private static readonly GUIContent _accessoryPositionContent = new GUIContent("Position", "The position of this accessory (in global space)");
		private static readonly GUIContent _spriteContent = new GUIContent("Sprite", "The sprite that this building part displays");
		private static readonly GUIContent _animationsContent = new GUIContent("Animations", "The animations that can be triggered on this accessory");
		private static readonly GUIContent _orderOffsetContent = new GUIContent("Order Offset", "The sorting order of the sprite for this part");
		private static readonly GUIContent _openAnimationContent = new GUIContent("Open Animation", "The animation to play when this door opens");
		private static readonly GUIContent _closeAnimationContent = new GUIContent("Close Animation", "The animation to play when this door closes");
		private static readonly GUIContent _openSoundContent = new GUIContent("Open Sound", "The sound to play when this door opens");
		private static readonly GUIContent _closeSoundContent = new GUIContent("Close Sound", "The sound to play when this door closes");

		private static Array _collisionLayerValues = Enum.GetValues(typeof(CollisionLayer)).Cast<int>().Take(LayerSorting.LayerCount + 1).ToArray();
		private static GUIContent[] _collisionLayerNames = Enum.GetNames(typeof(CollisionLayer)).Take(LayerSorting.LayerCount + 1).Select(name => new GUIContent(name)).ToArray();

		private static readonly Vector2 _minimumSize = new Vector2(2.0f, 2.0f);

		private ObjectListControl _doorsControl = new ObjectListControl();
		private ObjectListControl _accessoriesControl = new ObjectListControl();

		private Building _building;

		void OnEnable()
		{
			_building = target as Building;

			if (_building.Facade == null)
				_building.Facade = CreatePart<Building.Part>("Facade", 0, SpriteDrawMode.Tiled);

			if (_building.Roof == null)
				_building.Roof = CreatePart<Building.Part>("Roof", 1, SpriteDrawMode.Tiled);

			_doorsControl.Setup(_building.Doors)
				.MakeDrawable(DrawDoor)
				.MakeAddable(_addDoorIcon, AddDoor)
				.MakeRemovable(_removeDoorIcon, RemoveDoor)
				.MakeCustomHeight(DoorHeight);

			_accessoriesControl.Setup(_building.Accessories)
				.MakeDrawable(DrawAccessory)
				.MakeAddable(_addAccessoryIcon, AddAccessory)
				.MakeRemovable(_removeAccessoryIcon, RemoveAccessory)
				.MakeCustomHeight(AccessoryHeight);
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_building.gameObject, false))
			{
				DrawAdvancedEditing();

				_building.CollisionLayer = (CollisionLayer)EnumButtonsDrawer.Draw(_collisionLayerContent.Content, (int)_building.CollisionLayer, false, _collisionLayerValues, _collisionLayerNames, 40.0f);

				var bounds = EditorGUILayout.RectField(_boundsContent.Content, _building.Bounds);
				var sortPoint = EditorGUILayout.FloatField(_sortPointContent.Content, _building.SortPoint);
				var roofHeight = EditorGUILayout.IntSlider(_roofHeightContent.Content, _building.RoofHeight, 1, Mathf.RoundToInt(_building.Bounds.height));

				DrawMainPart(_building.Roof, _roofContent.Content);
				DrawMainPart(_building.Facade, _facadeContent.Content);

				_doorsControl.Draw(_doorsContent.Content);
				_accessoriesControl.Draw(_accessoriesContent.Content);

				bounds.width = Mathf.Max(bounds.width, _minimumSize.x);
				bounds.height = Mathf.Max(bounds.height, _minimumSize.y);

				UpdateMainTransforms(bounds, sortPoint, roofHeight, false);
			}
		}

		private void DrawAdvancedEditing()
		{
			var value = EditorGUILayout.Toggle(_advancedEditingContent, _advancedEditingPreference.Value);
			_building.Facade.GameObject.hideFlags = value ? HideFlags.None : HideFlags.NotEditable;
			_building.Roof.GameObject.hideFlags = value ? HideFlags.None : HideFlags.NotEditable;

			foreach (var accessory in _building.Accessories)
				accessory.GameObject.hideFlags = value ? HideFlags.None : HideFlags.NotEditable;

			if (value)
				EditorGUILayout.HelpBox("Manual editing of building objects may not behave as expected", MessageType.Info);

			_advancedEditingPreference.Value = value;
		}

		private void DrawMainPart(Building.Part part, GUIContent label)
		{
			EditorGUILayout.LabelField(label);

			using (new EditorGUI.IndentLevelScope())
			{
				part.Renderer.sprite = EditorGUILayout.ObjectField(_spriteContent, part.Renderer.sprite, typeof(Sprite), false, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)) as Sprite;
				part.OrderOffset = part.Renderer.sortingOrder = EditorGUILayout.IntSlider(_orderOffsetContent, part.Renderer.sortingOrder, 0, _building.Accessories.Count + 2);
			}
		}

		private int _MaxOrderOffset => _building.Accessories.Count + _building.Doors.Count + 2;

		private float DoorHeight(int index)
		{
			return 8 * RectHelper.LineHeight;
		}

		private void DrawDoor(Rect rect, IList list, int index)
		{
			var door = _building.Doors[index];

			var nameRect =		RectHelper.TakeLine(ref rect);
								RectHelper.TakeWidth(ref rect, RectHelper.LeftMargin);
			var positionRect =	RectHelper.TakeLine(ref rect);
			var orderRect =		RectHelper.TakeLine(ref rect);
			var spriteRect =	RectHelper.TakeLine(ref rect);
			var openARect =		RectHelper.TakeLine(ref rect);
			var closeARect =	RectHelper.TakeLine(ref rect);
			var openSRect =		RectHelper.TakeLine(ref rect);
			var closeSRect =	RectHelper.TakeLine(ref rect);

			door.GameObject.name = EditorGUI.TextField(nameRect, door.GameObject.name);
			var position = EditorGUI.Vector2Field(positionRect, _accessoryPositionContent, door.Bounds.position);

			door.OrderOffset = door.Renderer.sortingOrder = EditorGUI.IntSlider(orderRect, _orderOffsetContent, door.Renderer.sortingOrder, 0, _MaxOrderOffset);
			door.Renderer.sprite = EditorGUI.ObjectField(spriteRect, _spriteContent, door.Renderer.sprite, typeof(Sprite), false) as Sprite;
			door.Door.OpenAnimation = EditorGUI.ObjectField(openARect, _openAnimationContent, door.Door.OpenAnimation, typeof(AnimationClip), false) as AnimationClip;
			door.Door.CloseAnimation = EditorGUI.ObjectField(closeARect, _closeAnimationContent, door.Door.CloseAnimation, typeof(AnimationClip), false) as AnimationClip;
			door.Door.OpenSound = EditorGUI.ObjectField(openSRect, _openSoundContent, door.Door.OpenSound, typeof(AudioClip), false) as AudioClip;
			door.Door.CloseSound = EditorGUI.ObjectField(closeSRect, _closeSoundContent, door.Door.CloseSound, typeof(AudioClip), false) as AudioClip;

			UpdatePartTransform(door, position);
		}

		private void AddDoor(IList list)
		{
			var door = CreatePart<Building.DoorPart>("Door" + _building.Doors.Count, _MaxOrderOffset, SpriteDrawMode.Simple);
			door.Animator = door.GameObject.AddComponent<Animator>();
			door.Audio = door.GameObject.AddComponent<AudioSource>();
			door.Door = door.GameObject.AddComponent<Door>();

			Undo.RegisterCreatedObjectUndo(door.GameObject, "Create door");
			_building.Doors.Add(door);
		}

		private void RemoveDoor(IList list, int index)
		{
			var accessory = _building.Accessories[index];
			Undo.DestroyObjectImmediate(accessory.GameObject);
			_building.Accessories.RemoveAt(index);
		}

		private float AccessoryHeight(int index)
		{
			return 5 * RectHelper.LineHeight;
		}

		private void DrawAccessory(Rect rect, IList list, int index)
		{
			var accessory = _building.Accessories[index];

			var nameRect =		RectHelper.TakeLine(ref rect);
								RectHelper.TakeWidth(ref rect, RectHelper.LeftMargin);
			var orderRect =		RectHelper.TakeLine(ref rect);
			var positionRect =	RectHelper.TakeLine(ref rect);
			var spriteRect =	RectHelper.TakeLine(ref rect);

			var animationRect =			RectHelper.TakeLine(ref rect);
			var animationLabelRect =	RectHelper.TakeLabel(ref animationRect);
			var animationToggleRect =	RectHelper.TakeLeadingIcon(ref animationRect);

			accessory.GameObject.name = EditorGUI.TextField(nameRect, accessory.GameObject.name);
			var position = EditorGUI.Vector2Field(positionRect, _accessoryPositionContent, accessory.Bounds.position);

			accessory.OrderOffset = accessory.Renderer.sortingOrder = EditorGUI.IntSlider(orderRect, _orderOffsetContent, accessory.Renderer.sortingOrder, 0, _MaxOrderOffset);
			accessory.Renderer.sprite = EditorGUI.ObjectField(spriteRect, _spriteContent, accessory.Renderer.sprite, typeof(Sprite), false) as Sprite;

			EditorGUI.LabelField(animationLabelRect, _animationsContent);

			var hasAnimations = accessory.Animation != null;
			var selectedHasAnimations = EditorGUI.Toggle(animationToggleRect, hasAnimations);

			if (selectedHasAnimations && !hasAnimations) AddAnimations(accessory);
			else if (!selectedHasAnimations && hasAnimations) RemoveAnimations(accessory);

			if (selectedHasAnimations)
				accessory.Animation.Animation = EditorGUI.ObjectField(animationRect, accessory.Animation.Animation, typeof(AnimationClip), false) as AnimationClip;

			UpdatePartTransform(accessory, position);
		}

		private void AddAccessory(IList list)
		{
			var accessory = CreatePart<Building.Accessory>("Accessory" + _building.Accessories.Count, _MaxOrderOffset, SpriteDrawMode.Simple);
			Undo.RegisterCreatedObjectUndo(accessory.GameObject, "Create accessory");
			_building.Accessories.Add(accessory);
		}

		private void RemoveAccessory(IList list, int index)
		{
			var accessory = _building.Accessories[index];
			Undo.DestroyObjectImmediate(accessory.GameObject);
			_building.Accessories.RemoveAt(index);
		}

		private void AddAnimations(Building.Accessory part)
		{
			part.Audio = part.GameObject.AddComponent<AudioSource>();
			part.Animator = part.GameObject.AddComponent<Animator>();
			part.Animation = part.GameObject.AddComponent<SimpleAnimationPlayer>();
		}

		private void RemoveAnimations(Building.Accessory part)
		{
			DestroyImmediate(part.Animation);
			DestroyImmediate(part.Animator);
			DestroyImmediate(part.Audio);
			part.Animation = null;
			part.Animator = null;
			part.Audio = null;
		}

		private void OnSceneGUI()
		{
			using (new UndoScope(_building.gameObject, false))
			{
				DrawBounds();

				foreach (var accessory in _building.Accessories)
					DrawPart(accessory);

				foreach (var door in _building.Doors)
					DrawPart(door);
			}
		}

		private void DrawBounds()
		{
			var bounds = _building.Bounds;
			var roofPosition = new Vector2(bounds.center.x, bounds.yMax - _building.RoofHeight);
			var size = HandleUtility.GetHandleSize(bounds.center) * 0.1f;

			HandleHelper.DrawLine(new Vector2(bounds.xMin, roofPosition.y), new Vector2(bounds.xMax, roofPosition.y), Color.cyan);

			var selectedBounds = HandleHelper.BoundsHandle(bounds, new Vector2(0.5f, 0.5f), Color.blue, Color.clear, Color.white, Color.white);
			var selectedRoofPosition = HandleHelper.MoveHandle(roofPosition, new Vector2(size * 2.5f, size), Vector2.one, Color.cyan, Color.cyan);

			UpdateMainTransforms(selectedBounds, _building.SortPoint, bounds.yMax - selectedRoofPosition.y, true);
		}

		private void DrawPart(Building.Part part)
		{
			if (part.Renderer.sprite)
			{
				var snap = 1 / part.Renderer.sprite.pixelsPerUnit;
				var selectedBounds = HandleHelper.MoveHandle(part.Renderer.bounds.center, part.Bounds.size, new Vector2(snap, snap), Color.green, new Color(0.0f, 0.0f, 0.0f, 0.25f));
				selectedBounds -= (Vector2)part.Renderer.bounds.extents;

				UpdatePartTransform(part, selectedBounds);
			}
		}

		private void UpdateMainTransforms(Rect bounds, float sortPoint, float roofHeight, bool clamp)
		{
			bounds = SnapBounds(bounds, 0.5f);

			if (clamp)
				bounds = ClampBounds(_building.Bounds, bounds);

			sortPoint = Mathf.Max(sortPoint, 0.0f);
			roofHeight = Mathf.Clamp(MathHelper.Snap(roofHeight, 1.0f), 1.0f, bounds.height);

			_building.Bounds = bounds;
			_building.SortPoint = sortPoint;
			_building.RoofHeight = Mathf.RoundToInt(roofHeight);
			_building.transform.localScale = Vector3.one;
			_building.transform.position = new Vector3(bounds.center.x, bounds.position.y + sortPoint);

			if (_building.Roof.Renderer.sprite)
			{
				var size = new Vector2(bounds.width, roofHeight);
				var pivot = _building.Roof.Renderer.sprite.pivot / _building.Roof.Renderer.sprite.rect.size * size;

				_building.Roof.Bounds = new Rect(bounds.x, bounds.yMax - _building.RoofHeight, size.x, size.y);
				_building.Roof.Renderer.size = _building.Roof.Bounds.size;
				_building.Roof.Renderer.transform.localScale = Vector2.one;
				_building.Roof.Renderer.transform.position = _building.Roof.Bounds.position + pivot;
			}

			if (_building.Facade.Renderer.sprite)
			{
				var size = new Vector2(bounds.width, bounds.height - (roofHeight * 0.5f));
				var pivot = _building.Facade.Renderer.sprite.pivot / _building.Facade.Renderer.sprite.rect.size * size;

				_building.Facade.Bounds = new Rect(bounds.position, size);
				_building.Facade.Renderer.size = _building.Facade.Bounds.size;
				_building.Facade.Renderer.transform.localScale = Vector2.one;
				_building.Facade.Renderer.transform.position = _building.Facade.Bounds.position + pivot;
			}
		}

		private void UpdatePartTransform(Building.Part part, Vector2 position)
		{
			if (part.Renderer.sprite)
			{
				var snap = 1 / part.Renderer.sprite.pixelsPerUnit;
				var pivot = part.Renderer.sprite.pivot / part.Renderer.sprite.rect.size * part.Renderer.size;

				position.x = MathHelper.Snap(position.x, snap);
				position.y = MathHelper.Snap(position.y, snap);

				var clamped = position + pivot;
				clamped.x = Mathf.Clamp(clamped.x, _building.Bounds.xMin, _building.Bounds.xMax);
				clamped.y = Mathf.Clamp(clamped.y, _building.Bounds.yMin, _building.Bounds.yMax);

				part.Bounds = new Rect(position, part.Renderer.size);
				part.Renderer.transform.localScale = Vector2.one;
				part.Renderer.transform.position = clamped;
			}
		}

		private T CreatePart<T>(string name, int offset, SpriteDrawMode mode) where T : Building.Part, new()
		{
			var gameObject = new GameObject(name);
			gameObject.hideFlags = _advancedEditingPreference.Value ? HideFlags.None : HideFlags.NotEditable;
			gameObject.transform.parent = _building.transform;
			gameObject.transform.position = _building.transform.position;
			gameObject.transform.localScale = Vector2.one;

			var renderer = gameObject.AddComponent<SpriteRenderer>();
			renderer.drawMode = mode;
			renderer.sortingOrder = offset;
			renderer.spriteSortPoint = SpriteSortPoint.Pivot;

			return new T
			{
				GameObject = gameObject,
				OrderOffset = offset,
				Renderer = renderer,
				Bounds = new Rect(_building.transform.position, Vector2.one)
			};
		}

		private Rect SnapBounds(Rect bounds, float snap)
		{
			bounds.x = MathHelper.Snap(bounds.x, snap);
			bounds.y = MathHelper.Snap(bounds.y, snap);
			bounds.width = MathHelper.Snap(bounds.width, snap);
			bounds.height = MathHelper.Snap(bounds.height, snap);

			return bounds;
		}

		private Rect ClampBounds(Rect bounds, Rect selectedBounds)
		{
			var leftPosition = new Vector3(bounds.xMin, bounds.center.y);
			var rightPosition = new Vector3(bounds.xMax, bounds.center.y);
			var topPosition = new Vector3(bounds.center.x, bounds.yMax);
			var bottomPosition = new Vector3(bounds.center.x, bounds.yMin);
			
			selectedBounds.xMin = Mathf.Min(selectedBounds.xMin, rightPosition.x - _minimumSize.x);
			selectedBounds.xMax = Mathf.Max(selectedBounds.xMax, leftPosition.x + _minimumSize.x);
			selectedBounds.yMin = Mathf.Min(selectedBounds.yMin, topPosition.y - _minimumSize.y);
			selectedBounds.yMax = Mathf.Max(selectedBounds.yMax, bottomPosition.y + _minimumSize.y);

			return selectedBounds;
		}
	}
}
