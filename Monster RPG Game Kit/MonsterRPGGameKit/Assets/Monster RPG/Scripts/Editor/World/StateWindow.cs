using System;
using System.Collections.Generic;
using System.IO;
using PiRhoSoft.CompositionEditor;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using MenuItem = UnityEditor.MenuItem;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class StateWindow : EditorWindow
	{
		private static readonly IconButton _editCreatureButton = new IconButton(IconButton.Edit, "Edit this Creature");
		private static readonly TextButton _saveButton = new TextButton("Save", "Save the game");
		private static readonly TextButton _newButton = new TextButton("New", "Create a new save file");
		private static readonly TextButton _openButton = new TextButton("Open", "Open an existing save file");
		private static readonly TextButton _clearButton = new TextButton("Clear", "Stop using a save file");
		private static readonly GUIContent _activeZoneOptionContent = new GUIContent("Active Zone in Editor");
		private static readonly GUIContent _savedZoneOptionContent = new GUIContent("Saved Zone");
		private static readonly GUIContent _startingZoneContent = new GUIContent("Starting Zone");
		private static readonly GUIContent _spawnContent = new GUIContent("Starting Spawn");

		private StoreTreeView _tree = null;
		private TreeViewState _state = new TreeViewState();
		private List<StoreTreeView.StoreItem> _selections = new List<StoreTreeView.StoreItem>();

		private Vector2 _scrollPosition;
		private float _splitMinimum = 100.0f;
		private float _splitPosition = 200.0f;
		private float _splitWidth = 3.0f;
		private bool _dragging = false;

		private AssetList _zoneList;
		private GUIContent[] _zoneNames;
		
		[MenuItem("Window/PiRho Soft/State Manager")]
		public static void Open()
		{
			GetWindow<StateWindow>("State Manager").Show();
		}

		void OnInspectorUpdate()
		{
			if (EditorApplication.isPlaying && _tree == null)
			{
				if (Player.Instance != null && Player.Instance.Zone != null)
				{
					_tree = new StoreTreeView(this, _state);
					_tree.Reload();
					_tree.RestoreSelection();
				}
			}

			if (!EditorApplication.isPlaying && _tree != null)
			{
				_tree = null;
			}

			Repaint();
		}

		void OnGUI()
		{
			var playing = EditorApplication.isPlaying && _tree != null;
			var top = DrawHeader(playing);

			if (playing)
			{
				var offset = SceneLoader.ZoneTypePreference.Value != SceneLoader.LoadSavedZone ? 90 : 70; // not sure why but 'top' wasn't coming out right so hardcoded

				_tree.OnGUI(new Rect(0, top, _splitPosition - _splitWidth, position.height - top));
				var splitterRect = DrawSplitter(top);
				var storeRect = new Rect(_splitPosition + _splitWidth, offset, position.width - _splitPosition - _splitWidth, position.height - offset);

				using (new GUILayout.AreaScope(storeRect))
				{
					using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition))
					{
						foreach (var selection in _selections)
						{
							EditorGUILayout.Space();

							switch (selection)
							{
								case StoreTreeView.PlayerItem player: DrawTrainer(player.Player.Trainer); break;
								case StoreTreeView.NpcItem npc: DrawTrainer(npc.Npc.Trainer); break;
							}

							using (new LabelWidthScope(storeRect.width * 0.4f))
								VariableStoreControl.DrawTable(selection.Store, false);
						}

						_scrollPosition = scroll.scrollPosition;
					}
				}

				UpdateSplitter(splitterRect);
			}
		}

		private void DrawTrainer(Trainer trainer)
		{
			if (trainer && trainer.Roster != null && trainer.Roster.Creatures != null)
			{
				EditorGUILayout.LabelField("Roster");

				var rosterRect = EditorGUILayout.GetControlRect(false, RectHelper.LineHeight * trainer.Roster.Creatures.Count);
				RectHelper.TakeIndent(ref rosterRect);

				foreach (var creature in trainer.Roster.Creatures)
				{
					var rect = RectHelper.TakeLine(ref rosterRect);
					var icon = RectHelper.TakeLeadingIcon(ref rect);

					if (GUI.Button(icon, _editCreatureButton.Content, GUIStyle.none))
						Selection.activeObject = creature;

					GUI.Label(rect, creature.Name);
				}
			}
		}

		private float DrawHeader(bool playing)
		{
			var lineWidth = 2.0f;
			var lineColor = new Color(0.3f, 0.3f, 0.3f);

			EditorGUILayout.Space();
			DrawFilePicker(playing);
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			DrawZonePicker();
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			DrawSpawn();
			EditorGUILayout.Space();

			var top = GUILayoutUtility.GetLastRect().yMax;

			using (ColorScope.Color(lineColor))
				GUI.Box(new Rect(0, top, position.width, lineWidth), GUIContent.none);

			return top + lineWidth;
		}

		private void DrawFilePicker(bool playing)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PrefixLabel("Save File");

				if (playing)
				{
					EditorGUILayout.SelectableLabel(WorldManager.Instance.SaveFilename, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MinWidth(5));

					if (GUILayout.Button(_saveButton.Content, GUILayout.MinWidth(20)))
						WorldLoader.Save(WorldManager.Instance);
				}
				else
				{
					EditorGUILayout.SelectableLabel(SceneLoader.FilePreference.Value, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MinWidth(5));

					if (GUILayout.Button(_newButton.Content, GUILayout.MinWidth(20)))
					{
						var path = EditorUtility.SaveFilePanel("Create Save File", Application.persistentDataPath, "Editor Save", "mmk");

						if (!string.IsNullOrEmpty(path))
						{
							File.WriteAllText(path, "{}");
							SceneLoader.FilePreference.Value = path;

							if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
								SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
						}
					}

					if (GUILayout.Button(_openButton.Content, GUILayout.MinWidth(20)))
					{
						var path = EditorUtility.OpenFilePanel("Open Save File", Application.persistentDataPath, "mmk");

						if (!string.IsNullOrEmpty(path))
							SceneLoader.FilePreference.Value = path;
					}

					if (GUILayout.Button(_clearButton.Content, GUILayout.MinWidth(20)))
					{
						SceneLoader.FilePreference.Value = "";

						if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
							SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
					}
				}
			}
		}

		private void BuildZoneList()
		{
			var zoneList = AssetHelper.GetAssetList<Zone>(false, false);

			if (zoneList != _zoneList)
			{
				_zoneNames = new GUIContent[zoneList.Names.Length + 3];
				_zoneList = zoneList;

				Array.Copy(zoneList.Names, 0, _zoneNames, 3, zoneList.Names.Length);

				_zoneNames[0] = _activeZoneOptionContent;
				_zoneNames[1] = _savedZoneOptionContent;
				_zoneNames[2] = new GUIContent(string.Empty);
			}
		}

		private void DrawZonePicker()
		{
			BuildZoneList();

			var index = 0;

			if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadActiveZone)
			{
				index = 0;
			}
			else if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
			{
				index = 1;
			}
			else
			{
				for (var i = 0; i < _zoneList.Assets.Count; i++)
				{
					if ((_zoneList.Assets[i] as Zone).Scene.Path == SceneLoader.ZonePreference.Value)
					{
						index = i + 3;
						break;
					}
				}
			}

			index = EditorGUILayout.Popup(_startingZoneContent, index, _zoneNames);

			if (index == 0)
			{
				SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
			}
			else if (index == 1)
			{
				SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadSavedZone;
			}
			else
			{
				SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadSpecificZone;
				SceneLoader.ZonePreference.Value = (_zoneList.Assets[index - 3] as Zone).Scene.Path;
			}
		}

		private void DrawSpawn()
		{
			if (SceneLoader.ZoneTypePreference.Value != SceneLoader.LoadSavedZone)
			{
				var spawn = EditorGUILayout.TextField(_spawnContent, SceneLoader.SpawnPreference.Value);
				SceneLoader.SpawnPreference.Value = spawn;
			}
		}

		private Rect DrawSplitter(float top)
		{
			var rect = new Rect(_splitPosition - _splitWidth, top, _splitWidth * 2.0f, position.height - top);
			GUI.Box(rect, "");
			return rect;
		}

		private void UpdateSplitter(Rect rect)
		{
			if (Event.current != null)
			{
				switch (Event.current.rawType)
				{
					case EventType.MouseDown:
						if (rect.Contains(Event.current.mousePosition))
							_dragging = true;

						break;
					case EventType.MouseDrag:
						if (_dragging)
						{
							_splitPosition = Event.current.mousePosition.x;

							if (_splitPosition < _splitMinimum)
								_splitPosition = _splitMinimum;
							else if (_splitPosition > (position.width - _splitMinimum))
								_splitPosition = (position.width - _splitMinimum);

							Repaint();
						}

						break;
					case EventType.MouseUp:
						_dragging = false;
						break;
				}
			}

			if (_dragging)
				EditorGUIUtility.AddCursorRect(new Rect(0, 0, position.width, position.height), MouseCursor.SplitResizeLeftRight);
			else
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
		}

		private void ClearSelection()
		{
			_selections.Clear();
		}

		private void AddToSelection(StoreTreeView.StoreItem item)
		{
			_selections.Add(item);
		}

		private class StoreTreeView : TreeView
		{
			private StateWindow _window;
			private GUIStyle _loadedStyle;
			private GUIStyle _activeStyle;

			public class StoreItem : TreeViewItem
			{
				public IVariableStore Store;
			}

			public class WorldItem : StoreItem
			{
				public WorldManager World;
			}

			public class PlayerItem : StoreItem
			{
				public Player Player;
			}

			public class ZoneItem : StoreItem
			{
				public ZoneData Zone;
			}

			public class NpcItem : StoreItem
			{
				public Npc Npc;
			}

			public StoreTreeView(StateWindow window, TreeViewState treeViewState) : base(treeViewState)
			{
				_window = window;
			}

			public void RestoreSelection()
			{
				SelectionChanged(GetSelection());
			}

			protected override TreeViewItem BuildRoot()
			{
				_loadedStyle = EditorStyles.boldLabel;
				_activeStyle = new GUIStyle(EditorStyles.boldLabel);
				_activeStyle.fontStyle = FontStyle.BoldAndItalic;

				var id = 0;
				var rootItem = new TreeViewItem { id = id++, depth = -1, displayName = "Root" };

				var worldItem = new WorldItem { Store = WorldManager.Instance.Variables, World = WorldManager.Instance, id = id++, displayName = "World" };
				rootItem.AddChild(worldItem);

				var playerItem = new PlayerItem { Store = Player.Instance.Variables, Player = Player.Instance, id = id++, displayName = "Player" };
				rootItem.AddChild(playerItem);

				var zonesItem = new TreeViewItem { id = id++, displayName = "Zones" };
				rootItem.AddChild(zonesItem);

				foreach (var zone in WorldManager.Instance.Zones)
				{
					if (zone != null)
					{
						var zoneItem = new ZoneItem { Store = zone.Variables, Zone = zone, id = id++, displayName = zone.Zone.Name };
						zonesItem.AddChild(zoneItem);

						foreach (var npc in zone.Npcs)
						{
							var npcItem = new NpcItem { Store = npc.Variables, Npc = npc, id = id++, displayName = string.IsNullOrEmpty(npc.Name) ? "(Npc)" : npc.Name };
							zoneItem.AddChild(npcItem);
						}
					}
				}

				SetExpanded(zonesItem.id, true);
				SetupDepthsFromParentsAndChildren(rootItem);

				return rootItem;
			}

			protected override void SelectionChanged(IList<int> selectedIds)
			{
				_window.ClearSelection();

				foreach (var id in selectedIds)
				{
					var item = FindItem(id, rootItem) as StoreItem;

					if (item != null)
					{
						if (Event.current != null && Event.current.alt)
						{
							switch (item)
							{
								case PlayerItem player: Selection.activeObject = player.Player; break;
								case NpcItem npc: Selection.activeObject = npc.Npc; break;
							}
						}

						_window.AddToSelection(item);
					}
				}

				_window.Repaint();
			}

			protected override void RowGUI(RowGUIArgs args)
			{
				var storeItem = args.item as StoreItem;

				if (storeItem != null && WorldManager.Instance != null)
				{
					if (storeItem is ZoneItem zoneItem)
					{
						var rect = args.rowRect;
						rect.xMin += GetContentIndent(args.item);

						if (zoneItem.Zone == Player.Instance.Zone)
							EditorGUI.LabelField(rect, args.label, _activeStyle);
						else if (zoneItem.Zone.State == ZoneState.Loaded)
							EditorGUI.LabelField(rect, args.label, _loadedStyle);
						else
							base.RowGUI(args);
					}
					else
					{
						base.RowGUI(args);
					}
				}
				else
				{
					base.RowGUI(args);
				}
			}
		}
	}
}
