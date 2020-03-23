using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterMaker
{
	[CustomGridBrush(true, false, false, "Map Properties Brush")]
	public class MapPropertiesBrush : GridBrushBase
	{
		public class SelectionData
		{
			public bool HasDifferentCollisionLayer = false;
			public bool HasDifferentCollisionIncrement = false;
			public bool HasDifferentHasSpawnPoint = false;
			public bool HasDifferentSpawnPointNames = false;
			public bool HasDifferentSpawnPointDirections = false;
			public bool HasDifferentSpawnPointLayers = false;
			public bool HasDifferentHasZoneTrigger = false;
			public bool HasDifferentTargetZones = false;
			public bool HasDifferentHasZoneTransitions = false;
			public bool HasDifferentZoneTransitions = false;
			public bool HasDifferentZoneTargetSpwans = false;
			public bool HasDifferentHasEncounter = false;
			public bool HasDifferentEncounters = false;
			public bool HasDifferentHasInstructions = false;
			public bool HasDifferentInstructions = false;
			public bool HasEmptySelected = false;
			public List<TileInfo> SelectedTiles = new List<TileInfo>();
			public GridLayout Grid;
			public GameObject Target;
			public BoundsInt Bounds;

			public void Reset(GridLayout grid, GameObject target, BoundsInt bounds)
			{
				HasDifferentCollisionLayer = false;
				HasDifferentCollisionIncrement = false;
				HasDifferentHasSpawnPoint = false;
				HasDifferentSpawnPointNames = false;
				HasDifferentSpawnPointDirections = false;
				HasDifferentSpawnPointLayers = false;
				HasDifferentHasZoneTrigger = false;
				HasDifferentTargetZones = false;
				HasDifferentHasZoneTransitions = false;
				HasDifferentZoneTransitions = false;
				HasDifferentZoneTargetSpwans = false;
				HasDifferentHasEncounter = false;
				HasDifferentEncounters = false;
				HasDifferentHasInstructions = false;
				HasDifferentInstructions = false;
				HasEmptySelected = false;
				SelectedTiles.Clear();
				Grid = grid;
				Target = target;
				Bounds = bounds;
			}
		}

		[NonSerialized] public TileInfo ActiveTileInfo = new TileInfo();
		[NonSerialized] public SelectionData Selection = new SelectionData();

		public CollisionTile CollisionTile;

		private void OnEnable()
		{
			if (!CollisionTile)
			{
				CollisionTile = AssetFinder.GetMainAsset<CollisionTile>();
				if (!CollisionTile)
				{
					CollisionTile = CreateInstance<CollisionTile>();
					CollisionTile.name = "CollisionTile";
					CollisionTile.hideFlags = HideFlags.HideInHierarchy;

					AssetDatabase.CreateAsset(CollisionTile, "Assets/CollisionTile.asset");
					AssetDatabase.SaveAssets();
				}
			}
		}

		public override void Select(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
		{
			var translate = new Vector3Int(Mathf.FloorToInt(brushTarget.transform.position.x), Mathf.FloorToInt(brushTarget.transform.position.y), Mathf.FloorToInt(brushTarget.transform.position.z));
			bounds.SetMinMax(bounds.min + translate, bounds.max + translate);

			Selection.Reset(grid, brushTarget, bounds);
			
			var properties = AddOrGetProperties(grid);
			foreach (var position in Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile(new Vector2Int(position.x, position.y));
				if (tile != null)
					Selection.SelectedTiles.Add(tile);
				else
					Selection.HasEmptySelected = true;
			}

			ActiveTileInfo = Selection.SelectedTiles.Count == 0 ? new TileInfo() : Selection.SelectedTiles[0];

			foreach (var tile in Selection.SelectedTiles)
			{
				if (ActiveTileInfo.CollisionLayer != tile.CollisionLayer || (ActiveTileInfo.CollisionLayer != CollisionLayer.None && Selection.HasEmptySelected))
					Selection.HasDifferentCollisionLayer = true;
				
				if (ActiveTileInfo.CollisionLayerIncrement != tile.CollisionLayerIncrement || (ActiveTileInfo.CollisionLayerIncrement != CollisionLayer.None && Selection.HasEmptySelected))
					Selection.HasDifferentCollisionIncrement = true;

				if ((!ActiveTileInfo.HasSpawnPoint && tile.HasSpawnPoint) || (ActiveTileInfo.HasSpawnPoint && Selection.HasEmptySelected))
					Selection.HasDifferentHasSpawnPoint = true;

				if ((!ActiveTileInfo.HasZoneTrigger && tile.HasZoneTrigger) || (ActiveTileInfo.HasZoneTrigger && Selection.HasEmptySelected))
					Selection.HasDifferentHasZoneTrigger = true;
				
				if ((!ActiveTileInfo.HasEncounter && tile.HasEncounter) || (ActiveTileInfo.HasEncounter && Selection.HasEmptySelected))
					Selection.HasDifferentHasEncounter = true;
				
				if ((!ActiveTileInfo.HasInstructions && tile.HasInstructions) || (ActiveTileInfo.HasInstructions && Selection.HasEmptySelected))
					Selection.HasDifferentHasInstructions = true;
				
			}

			if (ActiveTileInfo.HasSpawnPoint && !Selection.HasDifferentHasSpawnPoint)
			{
				foreach (var tile in Selection.SelectedTiles)
				{
					if (ActiveTileInfo.SpawnPoint.Name != tile.SpawnPoint.Name)
						Selection.HasDifferentSpawnPointNames = true;

					if (ActiveTileInfo.SpawnPoint.Direction != tile.SpawnPoint.Direction)
						Selection.HasDifferentSpawnPointDirections = true;
					
					if (ActiveTileInfo.SpawnPoint.Layer != tile.SpawnPoint.Layer)
						Selection.HasDifferentSpawnPointLayers = true;
				}
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
							Selection.HasDifferentZoneTargetSpwans = true;
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
		}

		public MapProperties AddOrGetProperties(GridLayout grid)
		{
			var properties = grid.GetComponent<MapProperties>();
			if (!properties)
				properties = grid.gameObject.AddComponent<MapProperties>();

			return properties;
		}

		public void RefreshSelection()
		{
			Select(Selection.Grid, Selection.Target, Selection.Bounds);
		}
	}

	[CustomEditor(typeof(MapPropertiesBrush))]
	public class MapPropertiesBrushEditor : GridBrushEditorBase
	{
		private List<string> _collisionLayerOptions = new List<string>();

		private SubassetPicker<World, Zone> _zonePicker = new SubassetPicker<World, Zone>();
		private AssetPicker<Transition> _transitionPicker = new AssetPicker<Transition>();

		private GUIContent _collisionIncrementContent = new GUIContent("Collision Increment", "The collision layer to set a mover to when they enter this tile.");
		private GUIContent _collisionLayerContent = new GUIContent("Collision Layer", "The collision layer(s) that this tile is a part of. Movers on these layers will not be able to enter this tile.");
		private GUIContent _spawnPointContent = new GUIContent("Spawn Point", "Whether this tile has a spawn point or not.");
		private GUIContent _spawnNameContent = new GUIContent("", "The name of this spawn point to be referenced by a zone trigger.");
		private GUIContent _spawnDirectionContent = new GUIContent("Direction", "The direction the mover will face when spawning at this spawn point.");
		private GUIContent _spawnLayerContent = new GUIContent("Layer", "The collision layer to set the mover to when they spawn at this spawn point.");
		private GUIContent _zoneTriggerContent = new GUIContent("Zone Trigger", "Whether this tile has a zone trigger or not.");
		private GUIContent _transitionContent = new GUIContent("Transition", "Whether this zone trigger starts a transition or not.");
		private GUIContent _targetSpwanContent = new GUIContent("Target Spwan", "The spawn point to start at in the target zone.");
		private GUIContent _encounterContent = new GUIContent("Encounter Trigger", "Whether this tile can trigger an encounter or not.");
		private GUIContent _targetEncounterContent = new GUIContent("", "The game object with the encounter to initiate when this tile is enterred.");
		private GUIContent _instructionsContent = new GUIContent("Instructions", "Whether this tile should run instructions when enterred.");
		private GUIContent _targetInstructionsContent = new GUIContent("", "The instructions to run when this tile is enterred.");

		void OnEnable()
		{
			_zonePicker.Setup(new GUIContent("", "The zone to enter when this tile is stepped on"), true);
			_transitionPicker.Setup(new GUIContent("", "The transition to use for this zone trigger."), true);

			_collisionLayerOptions.Clear();

			for (int i = -1, value = 0; i < CollisionLayerData.LayerCount; i++, value = MathHelper.IntExponent(2, i))
				_collisionLayerOptions.Add(((CollisionLayer)value).ToString());

			_collisionLayerOptions.Add("different values");

			Undo.undoRedoPerformed += UndoRedo;
		}

		void OnDisable()
		{
			Undo.undoRedoPerformed -= UndoRedo;
		}

		void UndoRedo()
		{
			Repaint();
			(target as MapPropertiesBrush).RefreshSelection();
		}

		public override void OnPaintInspectorGUI()
		{
			// Empty so no inspector is shown.
		}

		public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
		{
			// Don't allow editing of palettes.
			if (brushTarget.layer == 31)
				return;

			Handles.DrawSolidRectangleWithOutline(new Rect(position.x + brushTarget.transform.position.x, position.y + brushTarget.transform.position.y, position.size.x, position.size.y), Color.clear, Color.white);
		}

		public override void OnSelectionInspectorGUI()
		{
			var brush = target as MapPropertiesBrush;
			var properties = brush.AddOrGetProperties(GridSelection.grid);

			using (new UndoScope(properties))
			{
				DrawCollisionLayer(brush, properties);

				EditorHelper.Separator(Color.grey);

				DrawCollisionIncrement(brush, properties);

				EditorHelper.Separator(Color.gray);

				DrawSpawnPoint(brush, properties);

				EditorHelper.Separator(Color.grey);

				DrawZone(brush, properties);

				EditorHelper.Separator(Color.grey);

				DrawEncounter(brush, properties);

				EditorHelper.Separator(Color.gray);

				DrawInstructions(brush, properties);
			}
		}

		private void DrawCollisionLayer(MapPropertiesBrush brush, MapProperties properties)
		{
			// COLLISION LAYER
			var collisionLayer = brush.ActiveTileInfo.CollisionLayer;
			var collisionLayerIndex = _collisionLayerOptions.Count - 1;
			var selectedCollisionLayer = collisionLayer;
			var selectedCollisionLayerIndex = collisionLayerIndex;

			if (brush.Selection.HasDifferentCollisionLayer)
				selectedCollisionLayerIndex = EditorGUILayout.Popup(_collisionLayerContent, collisionLayerIndex, _collisionLayerOptions.ToArray());
			else
				selectedCollisionLayer = (CollisionLayer)EditorGUILayout.EnumFlagsField(_collisionLayerContent, collisionLayer);
			
			if (collisionLayer != selectedCollisionLayer) ChangeCollisionLayers(brush, properties, selectedCollisionLayer);
			if (collisionLayerIndex != selectedCollisionLayerIndex && selectedCollisionLayerIndex < _collisionLayerOptions.Count - 1) ChangeCollisionLayers(brush, properties, selectedCollisionLayerIndex);
		}

		private void DrawCollisionIncrement(MapPropertiesBrush brush, MapProperties properties)
		{
			// COLLISION INCREMENT
			var collisionIncrementOptions = brush.Selection.HasDifferentCollisionIncrement ? _collisionLayerOptions.ToArray() : _collisionLayerOptions.GetRange(0, _collisionLayerOptions.Count - 1).ToArray();
			var collisionIncrement = brush.Selection.HasDifferentCollisionIncrement ? _collisionLayerOptions.Count - 1 : brush.ActiveTileInfo.CollisionLayerIncrement == CollisionLayer.None ? 0 : MathHelper.LogBase2((int)brush.ActiveTileInfo.CollisionLayerIncrement) + 1;
			var selectedCollisionIncrement = EditorGUILayout.Popup(_collisionIncrementContent, collisionIncrement, collisionIncrementOptions);
			
			if (collisionIncrement != selectedCollisionIncrement) SetCollisionIncrements(brush, properties, selectedCollisionIncrement);
		}

		private void DrawSpawnPoint(MapPropertiesBrush brush, MapProperties properties)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				// HAS SPAWN
				_spawnPointContent.text = brush.Selection.HasDifferentHasSpawnPoint ? "Spawn Point (different values)" : "Spawn Point";

				var hasSpawnPoint = brush.Selection.HasDifferentHasSpawnPoint ? false : brush.ActiveTileInfo.HasSpawnPoint;
				var selectedHasSpawnPoint = EditorGUILayout.Toggle(_spawnPointContent, hasSpawnPoint);
				
				if (!hasSpawnPoint && selectedHasSpawnPoint) AddSpawnPoints(brush, properties);
				if (hasSpawnPoint && !selectedHasSpawnPoint) RemoveSpawnPoints(brush, properties);

				if (brush.ActiveTileInfo.HasSpawnPoint && !brush.Selection.HasDifferentHasSpawnPoint)
				{
					// NAME
					var name = brush.Selection.HasDifferentSpawnPointNames ? "different values" : brush.ActiveTileInfo.SpawnPoint.Name;
					var selectedName = EditorGUILayout.DelayedTextField(_spawnNameContent, name);

					if (name != selectedName) SetSpawnPointNames(brush, properties, selectedName);
				}
			}

			if (brush.ActiveTileInfo.HasSpawnPoint && !brush.Selection.HasDifferentHasSpawnPoint)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					// DIRECTION
					_spawnDirectionContent.text = brush.Selection.HasDifferentSpawnPointDirections ? "Direction (different values)" : "Direction";

					var direction = brush.Selection.HasDifferentSpawnPointDirections ? Vector2.down : brush.ActiveTileInfo.SpawnPoint.Direction;
					var selectedDirection = EditorGUILayout.Vector2Field(_spawnDirectionContent, direction);

					// LAYER
					var layerOptions = brush.Selection.HasDifferentSpawnPointLayers ? _collisionLayerOptions.GetRange(1, _collisionLayerOptions.Count - 1).ToArray() : _collisionLayerOptions.GetRange(1, _collisionLayerOptions.Count - 2).ToArray();
					var layer = brush.Selection.HasDifferentSpawnPointLayers ? _collisionLayerOptions.Count - 1 : MathHelper.LogBase2((int)brush.ActiveTileInfo.SpawnPoint.Layer);
					var selectedLayer = EditorGUILayout.Popup(_spawnLayerContent, layer, layerOptions);
					
					if (direction != selectedDirection) SetSpawnPointDirections(brush, properties, selectedDirection);
					if (layer != selectedLayer) SetSpawnPointLayers(brush, properties, selectedLayer);
				}
			}
		}

		private void DrawZone(MapPropertiesBrush brush, MapProperties properties)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				// HAS ZONE
				_zoneTriggerContent.text = brush.Selection.HasDifferentHasZoneTrigger ? "Zone Trigger (different values)" : "Zone Trigger";

				var hasZone = brush.Selection.HasDifferentHasZoneTrigger ? false : brush.ActiveTileInfo.HasZoneTrigger;
				var selectedHasZone = EditorGUILayout.Toggle(_zoneTriggerContent, hasZone);

				if (!hasZone && selectedHasZone) AddZoneTriggers(brush, properties);
				if (hasZone && !selectedHasZone) RemoveZoneTriggers(brush, properties);

				if (brush.ActiveTileInfo.HasZoneTrigger && !brush.Selection.HasDifferentHasZoneTrigger)
				{
					// TARGET ZONE
					if (_zonePicker.DrawDropDownList(ref brush.ActiveTileInfo.Zone.TargetZone, brush.Selection.HasDifferentTargetZones))
						SetTargetZones(brush, properties);

					if (brush.ActiveTileInfo.Zone.TargetZone != null && !brush.Selection.HasDifferentTargetZones)
					{
						GUILayout.FlexibleSpace();

						if (GUILayout.Button("Load"))
						{
							var path = SceneUtility.GetScenePathByBuildIndex(brush.ActiveTileInfo.Zone.TargetZone.Scene.Index);
							EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
						}
					}
				}
			}

			if (brush.ActiveTileInfo.HasZoneTrigger && !brush.Selection.HasDifferentHasZoneTrigger)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						// HAS TRANSITION
						_transitionContent.text = brush.Selection.HasDifferentHasZoneTransitions ? "Transition (different values)" : "Transition";

						var hasTransition = brush.Selection.HasDifferentHasZoneTransitions ? false : brush.ActiveTileInfo.Zone.HasTransition;
						var selectedHasTransition = EditorGUILayout.Toggle(_transitionContent, hasTransition);

						
						if (!hasTransition && selectedHasTransition) AddZoneTransitions(brush, properties);
						if (hasTransition && !selectedHasTransition) RemoveZoneTransitions(brush, properties);

						if (brush.ActiveTileInfo.Zone.HasTransition && !brush.Selection.HasDifferentHasZoneTransitions)
						{
							// TRANSITION TYPE
							if (_transitionPicker.DrawDropDownList(ref brush.ActiveTileInfo.Zone.Transition, brush.Selection.HasDifferentZoneTransitions))
								SetZoneTransitions(brush, properties);

							GUILayout.FlexibleSpace();
						}
					}

					if (brush.ActiveTileInfo.Zone.HasTransition && !brush.Selection.HasDifferentHasZoneTransitions)
					{
						using (new EditorGUI.IndentLevelScope())
						{
							// TARGET SPAWN
							var spawn = brush.Selection.HasDifferentZoneTargetSpwans ? "different values" : brush.ActiveTileInfo.Zone.TargetSpawn;
							var selectedSpawn = EditorGUILayout.DelayedTextField(_targetSpwanContent, spawn);

							if (spawn != selectedSpawn) SetTargetSpawns(brush, properties, selectedSpawn);
						}
					}
				}
			}
		}

		private void DrawEncounter(MapPropertiesBrush brush, MapProperties properties)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				// HAS ENCOUNTER
				_encounterContent.text = brush.Selection.HasDifferentHasEncounter ? "Encounter (different values)" : "Encounter";

				var hasEncounter = brush.Selection.HasDifferentHasEncounter ? false : brush.ActiveTileInfo.HasEncounter;
				var selectedHasEncounter = EditorGUILayout.Toggle(_encounterContent, hasEncounter);

				if (brush.ActiveTileInfo.HasEncounter && !brush.Selection.HasDifferentHasEncounter)
				{
					// ENCOUNTER OBJECT
					_targetEncounterContent.text = brush.Selection.HasDifferentEncounters ? "(different values)" : "";

					var encounter = brush.Selection.HasDifferentEncounters ? null : brush.ActiveTileInfo.Encounter;
					var selectedEncounter = EditorGUILayout.ObjectField(_targetEncounterContent, encounter, typeof(Encounter), true) as Encounter;

					if (encounter != selectedEncounter) SetTargetEncounters(brush, properties, selectedEncounter);
				}
				
				if (!hasEncounter && selectedHasEncounter) AddEncounterTriggers(brush, properties);
				if (hasEncounter && !selectedHasEncounter) RemoveEncounterTriggers(brush, properties);
			}
		}

		private void DrawInstructions(MapPropertiesBrush brush, MapProperties properties)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				// HAS INSTRUCTIONS
				_instructionsContent.text = brush.Selection.HasDifferentHasInstructions ? "Instructions (different values)" : "Instructions";

				var hasInstruction = brush.Selection.HasDifferentHasInstructions ? false : brush.ActiveTileInfo.HasInstructions;
				var selectedHasInstruction = EditorGUILayout.Toggle(_instructionsContent, hasInstruction);

				if (brush.ActiveTileInfo.HasInstructions && !brush.Selection.HasDifferentHasInstructions)
				{
					// INSTRUCTION OBJECT
					_targetInstructionsContent.text = brush.Selection.HasDifferentInstructions ? "(different values)" : "";

					var instruction = brush.Selection.HasDifferentInstructions ? null : brush.ActiveTileInfo.Instructions;
					var selectedInstruction = InstructionDrawer.Draw(null, instruction, _targetInstructionsContent);

					if (instruction != selectedInstruction) SetInstructions(brush, properties, selectedInstruction);
				}
				
				if (!hasInstruction && selectedHasInstruction) AddInstructions(brush, properties);
				if (hasInstruction && !selectedHasInstruction) RemoveInstructions(brush, properties);
			}
		}

		private void ChangeCollisionLayers(MapPropertiesBrush brush, MapProperties properties, int index)
		{
			var layer = index == 0 ? CollisionLayer.None : (CollisionLayer)MathHelper.IntExponent(2, index - 1);
			ChangeCollisionLayers(brush, properties, layer);
		}

		private void ChangeCollisionLayers(MapPropertiesBrush brush, MapProperties properties, CollisionLayer layer)
		{
			brush.Selection.HasDifferentCollisionLayer = false;
			brush.ActiveTileInfo.CollisionLayer = layer;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				if (layer == CollisionLayer.None)
				{
					var tile = properties.GetTile(new Vector2Int(position.x, position.y));
					if (tile != null)
						tile.CollisionLayer = CollisionLayer.None;
				}
				else
				{
					var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
					tile.CollisionLayer = layer;
				}
			}

			var size = brush.Selection.Bounds.size.x * brush.Selection.Bounds.size.y;
			var tileArray = Enumerable.Repeat(brush.CollisionTile, size).ToArray();
			var nullArray = new TileBase[size];

			for (int index = 0, value = 1; index < CollisionLayerData.LayerCount; index++, value *= 2)
			{
				if ((value & (int)layer) > 0)
				{
					var tilemap = properties.AddOrGetCollisionLayer(index);
					tilemap.SetTilesBlock(brush.Selection.Bounds, tileArray);
				}
				else
				{
					var tilemap = properties.GetCollisionLayer(index);
					if (tilemap)
					{
						tilemap.SetTilesBlock(brush.Selection.Bounds, nullArray);
						properties.CheckForCollisionRemoval(tilemap, index);
					}
				}
			}
			
			properties.UpdateBounds();
		}

		private void SetCollisionIncrements(MapPropertiesBrush brush, MapProperties properties, int index)
		{
			var layer = index == 0 ? CollisionLayer.None : (CollisionLayer)MathHelper.IntExponent(2, index - 1);

			brush.Selection.HasDifferentCollisionIncrement = false;
			brush.ActiveTileInfo.CollisionLayerIncrement = layer;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				if (layer == CollisionLayer.None)
				{
					var tile = properties.GetTile(new Vector2Int(position.x, position.y));
					tile.CollisionLayerIncrement = CollisionLayer.None;
				}
				else
				{
					var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
					tile.CollisionLayerIncrement = layer;
				}
			}

			properties.UpdateBounds();
		}

		private void AddSpawnPoints(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasSpawnPoint = false;
			if (!brush.ActiveTileInfo.HasSpawnPoint)
			{
				brush.ActiveTileInfo.HasSpawnPoint = true;
				brush.ActiveTileInfo.SpawnPoint = new SpawnPoint()
				{
					Name = brush.ActiveTileInfo.SpawnPoint.Name,
					Direction = brush.ActiveTileInfo.SpawnPoint.Direction,
					Position = brush.ActiveTileInfo.Position,
					Layer = CollisionLayer.One
				};
			}

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				if (!tile.HasSpawnPoint)
				{
					tile.HasSpawnPoint = true;
					tile.SpawnPoint = new SpawnPoint()
					{
						Name = brush.ActiveTileInfo.SpawnPoint.Name,
						Direction = brush.ActiveTileInfo.SpawnPoint.Direction,
						Position = tile.Position,
						Layer = CollisionLayer.One
					};
				}
			}
			
			properties.UpdateBounds();
		}

		private void RemoveSpawnPoints(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasSpawnPoint = false;
			brush.ActiveTileInfo.HasSpawnPoint = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile(new Vector2Int(position.x, position.y));
				if (tile != null)
					tile.HasSpawnPoint = false;
			}
			
			properties.UpdateBounds();
		}

		private void SetSpawnPointNames(MapPropertiesBrush brush, MapProperties properties, string name)
		{
			brush.Selection.HasDifferentSpawnPointNames = false;
			brush.ActiveTileInfo.SpawnPoint.Name = name;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.SpawnPoint.Name = name;
			}
			
			properties.UpdateBounds();
		}

		private void SetSpawnPointDirections(MapPropertiesBrush brush, MapProperties properties, Vector2 direction)
		{
			brush.Selection.HasDifferentSpawnPointDirections = false;
			brush.ActiveTileInfo.SpawnPoint.Direction = direction;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.SpawnPoint.Direction = direction;
			}
			
			properties.UpdateBounds();
		}
		
		private void SetSpawnPointLayers(MapPropertiesBrush brush, MapProperties properties, int index)
		{
			var layer = (CollisionLayer)MathHelper.IntExponent(2, index);

			brush.Selection.HasDifferentSpawnPointLayers = false;
			brush.ActiveTileInfo.SpawnPoint.Layer = layer;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.SpawnPoint.Layer = layer;
			}

			properties.UpdateBounds();
		}

		private void AddZoneTriggers(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasZoneTrigger = false;
			if (!brush.ActiveTileInfo.HasZoneTrigger)
			{
				brush.ActiveTileInfo.HasZoneTrigger = true;
				brush.ActiveTileInfo.HasZoneTrigger = true;
				brush.ActiveTileInfo.Zone = new ZoneTrigger()
				{
					TargetZone = brush.ActiveTileInfo.Zone.TargetZone,
					Transition = brush.ActiveTileInfo.Zone.Transition,
					TargetSpawn = brush.ActiveTileInfo.Zone.TargetSpawn
				};
			}

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				if (!tile.HasZoneTrigger)
				{
					tile.HasZoneTrigger = true;
					tile.Zone = new ZoneTrigger()
					{
						TargetZone = brush.ActiveTileInfo.Zone.TargetZone,
						Transition = brush.ActiveTileInfo.Zone.Transition,
						TargetSpawn = brush.ActiveTileInfo.Zone.TargetSpawn
					};
				}
			}
			
			properties.UpdateBounds();
		}

		private void RemoveZoneTriggers(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasZoneTrigger = false;
			brush.ActiveTileInfo.HasZoneTrigger = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile(new Vector2Int(position.x, position.y));
				if (tile != null)
					tile.HasZoneTrigger = false;
			}
			
			properties.UpdateBounds();
		}

		private void SetTargetZones(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentTargetZones = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.Zone.TargetZone = brush.ActiveTileInfo.Zone.TargetZone;
			}
			
			properties.UpdateBounds();
		}
		
		private void AddZoneTransitions(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasZoneTransitions = false;
			brush.ActiveTileInfo.Zone.HasTransition = true;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.Zone.HasTransition = true;
			}
			
			properties.UpdateBounds();
		}

		private void RemoveZoneTransitions(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasZoneTrigger = false;
			brush.ActiveTileInfo.Zone.HasTransition = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile(new Vector2Int(position.x, position.y));
				if (tile != null)
					tile.Zone.HasTransition = false;
			}
			
			properties.UpdateBounds();
		}

		private void SetZoneTransitions(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentZoneTransitions = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.Zone.Transition = brush.ActiveTileInfo.Zone.Transition;
			}
			
			properties.UpdateBounds();
		}

		private void SetTargetSpawns(MapPropertiesBrush brush, MapProperties properties, string spawn)
		{
			brush.Selection.HasDifferentZoneTargetSpwans = false;
			brush.ActiveTileInfo.Zone.TargetSpawn = spawn;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.Zone.TargetSpawn = spawn;
			}
			
			properties.UpdateBounds();
		}

		private void AddEncounterTriggers(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasEncounter = false;
			brush.ActiveTileInfo.HasEncounter = true;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				if (!tile.HasEncounter)
				{
					tile.HasEncounter = true;
					tile.Encounter = brush.ActiveTileInfo.Encounter;
				}
			}
			
			properties.UpdateBounds();
		}

		private void RemoveEncounterTriggers(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasEncounter = false;
			brush.ActiveTileInfo.HasEncounter = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile(new Vector2Int(position.x, position.y));
				if (tile != null)
					tile.HasEncounter = false;
			}
			
			properties.UpdateBounds();

		}

		private void SetTargetEncounters(MapPropertiesBrush brush, MapProperties properties, Encounter encounter)
		{
			brush.Selection.HasDifferentEncounters = false;
			brush.ActiveTileInfo.Encounter = encounter;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.Encounter = encounter;
			}
			
			properties.UpdateBounds();
		}
		
		private void AddInstructions(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasInstructions = false;
			brush.ActiveTileInfo.HasInstructions = true;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				if (!tile.HasInstructions)
				{
					tile.HasInstructions = true;
					tile.Instructions = brush.ActiveTileInfo.Instructions;
				}
			}
			
			properties.UpdateBounds();
		}

		private void RemoveInstructions(MapPropertiesBrush brush, MapProperties properties)
		{
			brush.Selection.HasDifferentHasInstructions = false;
			brush.ActiveTileInfo.HasInstructions = false;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.GetTile(new Vector2Int(position.x, position.y));
				if (tile != null)
					tile.HasInstructions = false;
			}
			
			properties.UpdateBounds();
		}

		private void SetInstructions(MapPropertiesBrush brush, MapProperties properties, Instruction instructions)
		{
			brush.Selection.HasDifferentInstructions = false;
			brush.ActiveTileInfo.Instructions = instructions;

			foreach (var position in brush.Selection.Bounds.allPositionsWithin)
			{
				var tile = properties.AddOrGetTile(new Vector2Int(position.x, position.y));
				tile.Instructions = instructions;
			}
			
			properties.UpdateBounds();
		}
	}
}
