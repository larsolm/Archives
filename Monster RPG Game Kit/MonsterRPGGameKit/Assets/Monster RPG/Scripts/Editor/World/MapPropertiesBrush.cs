using PiRhoSoft.CompositionEditor;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomGridBrush(true, false, false, "Map Properties Brush")]
	public class MapPropertiesBrush : GridBrushBase
	{
		#region Selection Data

		public class SelectionData
		{
			public bool HasDifferentCollisionLayer = false;
			public bool HasDifferentCollisionIncrement = false;
			public bool HasDifferentHasSpawnPoint = false;
			public bool HasDifferentHasZoneTrigger = false;
			public bool HasDifferentTargetZones = false;
			public bool HasDifferentHasZoneTransitions = false;
			public bool HasDifferentZoneTransitions = false;
			public bool HasDifferentZoneTargetSpawns = false;
			public bool HasDifferentHasEncounter = false;
			public bool HasDifferentEncounters = false;
			public bool HasDifferentHasInstructions = false;
			public bool HasDifferentInstructions = false;
			public bool HasDifferentHasStairs = false;
			public bool HasDifferentSlopes = false;
			public bool HasDifferentHasOffset = false;
			public bool HasDifferentOffsets = false;
			public bool HasDifferentHasEdge = false;
			public bool HasDifferentEdgeDirections = false;
			public bool HasMultipleSelected = false;
			public bool HasEmptySelected = false;

			public List<TileInfo> SelectedTiles = new List<TileInfo>();
			public GridLayout Grid;
			public GameObject Target;
			public BoundsInt Bounds;

			public InstructionCallerControl InteractionInstructionControl = new InstructionCallerControl();
			public InstructionCallerControl EnteringInstructionControl = new InstructionCallerControl();
			public InstructionCallerControl EnterInstructionControl = new InstructionCallerControl();
			public InstructionCallerControl ExitingInstructionControl = new InstructionCallerControl();
			public InstructionCallerControl ExitInstructionControl = new InstructionCallerControl();

			public void Reset(GridLayout grid, GameObject target, BoundsInt bounds)
			{
				HasDifferentCollisionLayer = false;
				HasDifferentCollisionIncrement = false;
				HasDifferentHasSpawnPoint = false;
				HasDifferentHasZoneTrigger = false;
				HasDifferentTargetZones = false;
				HasDifferentHasZoneTransitions = false;
				HasDifferentZoneTransitions = false;
				HasDifferentZoneTargetSpawns = false;
				HasDifferentHasEncounter = false;
				HasDifferentEncounters = false;
				HasDifferentHasInstructions = false;
				HasDifferentInstructions = false;
				HasDifferentHasStairs = false;
				HasDifferentSlopes = false;
				HasDifferentHasOffset = false;
				HasDifferentOffsets = false;
				HasDifferentHasEdge = false;
				HasDifferentEdgeDirections = false;
				HasEmptySelected = false;
				HasMultipleSelected = bounds.size.x * bounds.size.y > 1;
				SelectedTiles.Clear();
				Grid = grid;
				Target = target;
				Bounds = bounds;
			}
		}

		[NonSerialized] public TileInfo ActiveTileInfo = new TileInfo();
		[NonSerialized] public SelectionData Selection = new SelectionData();

		#endregion

		public MapProperties AddOrGetProperties(GridLayout grid)
		{
			var properties = grid.GetComponent<MapProperties>();
			if (!properties)
				properties = grid.gameObject.AddComponent<MapProperties>();

			return properties;
		}

		#region Selection

		public override void Select(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
		{
			if (!brushTarget || !grid)
				return;

			var translate = new Vector3Int(Mathf.FloorToInt(brushTarget.transform.position.x), Mathf.FloorToInt(brushTarget.transform.position.y), Mathf.FloorToInt(brushTarget.transform.position.z));
			bounds.SetMinMax(bounds.min + translate, bounds.max + translate);

			Selection.Reset(grid, brushTarget, bounds);

			var properties = AddOrGetProperties(grid);
			foreach (var position in Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
					Selection.SelectedTiles.Add(tile);
				else
					Selection.HasEmptySelected = true;
			}

			ActiveTileInfo = Selection.SelectedTiles.Count == 0 ? new TileInfo() : Selection.SelectedTiles[0];

			foreach (var tile in Selection.SelectedTiles)
			{
				CheckIsDifferent(ref Selection.HasDifferentCollisionLayer, ActiveTileInfo.CollisionLayer, tile.CollisionLayer);
				CheckIsDifferent(ref Selection.HasDifferentCollisionIncrement, ActiveTileInfo.LayerChange, tile.LayerChange);
				CheckIsDifferent(ref Selection.HasDifferentHasSpawnPoint, ActiveTileInfo.HasSpawnPoint, tile.HasSpawnPoint);
				CheckIsDifferent(ref Selection.HasDifferentHasZoneTrigger, ActiveTileInfo.HasZoneTrigger, tile.HasZoneTrigger);
				CheckIsDifferent(ref Selection.HasDifferentHasEncounter, ActiveTileInfo.HasEncounter, tile.HasEncounter);
				CheckIsDifferent(ref Selection.HasDifferentHasInstructions, ActiveTileInfo.HasInstructions, tile.HasInstructions);
				CheckIsDifferent(ref Selection.HasDifferentHasStairs, ActiveTileInfo.HasStairs, tile.HasStairs);
				CheckIsDifferent(ref Selection.HasDifferentHasOffset, ActiveTileInfo.HasOffset, tile.HasOffset);
				CheckIsDifferent(ref Selection.HasDifferentHasEdge, ActiveTileInfo.HasEdge, tile.HasEdge);
			}

			if (ActiveTileInfo.HasZoneTrigger && !Selection.HasDifferentHasZoneTrigger)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.Zone.TargetZone != tile.Zone.TargetZone)
						Selection.HasDifferentTargetZones = true;

					if (ActiveTileInfo.Zone.HasTransition != tile.Zone.HasTransition || (ActiveTileInfo.Zone.HasTransition && Selection.HasEmptySelected))
						Selection.HasDifferentHasZoneTransitions = true;
				}

				if (ActiveTileInfo.Zone.HasTransition && !Selection.HasDifferentHasZoneTransitions)
				{
					foreach (var tile in Selection.SelectedTiles)
					{
						if (ActiveTileInfo.Zone.Transition != tile.Zone.Transition)
							Selection.HasDifferentZoneTransitions = true;

						if (ActiveTileInfo.Zone.TargetSpawn != tile.Zone.TargetSpawn)
							Selection.HasDifferentZoneTargetSpawns = true;
					}
				}
			}

			if (ActiveTileInfo.HasEncounter && !Selection.HasDifferentHasEncounter)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.Encounter != tile.Encounter)
						Selection.HasDifferentEncounters = true;
				}
			}

			if (ActiveTileInfo.HasInstructions && !Selection.HasDifferentHasInstructions)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.Instructions != tile.Instructions)
						Selection.HasDifferentInstructions = true;
				}
			}

			if (ActiveTileInfo.HasStairs && !Selection.HasDifferentHasStairs)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.Slope != tile.Slope)
						Selection.HasDifferentSlopes = true;
				}
			}

			if (ActiveTileInfo.HasOffset && !Selection.HasDifferentHasOffset)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.Offset != tile.Offset)
						Selection.HasDifferentOffsets = true;
				}
			}

			if (ActiveTileInfo.HasEdge && !Selection.HasDifferentHasEdge)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.EdgeDirection != tile.EdgeDirection)
						Selection.HasDifferentEdgeDirections = true;
				}
			}

			RefreshControls();
		}

		public void RefreshSelection()
		{
			Select(Selection.Grid, Selection.Target, Selection.Bounds);
		}

		public void RefreshControls()
		{
			if (ActiveTileInfo.HasInstructions && !Selection.HasDifferentInstructions)
			{
				Selection.InteractionInstructionControl = new InstructionCallerControl();
				Selection.EnteringInstructionControl = new InstructionCallerControl();
				Selection.EnterInstructionControl = new InstructionCallerControl();
				Selection.ExitingInstructionControl = new InstructionCallerControl();
				Selection.ExitInstructionControl = new InstructionCallerControl();

				Selection.InteractionInstructionControl.Setup(ActiveTileInfo.Instructions.InteractionInstructions, null, null, null);
				Selection.EnteringInstructionControl.Setup(ActiveTileInfo.Instructions.EnteringInstructions, null, null, null);
				Selection.EnterInstructionControl.Setup(ActiveTileInfo.Instructions.EnterInstructions, null, null, null);
				Selection.ExitingInstructionControl.Setup(ActiveTileInfo.Instructions.ExitingInstructions, null, null, null);
				Selection.ExitInstructionControl.Setup(ActiveTileInfo.Instructions.ExitInstructions, null, null, null);
			}
		}

		private void CheckIsDifferent(ref bool checkValue, CollisionLayer activeCollisionLayer, CollisionLayer tileCollisionLayer)
		{
			checkValue = checkValue || (activeCollisionLayer != tileCollisionLayer) || (activeCollisionLayer != CollisionLayer.None && Selection.HasEmptySelected);
		}

		private void CheckIsDifferent(ref bool checkValue, bool activeTileValue, bool tileValue)
		{
			checkValue = checkValue || (!activeTileValue && tileValue) || (activeTileValue && Selection.HasEmptySelected);
		}

		#endregion
	}

	[CustomEditor(typeof(MapPropertiesBrush))]
	public class MapPropertiesBrushEditor : GridBrushEditorBase
	{
		#region Setup

		private static readonly IconButton _loadSceneButton = new IconButton(IconButton.Load, "Load this scene");
		private static readonly GUIContent _differentValuesContent = new GUIContent("(different values)");
		private static readonly GUIContent _collisionLayerContent = new GUIContent("Collision Layer", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.CollisionLayer)));
		private static readonly GUIContent _collisionLayerDifferentContent = new GUIContent("Collision Layer (different values)", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.CollisionLayer)));
		private static readonly GUIContent _layerChangeContent = new GUIContent("Layer Change", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.LayerChange)));
		private static readonly GUIContent _layerChangeDifferentContent = new GUIContent("Layer Change (different values)", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.LayerChange)));
		private static readonly GUIContent _spawnPointContent = new GUIContent("Spawn Point", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasSpawnPoint)));
		private static readonly GUIContent _spawnPointDifferentContent = new GUIContent("Spawn Point (different values)", _spawnPointContent.tooltip);
		private static readonly GUIContent _spawnNameContent = new GUIContent("", Label.GetTooltip(typeof(SpawnPoint), nameof(SpawnPoint.Name)));
		private static readonly GUIContent _spawnTransitionContent = new GUIContent("Transition", Label.GetTooltip(typeof(SpawnPoint), nameof(SpawnPoint.Transition)));
		private static readonly GUIContent _spawnDirectionContent = new GUIContent("Direction", Label.GetTooltip(typeof(SpawnPoint), nameof(SpawnPoint.Direction)));
		private static readonly GUIContent _spawnMoveContent = new GUIContent("Move", Label.GetTooltip(typeof(SpawnPoint), nameof(SpawnPoint.Move)));
		private static readonly GUIContent _spawnLayerContent = new GUIContent("Layer", Label.GetTooltip(typeof(SpawnPoint), nameof(SpawnPoint.Layer)));
		private static readonly GUIContent _zoneTriggerContent = new GUIContent("Zone Trigger", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasZoneTrigger)));
		private static readonly GUIContent _zoneTriggerDifferentContent = new GUIContent("Zone Trigger (different values)", _zoneTriggerContent.tooltip);
		private static readonly GUIContent _transitionContent = new GUIContent("Transition", Label.GetTooltip(typeof(ZoneTrigger), nameof(ZoneTrigger.HasTransition)));
		private static readonly GUIContent _transitionDifferentContent = new GUIContent("Transition (different values)", _transitionContent.tooltip);
		private static readonly GUIContent _targetSpawnContent = new GUIContent("Target Spawn", Label.GetTooltip(typeof(ZoneTrigger), nameof(ZoneTrigger.TargetSpawn)));
		private static readonly GUIContent _encounterContent = new GUIContent("Encounter", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasEncounter)));
		private static readonly GUIContent _encounterDifferentContent = new GUIContent("Encounter (different values)", _encounterContent.tooltip);
		private static readonly GUIContent _targetEncounterContent = new GUIContent("", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.Encounter)));
		private static readonly GUIContent _targetEncounterDifferentContent = new GUIContent("(different values)", _encounterContent.tooltip);
		private static readonly GUIContent _instructionsContent = new GUIContent("Instructions", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasInstructions)));
		private static readonly GUIContent _instructionsDifferentContent = new GUIContent("Instructions (different values)", _instructionsContent.tooltip);
		private static readonly GUIContent _interactionDirectionsContent = new GUIContent("Directions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.InteractionDirections)));
		private static readonly GUIContent _enteringDirectionsContent = new GUIContent("Directions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.EnteringDirections)));
		private static readonly GUIContent _enterDirectionsContent = new GUIContent("Directions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.EnterDirections)));
		private static readonly GUIContent _exitingDirectionsContent = new GUIContent("Directions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.ExitingDirections)));
		private static readonly GUIContent _exitDirectionsContent = new GUIContent("Directions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.ExitDirections)));
		private static readonly GUIContent _interactionInstructionsContent = new GUIContent("Instructions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.InteractionInstructions)));
		private static readonly GUIContent _enteringInstructionsContent = new GUIContent("Instructions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.EnteringInstructions)));
		private static readonly GUIContent _enterInstructionsContent = new GUIContent("Instructions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.EnterInstructions)));
		private static readonly GUIContent _exitingInstructionsContent = new GUIContent("Instructions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.ExitingInstructions)));
		private static readonly GUIContent _exitInstructionsContent = new GUIContent("Instructions", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.ExitInstructions)));
		private static readonly GUIContent _noneInteractionInstructionsContent = new GUIContent("Interaction", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.InteractionInstructions)));
		private static readonly GUIContent _noneEnteringInstructionsContent = new GUIContent("Entering", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.EnteringInstructions)));
		private static readonly GUIContent _noneEnterInstructionsContent = new GUIContent("Enter", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.EnterInstructions)));
		private static readonly GUIContent _noneExitingInstructionsContent = new GUIContent("Exiting", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.ExitingInstructions)));
		private static readonly GUIContent _noneExitInstructionsContent = new GUIContent("Exit", Label.GetTooltip(typeof(InstructionTrigger), nameof(InstructionTrigger.ExitInstructions)));
		private static readonly GUIContent _stairsContent = new GUIContent("Stairs", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasStairs)));
		private static readonly GUIContent _stairsDifferentContent = new GUIContent("Stairs (different values)", _stairsContent.tooltip);
		private static readonly GUIContent _slopeContent = new GUIContent("Slope", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.Slope)));
		private static readonly GUIContent _slopeDifferentContent = new GUIContent("Slope (different values)", _slopeContent.tooltip);
		private static readonly GUIContent _offsetContent = new GUIContent("Position Offset", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasOffset)));
		private static readonly GUIContent _offsetDifferentContent = new GUIContent("Position Offset (different values)", _offsetContent.tooltip);
		private static readonly GUIContent _offsetPositionContent = new GUIContent("", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.Offset)));
		private static readonly GUIContent _offsetPositionDifferentContent = new GUIContent("(different values)", _offsetPositionContent.tooltip);
		private static readonly GUIContent _edgeContent = new GUIContent("Edge", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.HasEdge)));
		private static readonly GUIContent _edgeDifferentContent = new GUIContent("Edge (different values)", _offsetContent.tooltip);
		private static readonly GUIContent _edgeDirectionContent = new GUIContent("", Label.GetTooltip(typeof(TileInfo), nameof(TileInfo.EdgeDirection)));
		private static readonly GUIContent _edgeDirectionDifferentContent = new GUIContent("(different values)", _edgeDirectionContent.tooltip);

		private static Array _collisionLayerValues = Enum.GetValues(typeof(CollisionLayer));
		private static GUIContent[] _collisionLayerNames = Enum.GetNames(typeof(CollisionLayer)).Select(name => new GUIContent(name)).ToArray();
		private static Array _layerChangeValues = Enum.GetValues(typeof(CollisionLayer)).Cast<int>().Take(LayerSorting.LayerCount + 1).ToArray();
		private static GUIContent[] _layerChangeNames = Enum.GetNames(typeof(CollisionLayer)).Take(LayerSorting.LayerCount + 1).Select(name => new GUIContent(name)).ToArray();
		private static Array _spawnLayerValues = Enum.GetValues(typeof(CollisionLayer)).Cast<int>().Skip(1).Take(LayerSorting.LayerCount).ToArray();
		private static GUIContent[] _spawnLayerNames = Enum.GetNames(typeof(CollisionLayer)).Skip(1).Take(LayerSorting.LayerCount).Select(name => new GUIContent(name)).ToArray();
		private static Array _spawnDirectionValues = Enum.GetValues(typeof(MovementDirection));
		private static GUIContent[] _spawnDirectionNames = Enum.GetNames(typeof(MovementDirection)).Select(name => new GUIContent(name)).ToArray();
		private static Array _interactionDirectionValues = Enum.GetValues(typeof(InteractionDirection));
		private static GUIContent[] _interactionDirectionNames = Enum.GetNames(typeof(InteractionDirection)).Select(name => new GUIContent(name)).ToArray();

		private static GUIContent[] _slopeOptions = new GUIContent[2] { new GUIContent("Up"), new GUIContent("Down") };
		private static int[] _slopeValues = new int[2] { 1, -1 };
		private static float _minimumButtonWidth = 40.0f;

		public override GameObject[] validTargets => FindObjectsOfType<Grid>().Select(o => o.gameObject).ToArray();

		private MapPropertiesBrush _brush;

		void OnEnable()
		{
			_brush = target as MapPropertiesBrush;

			Undo.undoRedoPerformed += UndoRedo;
		}

		void OnDisable()
		{
			Undo.undoRedoPerformed -= UndoRedo;
		}

		private void UndoRedo()
		{
			Repaint();
			_brush.RefreshSelection();
		}

		public override void OnInspectorGUI()
		{
			// Empty so nothing is drawn
		}

		public override void OnPaintInspectorGUI()
		{
			// Empty so nothing is drawn
		}

		public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
		{
			if (!brushTarget || brushTarget.layer == 31) // Don't allow palettes.
				return;

			Handles.DrawSolidRectangleWithOutline(new Rect(position.x + brushTarget.transform.position.x, position.y + brushTarget.transform.position.y, position.size.x, position.size.y), Color.clear, executing ? new Color(1f, 0.75f, 0.25f) : new Color(1.0f, 0.5f, 0.0f));
		}

		public override void OnSelectionInspectorGUI()
		{
			var properties = _brush.AddOrGetProperties(GridSelection.grid);

			using (new UndoScope(properties, false))
			{
				DrawCollisionLayer(properties); EditorGUILayout.Space();
				DrawLayerChange(properties); EditorGUILayout.Space();
				DrawSpawnPoint(properties); EditorGUILayout.Space();
				DrawZone(properties); EditorGUILayout.Space();
				DrawEncounter(properties); EditorGUILayout.Space();
				DrawInstructions(properties); EditorGUILayout.Space();
				DrawStairs(properties); EditorGUILayout.Space();
				DrawOffset(properties); EditorGUILayout.Space();
				DrawEdge(properties);
			}
		}

		#endregion

		#region Collision Layer

		private void DrawCollisionLayer(MapProperties properties)
		{
			// COLLISION LAYER
			var collisionLayerContent = _brush.Selection.HasDifferentCollisionLayer ? _collisionLayerDifferentContent : _collisionLayerContent;
			var position = EditorGUILayout.GetControlRect(false);
			var rect = EditorGUI.PrefixLabel(position, collisionLayerContent, EditorStyles.boldLabel);

			var collisionLayer = _brush.ActiveTileInfo.CollisionLayer;
			var selectedCollisionLayer = (CollisionLayer)EnumButtonsDrawer.Draw(rect, GUIContent.none, (int)collisionLayer, !_brush.Selection.HasDifferentCollisionLayer, _collisionLayerValues, _collisionLayerNames, _minimumButtonWidth);

			if (collisionLayer != selectedCollisionLayer) ChangeCollisionLayers(properties, selectedCollisionLayer);
		}

		private void ChangeCollisionLayers(MapProperties properties, CollisionLayer layer)
		{
			_brush.Selection.HasDifferentCollisionLayer = false;
			_brush.ActiveTileInfo.CollisionLayer = layer;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				if (layer == CollisionLayer.None)
				{
					var tile = properties.GetTile((Vector2Int)position);
					if (tile != null)
						tile.CollisionLayer = CollisionLayer.None;
				}
				else
				{
					var tile = properties.AddOrGetTile((Vector2Int)position);
					tile.CollisionLayer = layer;
				}
			}
		}

		#endregion

		#region Layer Change

		private void DrawLayerChange(MapProperties properties)
		{
			// LAYER CHANGE
			var layerChangeContent = _brush.Selection.HasDifferentCollisionIncrement ? _layerChangeDifferentContent : _layerChangeContent;
			var position = EditorGUILayout.GetControlRect(false);
			var rect = EditorGUI.PrefixLabel(position, layerChangeContent, EditorStyles.boldLabel);

			var layerChange = _brush.ActiveTileInfo.LayerChange;
			var selectedLayerChange = (CollisionLayer)EnumButtonsDrawer.Draw(rect, GUIContent.none, (int)layerChange, false, _layerChangeValues, _layerChangeNames, _minimumButtonWidth);

			if (layerChange != selectedLayerChange) SetLayerChange(properties, selectedLayerChange);
		}

		private void SetLayerChange(MapProperties properties, CollisionLayer layer)
		{
			_brush.Selection.HasDifferentCollisionIncrement = false;
			_brush.ActiveTileInfo.LayerChange = layer;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				if (layer == CollisionLayer.None)
				{
					var tile = properties.GetTile((Vector2Int)position);
					if (tile != null)
						tile.LayerChange = CollisionLayer.None;
				}
				else
				{
					var tile = properties.AddOrGetTile((Vector2Int)position);
					tile.LayerChange = layer;
				}
			}
		}

		#endregion

		#region Spawn Point

		private void DrawSpawnPoint(MapProperties properties)
		{
			var spawnRect = EditorGUILayout.GetControlRect(false);
			var spawnLabelRect = RectHelper.TakeLabel(ref spawnRect);
			var hasSpawnRect = RectHelper.TakeLeadingIcon(ref spawnRect);

			var spawnPointContent = _brush.Selection.HasDifferentHasSpawnPoint ? _spawnPointDifferentContent : _spawnPointContent;

			EditorGUI.LabelField(spawnLabelRect, _spawnPointContent, EditorStyles.boldLabel);

			// HAS SPAWN
			var hasSpawnPoint = _brush.Selection.HasDifferentHasSpawnPoint ? false : _brush.ActiveTileInfo.HasSpawnPoint;
			var selectedHasSpawnPoint = EditorGUI.Toggle(hasSpawnRect, hasSpawnPoint);

			if (!hasSpawnPoint && selectedHasSpawnPoint) AddSpawnPoints(properties);
			if (hasSpawnPoint && !selectedHasSpawnPoint) RemoveSpawnPoints(properties);

			if (_brush.ActiveTileInfo.HasSpawnPoint && !_brush.Selection.HasMultipleSelected)
			{
				// NAME
				var name = _brush.ActiveTileInfo.SpawnPoint.Name;
				var selectedName = EditorGUI.TextField(spawnRect, _spawnNameContent, name);

				if (name != selectedName) SetSpawnPointNames(properties, selectedName);

				using (new EditorGUI.IndentLevelScope())
				{
					// TRANSITION
					var transition = _brush.ActiveTileInfo.SpawnPoint.Transition;
					var selectedTransition = EditorGUILayout.ObjectField(_spawnTransitionContent, transition, typeof(Transition), false) as Transition;

					// DIRECTION
					var direction = _brush.ActiveTileInfo.SpawnPoint.Direction;
					var selectedDirection = (MovementDirection)EnumButtonsDrawer.Draw(_spawnDirectionContent, (int)direction, false, _spawnDirectionValues, _spawnDirectionNames, _minimumButtonWidth);

					// LAYER
					var layer = _brush.ActiveTileInfo.SpawnPoint.Layer;
					var selectedLayer = (CollisionLayer)EnumButtonsDrawer.Draw(_spawnLayerContent, (int)layer, false, _spawnLayerValues, _spawnLayerNames, _minimumButtonWidth);

					// MOVE
					var move = _brush.ActiveTileInfo.SpawnPoint.Move;
					var selectedMove = EditorGUILayout.Toggle(_spawnMoveContent, move);

					if (transition != selectedTransition) SetSpawnPointTransition(properties, selectedTransition);
					if (move != selectedMove) SetSpawnPointMove(properties, selectedMove);
					if (direction != selectedDirection) SetSpawnPointDirections(properties, selectedDirection);
					if (layer != selectedLayer) SetSpawnPointLayers(properties, selectedLayer);
				}
			}
		}

		private void AddSpawnPoints(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasSpawnPoint = false;
			_brush.ActiveTileInfo.HasSpawnPoint = true;
			_brush.ActiveTileInfo.SpawnPoint = SpawnPoint.Default;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.HasSpawnPoint = true;
				tile.SpawnPoint = SpawnPoint.Default;
				tile.SpawnPoint.Position = tile.Position;
			}
		}

		private void RemoveSpawnPoints(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasSpawnPoint = false;
			_brush.ActiveTileInfo.HasSpawnPoint = false;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
					tile.HasSpawnPoint = false;
			}
		}

		private void SetSpawnPointNames(MapProperties properties, string name)
		{
			_brush.ActiveTileInfo.SpawnPoint.Name = name;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.SpawnPoint.Name = name;
			}
		}

		private void SetSpawnPointTransition(MapProperties properties, Transition transition)
		{
			_brush.ActiveTileInfo.SpawnPoint.Transition = transition;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.SpawnPoint.Transition = transition;
			}
		}

		private void SetSpawnPointMove(MapProperties properties, bool move)
		{
			_brush.ActiveTileInfo.SpawnPoint.Move = move;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.SpawnPoint.Move = move;
			}
		}

		private void SetSpawnPointDirections(MapProperties properties, MovementDirection direction)
		{
			_brush.ActiveTileInfo.SpawnPoint.Direction = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.SpawnPoint.Direction = direction;
			}
		}

		private void SetSpawnPointLayers(MapProperties properties, CollisionLayer layer)
		{
			_brush.ActiveTileInfo.SpawnPoint.Layer = layer;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.SpawnPoint.Layer = layer;
			}
		}

		#endregion

		#region Zone Trigger

		private void DrawZone(MapProperties properties)
		{
			var zoneRect = EditorGUILayout.GetControlRect(false, RectHelper.LineHeight);
			var zoneLabelRect = RectHelper.TakeLabel(ref zoneRect);
			var hasZoneRect = RectHelper.TakeLeadingIcon(ref zoneRect);

			// HAS ZONE
			var zoneTriggerContent = _brush.Selection.HasDifferentHasZoneTrigger ? _zoneTriggerDifferentContent : _zoneTriggerContent;

			EditorGUI.LabelField(zoneLabelRect, zoneTriggerContent, EditorStyles.boldLabel);

			var hasZone = _brush.Selection.HasDifferentHasZoneTrigger ? false : _brush.ActiveTileInfo.HasZoneTrigger;
			var selectedHasZone = EditorGUI.Toggle(hasZoneRect, hasZone);

			if (!hasZone && selectedHasZone) AddZoneTriggers(properties);
			if (hasZone && !selectedHasZone) RemoveZoneTriggers(properties);

			if (_brush.ActiveTileInfo.HasZoneTrigger && !_brush.Selection.HasDifferentHasZoneTrigger)
			{
				var loadRect = _brush.ActiveTileInfo.Zone.TargetZone != null && !_brush.Selection.HasDifferentTargetZones ? RectHelper.TakeTrailingIcon(ref zoneRect) : zoneRect;

				// TARGET ZONE
				var zone = _brush.Selection.HasDifferentTargetZones ? null : _brush.ActiveTileInfo.Zone.TargetZone;
				var selectedZone = AssetPopupDrawer.Draw(zoneRect, GUIContent.none, zone, true, true, true);

				if (_brush.ActiveTileInfo.Zone.TargetZone != null && !_brush.Selection.HasDifferentTargetZones)
				{
					var targetZone = _brush.ActiveTileInfo.Zone.TargetZone;

					if (!targetZone.Scene.IsLoaded)
					{
						using (ColorScope.ContentColor(Color.black))
						{
							if (GUI.Button(loadRect, _loadSceneButton.Content, GUIStyle.none))
								EditorSceneManager.OpenScene(targetZone.Scene.Path, _brush.ActiveTileInfo.Zone.HasTransition ? OpenSceneMode.Single : OpenSceneMode.Additive);
						}
					}
				}

				if (_brush.ActiveTileInfo.Zone.TargetZone != selectedZone)
					SetTargetZones(properties, selectedZone);
			}

			if (_brush.ActiveTileInfo.HasZoneTrigger && !_brush.Selection.HasDifferentHasZoneTrigger)
			{
				var transitionRect = EditorGUILayout.GetControlRect(false, RectHelper.LineHeight);
				var transitionLabelRect = RectHelper.TakeLabel(ref transitionRect);
				RectHelper.TakeWidth(ref transitionLabelRect, RectHelper.GetIndentWidth(1));
				var hasTransitionRect = RectHelper.TakeLeadingIcon(ref transitionRect);

				// HAS TRANSITION
				var transitionContent = _brush.Selection.HasDifferentHasZoneTransitions ? _transitionDifferentContent : _transitionContent;

				EditorGUI.LabelField(transitionLabelRect, transitionContent);

				var hasTransition = _brush.Selection.HasDifferentHasZoneTransitions ? false : _brush.ActiveTileInfo.Zone.HasTransition;
				var selectedHasTransition = EditorGUI.Toggle(hasTransitionRect, hasTransition);

				if (!hasTransition && selectedHasTransition) AddZoneTransitions(properties);
				if (hasTransition && !selectedHasTransition) RemoveZoneTransitions(properties);

				if (_brush.ActiveTileInfo.Zone.HasTransition && !_brush.Selection.HasDifferentHasZoneTransitions)
				{
					// TRANSITION TYPE
					var transition = _brush.Selection.HasDifferentZoneTransitions ? null : _brush.ActiveTileInfo.Zone.Transition;
					var selectedTransition = AssetPopupDrawer.Draw(transitionRect, GUIContent.none, transition, true, true, true);

					if (_brush.ActiveTileInfo.Zone.Transition != selectedTransition)
						SetZoneTransitions(properties, selectedTransition);
				}

				if (_brush.ActiveTileInfo.Zone.HasTransition && !_brush.Selection.HasDifferentHasZoneTransitions)
				{
					using (new EditorGUI.IndentLevelScope())
					{
						// TARGET SPAWN
						var spawn = _brush.Selection.HasDifferentZoneTargetSpawns ? _differentValuesContent.text : _brush.ActiveTileInfo.Zone.TargetSpawn;
						var selectedSpawn = EditorGUILayout.TextField(_targetSpawnContent, spawn);

						if (spawn != selectedSpawn) SetTargetSpawns(properties, selectedSpawn);
					}
				}
			}
		}

		private void AddZoneTriggers(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasZoneTrigger = false;
			_brush.ActiveTileInfo.HasZoneTrigger = true;
			_brush.ActiveTileInfo.Zone = new ZoneTrigger();

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.HasZoneTrigger = true;
				tile.Zone = new ZoneTrigger();
			}
		}

		private void RemoveZoneTriggers(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasZoneTrigger = false;
			_brush.ActiveTileInfo.HasZoneTrigger = false;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
					tile.HasZoneTrigger = false;
			}
		}

		private void SetTargetZones(MapProperties properties, Zone zone)
		{
			_brush.Selection.HasDifferentTargetZones = false;
			_brush.ActiveTileInfo.Zone.TargetZone = zone;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Zone.TargetZone = zone;
			}
		}

		private void AddZoneTransitions(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasZoneTransitions = false;
			_brush.ActiveTileInfo.Zone.HasTransition = true;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Zone.HasTransition = true;
			}
		}

		private void RemoveZoneTransitions(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasZoneTrigger = false;
			_brush.ActiveTileInfo.Zone.HasTransition = false;
			_brush.ActiveTileInfo.Zone.Transition = null;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
				{
					tile.Zone.HasTransition = false;
					tile.Zone.Transition = null;
				}
			}
		}

		private void SetZoneTransitions(MapProperties properties, Transition transition)
		{
			_brush.Selection.HasDifferentZoneTransitions = false;
			_brush.ActiveTileInfo.Zone.Transition = transition;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Zone.Transition = transition;
			}
		}

		private void SetTargetSpawns(MapProperties properties, string spawn)
		{
			_brush.Selection.HasDifferentZoneTargetSpawns = false;
			_brush.ActiveTileInfo.Zone.TargetSpawn = spawn;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Zone.TargetSpawn = spawn;
			}
		}

		#endregion

		#region Encounter

		private void DrawEncounter(MapProperties properties)
		{
			var encounterRect = EditorGUILayout.GetControlRect(false, RectHelper.LineHeight);
			var encounterLabelRect = RectHelper.TakeLabel(ref encounterRect);
			var hasEncounterRect = RectHelper.TakeLeadingIcon(ref encounterRect);

			// HAS ENCOUNTER
			var encounterContent = _brush.Selection.HasDifferentHasEncounter ? _encounterDifferentContent : _encounterContent;

			EditorGUI.LabelField(encounterLabelRect, encounterContent, EditorStyles.boldLabel);

			var hasEncounter = _brush.Selection.HasDifferentHasEncounter ? false : _brush.ActiveTileInfo.HasEncounter;
			var selectedHasEncounter = EditorGUI.Toggle(hasEncounterRect, hasEncounter);

			if (_brush.ActiveTileInfo.HasEncounter && !_brush.Selection.HasDifferentHasEncounter)
			{
				// ENCOUNTER OBJECT
				var targetEncounterContent = _brush.Selection.HasDifferentEncounters ? _targetEncounterDifferentContent : _targetEncounterContent;
				var encounter = _brush.Selection.HasDifferentEncounters ? null : _brush.ActiveTileInfo.Encounter;
				var selectedEncounter = EditorGUI.ObjectField(encounterRect, targetEncounterContent, encounter, typeof(Encounter), true) as Encounter;

				if (encounter != selectedEncounter) SetTargetEncounters(properties, selectedEncounter);
			}

			if (!hasEncounter && selectedHasEncounter) AddEncounterTriggers(properties);
			if (hasEncounter && !selectedHasEncounter) RemoveEncounterTriggers(properties);
		}

		private void AddEncounterTriggers(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasEncounter = false;
			_brush.ActiveTileInfo.HasEncounter = true;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				if (!tile.HasEncounter)
				{
					tile.HasEncounter = true;
					tile.Encounter = _brush.ActiveTileInfo.Encounter;
				}
			}
		}

		private void RemoveEncounterTriggers(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasEncounter = false;
			_brush.ActiveTileInfo.HasEncounter = false;
			_brush.ActiveTileInfo.Encounter = null;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
				{
					tile.HasEncounter = false;
					tile.Encounter = null;
				}
			}
		}

		private void SetTargetEncounters(MapProperties properties, Encounter encounter)
		{
			_brush.Selection.HasDifferentEncounters = false;
			_brush.ActiveTileInfo.Encounter = encounter;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Encounter = encounter;
			}
		}

		#endregion

		#region Instructions

		private void DrawInstructions(MapProperties properties)
		{
			// HAS INSTRUCTIONS
			var instructionsContent = _brush.Selection.HasDifferentHasInstructions ? _instructionsDifferentContent : _instructionsContent;
			var position = EditorGUILayout.GetControlRect(false);
			var rect = EditorGUI.PrefixLabel(position, instructionsContent, EditorStyles.boldLabel);

			var hasInstruction = _brush.Selection.HasDifferentHasInstructions ? false : _brush.ActiveTileInfo.HasInstructions;
			var selectedHasInstruction = EditorGUI.Toggle(rect, hasInstruction);

			if (!hasInstruction && selectedHasInstruction) AddInstructions(properties);
			if (hasInstruction && !selectedHasInstruction) RemoveInstructions(properties);

			if (_brush.ActiveTileInfo.HasInstructions && !_brush.Selection.HasDifferentHasInstructions)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					// INSTRUCTIONS
					if (_brush.Selection.HasDifferentInstructions)
					{
						EditorGUILayout.LabelField(_instructionsDifferentContent);
					}
					else
					{
						// Interaction
						if (_brush.ActiveTileInfo.Instructions.InteractionInstructions.Instruction != null)
						{
							EditorGUILayout.LabelField("Interaction");

							using (new EditorGUI.IndentLevelScope())
							{
								var interactionDirection = _brush.ActiveTileInfo.Instructions.InteractionDirections;
								var selectedInteractionDirection = (InteractionDirection)EnumButtonsDrawer.Draw(_interactionDirectionsContent, (int)interactionDirection, true, _interactionDirectionValues, _interactionDirectionNames, _minimumButtonWidth);

								_brush.Selection.InteractionInstructionControl.Draw(_interactionInstructionsContent);

								if (interactionDirection != selectedInteractionDirection)
									SetInteractionDirections(properties, selectedInteractionDirection);
							}
						}
						else
						{
							_brush.Selection.InteractionInstructionControl.Draw(_noneInteractionInstructionsContent);
						}

						// Entering
						if (_brush.ActiveTileInfo.Instructions.EnteringInstructions.Instruction != null)
						{
							EditorGUILayout.LabelField("Entering");

							using (new EditorGUI.IndentLevelScope())
							{
								var enteringDirection = _brush.ActiveTileInfo.Instructions.EnteringDirections;
								var selectedEnteringDirection = (InteractionDirection)EnumButtonsDrawer.Draw(_enteringDirectionsContent, (int)enteringDirection, true, _interactionDirectionValues, _interactionDirectionNames, _minimumButtonWidth);

								_brush.Selection.EnteringInstructionControl.Draw(_enteringInstructionsContent);

								if (enteringDirection != selectedEnteringDirection)
									SetEnteringDirections(properties, selectedEnteringDirection);
							}
						}
						else
						{
							_brush.Selection.EnteringInstructionControl.Draw(_noneEnteringInstructionsContent);
						}

						// Enter
						if (_brush.ActiveTileInfo.Instructions.EnterInstructions.Instruction != null)
						{
							EditorGUILayout.LabelField("Enter");

							using (new EditorGUI.IndentLevelScope())
							{
								var enterDirection = _brush.ActiveTileInfo.Instructions.EnterDirections;
								var selectedEnterDirection = (InteractionDirection)EnumButtonsDrawer.Draw(_enterDirectionsContent, (int)enterDirection, true, _interactionDirectionValues, _interactionDirectionNames, _minimumButtonWidth);

								_brush.Selection.EnterInstructionControl.Draw(_enterInstructionsContent);

								if (enterDirection != selectedEnterDirection)
									SetEnterDirections(properties, selectedEnterDirection);
							}
						}
						else
						{
							_brush.Selection.EnterInstructionControl.Draw(_noneEnterInstructionsContent);
						}

						// Exiting
						if (_brush.ActiveTileInfo.Instructions.ExitingInstructions.Instruction != null)
						{
							EditorGUILayout.LabelField("Exiting");

							using (new EditorGUI.IndentLevelScope())
							{
								var exitingDirection = _brush.ActiveTileInfo.Instructions.ExitingDirections;
								var selectedExitingDirection = (InteractionDirection)EnumButtonsDrawer.Draw(_exitingDirectionsContent, (int)exitingDirection, true, _interactionDirectionValues, _interactionDirectionNames, _minimumButtonWidth);

								_brush.Selection.ExitingInstructionControl.Draw(_exitingInstructionsContent);

								if (exitingDirection != selectedExitingDirection)
									SetExitingDirections(properties, selectedExitingDirection);
							}
						}
						else
						{
							_brush.Selection.ExitingInstructionControl.Draw(_noneExitingInstructionsContent);
						}

						// Exit
						if (_brush.ActiveTileInfo.Instructions.ExitInstructions.Instruction != null)
						{
							EditorGUILayout.LabelField("Exit");

							using (new EditorGUI.IndentLevelScope())
							{
								var exitDirection = _brush.ActiveTileInfo.Instructions.ExitDirections;
								var selectedExitDirection = (InteractionDirection)EnumButtonsDrawer.Draw(_exitDirectionsContent, (int)exitDirection, true, _interactionDirectionValues, _interactionDirectionNames, _minimumButtonWidth);

								_brush.Selection.ExitInstructionControl.Draw(_exitInstructionsContent);

								if (exitDirection != selectedExitDirection)
									SetExitDirections(properties, selectedExitDirection);
							}
						}
						else
						{
							_brush.Selection.ExitInstructionControl.Draw(_noneExitInstructionsContent);
						}
					}
				}
			}
		}

		private void AddInstructions(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasInstructions = false;
			_brush.ActiveTileInfo.HasInstructions = true;
			_brush.ActiveTileInfo.Instructions = new InstructionTrigger();

			_brush.RefreshControls();

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				if (!tile.HasInstructions)
				{
					tile.HasInstructions = true;
					tile.Instructions = _brush.ActiveTileInfo.Instructions;
				}
			}

			_brush.RefreshSelection();
		}

		private void RemoveInstructions(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasInstructions = false;
			_brush.ActiveTileInfo.HasInstructions = false;
			_brush.ActiveTileInfo.Instructions = null;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
				{
					tile.HasInstructions = false;
					tile.Instructions = null;
				}
			}

			_brush.RefreshSelection();
		}

		private void SetInteractionDirections(MapProperties properties, InteractionDirection direction)
		{
			_brush.ActiveTileInfo.Instructions.InteractionDirections = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Instructions.InteractionDirections = direction;
			}
		}

		private void SetEnteringDirections(MapProperties properties, InteractionDirection direction)
		{
			_brush.ActiveTileInfo.Instructions.EnteringDirections = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Instructions.EnteringDirections = direction;
			}
		}
		private void SetEnterDirections(MapProperties properties, InteractionDirection direction)
		{
			_brush.ActiveTileInfo.Instructions.EnterDirections = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Instructions.EnterDirections = direction;
			}
		}
		private void SetExitingDirections(MapProperties properties, InteractionDirection direction)
		{
			_brush.ActiveTileInfo.Instructions.ExitingDirections = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Instructions.ExitingDirections = direction;
			}
		}

		private void SetExitDirections(MapProperties properties, InteractionDirection direction)
		{
			_brush.ActiveTileInfo.Instructions.ExitDirections = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Instructions.ExitDirections = direction;
			}
		}

		#endregion

		#region Stairs

		private void DrawStairs(MapProperties properties)
		{
			// HAS STAIRS
			var stairsContent = _brush.Selection.HasDifferentHasStairs ? _stairsDifferentContent : _stairsContent;
			var position = EditorGUILayout.GetControlRect(false);
			var rect = EditorGUI.PrefixLabel(position, stairsContent, EditorStyles.boldLabel);

			var hasStairs = _brush.Selection.HasDifferentHasStairs ? false : _brush.ActiveTileInfo.HasStairs;
			var selectedHasStairs = EditorGUI.Toggle(rect, hasStairs);

			if (!hasStairs && selectedHasStairs) AddStairs(properties);
			if (hasStairs && !selectedHasStairs) RemoveStairs(properties);

			if (_brush.ActiveTileInfo.HasStairs && !_brush.Selection.HasDifferentHasStairs)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					// SLOPE
					var slopeContent = _brush.Selection.HasDifferentSlopes ? _slopeDifferentContent : _slopeContent;
					var slope = _brush.Selection.HasDifferentSlopes ? 0 : _brush.ActiveTileInfo.Slope;
					var selectedSlope = EditorGUILayout.IntPopup(slopeContent, slope, _slopeOptions, _slopeValues);

					if (slope != selectedSlope) SetSlopes(properties, selectedSlope);
				}
			}
		}

		private void AddStairs(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasStairs = false;
			_brush.ActiveTileInfo.HasStairs = true;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				if (!tile.HasStairs)
				{
					tile.HasStairs = true;
					tile.Slope = _brush.ActiveTileInfo.Slope;
				}
			}
		}

		private void RemoveStairs(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasStairs = false;
			_brush.ActiveTileInfo.HasStairs = false;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
					tile.HasStairs = false;
			}
		}

		private void SetSlopes(MapProperties properties, int slope)
		{
			_brush.Selection.HasDifferentSlopes = false;
			_brush.ActiveTileInfo.Slope = slope;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Slope = slope;
			}
		}

		#endregion

		#region Edge

		private void DrawEdge(MapProperties properties)
		{
			// HAS EDGE
			var edgeContent = _brush.Selection.HasDifferentHasEdge ? _edgeDifferentContent : _edgeContent;
			var position = EditorGUILayout.GetControlRect(false);
			var rect = EditorGUI.PrefixLabel(position, edgeContent, EditorStyles.boldLabel);

			var hasEdge = _brush.Selection.HasDifferentHasEdge ? false : _brush.ActiveTileInfo.HasEdge;
			var selectedHasEdge = EditorGUI.Toggle(rect, hasEdge);

			if (!hasEdge && selectedHasEdge) AddEdges(properties);
			if (hasEdge && !selectedHasEdge) RemoveEdges(properties);

			if (_brush.ActiveTileInfo.HasEdge && !_brush.Selection.HasDifferentHasEdge)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					// DIRECTION
					var directionContent = _brush.Selection.HasDifferentEdgeDirections ? _edgeDirectionDifferentContent : _edgeDirectionContent;
					var direction = _brush.Selection.HasDifferentEdgeDirections ? MovementDirection.None : _brush.ActiveTileInfo.EdgeDirection;
					var selectedDirection = (MovementDirection)EnumButtonsDrawer.Draw(directionContent, (int)direction, false, _spawnDirectionValues, _spawnDirectionNames, _minimumButtonWidth);

					if (direction != selectedDirection) SetDirections(properties, selectedDirection);
				}
			}
		}


		private void AddEdges(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasEdge = false;
			_brush.ActiveTileInfo.HasEdge = true;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				if (!tile.HasEdge)
				{
					tile.HasEdge = true;
					tile.EdgeDirection = _brush.ActiveTileInfo.EdgeDirection;
				}
			}
		}

		private void RemoveEdges(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasEdge = false;
			_brush.ActiveTileInfo.HasEdge = false;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
					tile.HasEdge = false;
			}
		}

		private void SetDirections(MapProperties properties, MovementDirection direction)
		{
			_brush.Selection.HasDifferentEdgeDirections = false;
			_brush.ActiveTileInfo.EdgeDirection = direction;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.EdgeDirection = direction;
			}
		}

		#endregion

		#region Offset

		private void DrawOffset(MapProperties properties)
		{
			// HAS OFFSET
			var offsetContent = _brush.Selection.HasDifferentHasOffset ? _offsetDifferentContent : _offsetContent;
			var position = EditorGUILayout.GetControlRect(false);
			var rect = EditorGUI.PrefixLabel(position, offsetContent, EditorStyles.boldLabel);

			var hasOffset = _brush.Selection.HasDifferentHasOffset ? false : _brush.ActiveTileInfo.HasOffset;
			var selectedHasOffset = EditorGUI.Toggle(rect, hasOffset);

			if (!hasOffset && selectedHasOffset) AddOffsets(properties);
			if (hasOffset && !selectedHasOffset) RemoveOffsets(properties);

			if (_brush.ActiveTileInfo.HasOffset && !_brush.Selection.HasDifferentHasOffset)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					// OFFSET
					var offsetPositionContent = _brush.Selection.HasDifferentOffsets ? _offsetPositionDifferentContent : _offsetPositionContent;
					var offset = _brush.Selection.HasDifferentOffsets ? Vector2.zero : _brush.ActiveTileInfo.Offset;
					var selectedOffset = EditorGUILayout.Vector2Field(offsetPositionContent, offset);

					if (offset != selectedOffset) SetOffsets(properties, selectedOffset);
				}
			}
		}


		private void AddOffsets(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasOffset = false;
			_brush.ActiveTileInfo.HasOffset = true;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				if (!tile.HasOffset)
				{
					tile.HasOffset = true;
					tile.Offset = _brush.ActiveTileInfo.Offset;
				}
			}
		}

		private void RemoveOffsets(MapProperties properties)
		{
			_brush.Selection.HasDifferentHasOffset = false;
			_brush.ActiveTileInfo.HasOffset = false;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile((Vector2Int)position);
				if (tile != null)
					tile.HasOffset = false;
			}
		}

		private void SetOffsets(MapProperties properties, Vector2 offset)
		{
			_brush.Selection.HasDifferentOffsets = false;
			_brush.ActiveTileInfo.Offset = offset;

			foreach (var position in _brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile((Vector2Int)position);
				tile.Offset = offset;
			}
		}

		#endregion
	}
}