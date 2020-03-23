using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class WorldStateEditorTreeView : TreeView
	{
		public class WorldItem : TreeViewItem
		{
			public World World;
		}

		public class ZoneItem : TreeViewItem
		{
			public Zone Zone;
		}

		public WorldStateEditorTreeView(WorldStateWindow window, TreeViewState treeViewState) : base(treeViewState)
		{
			_window = window;
		}

		public void RestoreSelection()
		{
			SelectionChanged(GetSelection());
		}

		protected override TreeViewItem BuildRoot()
		{
			var id = 0;
			var worlds = AssetFinder.ListMainAssets<World>();
			var rootItem = new TreeViewItem { id = id++, depth = -1, displayName = "Root" };

			foreach (var world in worlds)
			{
				var worldItem = new WorldItem { World = world, id = id++, displayName = world.name };

				rootItem.AddChild(worldItem);

				foreach (var zone in world.Zones)
				{
					var name = worlds.Count > 1 ? string.Format("{0}/{1}", world.name, zone.name) : zone.name;
					var zoneItem = new ZoneItem { Zone = zone, id = id++, displayName = name };

					rootItem.AddChild(zoneItem);
				}
			}

			rootItem.children.Sort((left, right) => left.displayName.CompareTo(right.displayName));
			SetupDepthsFromParentsAndChildren(rootItem);

			return rootItem;
		}

		protected override void SelectionChanged(IList<int> selectedIds)
		{
			_window.ClearEditorSelection();

			foreach (var id in selectedIds)
			{
				var item = FindItem(id, rootItem);

				if (item is WorldItem)
					_window.AddToSelection((item as WorldItem).World);
				else if (item is ZoneItem)
					_window.AddToSelection((item as ZoneItem).Zone);
			}

			_window.Repaint();
		}

		private WorldStateWindow _window;
	}

	public class WorldStatePlayerTreeView : TreeView
	{
		public class WorldItem : TreeViewItem
		{
			public WorldManager World;
		}

		public class ZoneItem : TreeViewItem
		{
			public ZoneData Zone;
		}

		public WorldStatePlayerTreeView(WorldStateWindow window, TreeViewState treeViewState) : base(treeViewState)
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

			var worldItem = new WorldItem { World = WorldManager.Instance, id = id++, displayName = WorldManager.Instance.World.name };
			rootItem.AddChild(worldItem);

			foreach (var zone in WorldManager.Instance.Zones)
			{
				if (zone != null)
				{
					var zoneItem = new ZoneItem { Zone = zone, id = id++, displayName = zone.Zone.name };
					rootItem.AddChild(zoneItem);
				}
			}

			rootItem.children.Sort((left, right) => left.displayName.CompareTo(right.displayName));
			SetupDepthsFromParentsAndChildren(rootItem);

			return rootItem;
		}

		protected override void SelectionChanged(IList<int> selectedIds)
		{
			_window.ClearPlayerSelection();

			foreach (var id in selectedIds)
			{
				var item = FindItem(id, rootItem);

				if (item is WorldItem)
					_window.AddToSelection((item as WorldItem).World);
				else if (item is ZoneItem)
					_window.AddToSelection((item as ZoneItem).Zone);
			}

			_window.Repaint();
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var zoneItem = args.item as ZoneItem;

			if (zoneItem != null && WorldManager.Instance != null)
			{
				var rect = args.rowRect;
				rect.xMin += GetContentIndent(args.item);

				if (zoneItem.Zone == WorldManager.Instance.CurrentZone)
					EditorGUI.LabelField(rect, args.label, _activeStyle);
				else if (zoneItem.Zone.Status == ZoneStatus.Loaded)
					EditorGUI.LabelField(rect, args.label, _loadedStyle);
				else
					base.RowGUI(args);
			}
			else
			{
				base.RowGUI(args);
			}
		}

		private WorldStateWindow _window;
		private GUIStyle _loadedStyle;
		private GUIStyle _activeStyle;
	}

	public class WorldStateAssetPostprocessor : AssetPostprocessor
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			var windows = Resources.FindObjectsOfTypeAll<WorldStateWindow>();

			foreach (var window in windows)
				window.AssetsChanged();
		}
	}

	public class WorldStateWindow : EditorWindow
	{
		[MenuItem("Window/Monster Maker/World State")]
		public static void Open()
		{
			GetWindow<WorldStateWindow>("World State").Show();
		}

		public void ClearEditorSelection()
		{
			_editorSelections.Clear();
		}

		public void ClearPlayerSelection()
		{
			_playerSelections.Clear();
		}

		public void AddToSelection(World world)
		{
			_editorSelections.Add(new EditorSelection(world));
		}

		public void AddToSelection(Zone zone)
		{
			_editorSelections.Add(new EditorSelection(zone));
		}

		public void AddToSelection(WorldManager world)
		{
			_playerSelections.Add(new PlayerSelection(world));
		}

		public void AddToSelection(ZoneData zone)
		{
			_playerSelections.Add(new PlayerSelection(zone));
		}

		private enum ApplicationState
		{
			Transitioning,
			Editor,
			Player
		}

		private class EditorSelection
		{
			public ScriptableObject Asset;
			public World World;
			public Zone Zone;
			public VariableStoreControl StaticStore;
			public VariableStoreControl DefaultStore;

			public EditorSelection(World world)
			{
				Asset = world;
				World = world;
				StaticStore = new VariableStoreControl(world.StaticState, world.name, null, true, true);
				DefaultStore = new VariableStoreControl(world.DefaultState, world.name, null, true, true);
			}

			public EditorSelection(Zone zone)
			{
				Asset = zone;
				Zone = zone;
				StaticStore = new VariableStoreControl(zone.StaticState, zone.name, null, true, true);
				DefaultStore = new VariableStoreControl(zone.DefaultState, zone.name, null, true, true);
			}
		}

		private class PlayerSelection
		{
			public WorldManager World;
			public ZoneData Zone;
			public VariableStoreControl PersistentStore;
			public VariableStoreControl SessionStore;
			public VariableStoreControl LoadedStore;
			public VariableStoreControl ActiveStore;

			public PlayerSelection(WorldManager world)
			{
				World = world;
				PersistentStore = new VariableStoreControl(world.PersistentState, world.World.name, "", true, true);
				SessionStore = new VariableStoreControl(world.SessionState, world.World.name, "", true, true);
			}

			public PlayerSelection(ZoneData zone)
			{
				Zone = zone;
				PersistentStore = new VariableStoreControl(zone.PersistentState, zone.Zone.name, "", true, true);
				SessionStore = new VariableStoreControl(zone.SessionState, zone.Zone.name, "", true, true);
				LoadedStore = new VariableStoreControl(zone.LoadedState, zone.Zone.name, "", true, true);
				ActiveStore = new VariableStoreControl(zone.ActiveState, zone.Zone.name, "", true, true);
			}
		}

		private class Column
		{
			public string Name;
			public bool Visible = true;
		}

		private ApplicationState _state;
		private bool _loadPending = false;

		private GUIContent _foldoutIn;
		private GUIContent _foldoutOut;
		private GUIStyle _boxStyle;
		private float _splitMinimum = 100.0f;
		private float _splitPosition = 200.0f;
		private float _splitWidth = 3.0f;
		private bool _dragging = false;

		private WorldStateEditorTreeView _editorTree;
		private TreeViewState _editorState;
		private Vector2 _editorScrollPosition;
		private List<EditorSelection> _editorSelections = new List<EditorSelection>();
		private Column _editorStaticColumn = new Column { Name = "Static State" };
		private Column _editorDefaultColumn = new Column { Name = "Default State" };

		private WorldStatePlayerTreeView _playerTree;
		private TreeViewState _playerState;
		private Vector2 _playerScrollPosition;
		private List<PlayerSelection> _playerSelections = new List<PlayerSelection>();
		private Column _playerPersistentColumn = new Column { Name = "Persistent State" };
		private Column _playerSessionColumn = new Column { Name = "Session State" };
		private Column _playerLoadedColumn = new Column { Name = "Loaded State" };
		private Column _playerActiveColumn = new Column { Name = "Active State" };

		private void OnEnable()
		{
			_state = ApplicationState.Transitioning;

			_editorState = _editorState ?? new TreeViewState();
			_editorTree = new WorldStateEditorTreeView(this, _editorState);
			_editorTree.Reload();
			_editorTree.RestoreSelection();

			_playerState = _playerState ?? new TreeViewState();
			_playerTree = new WorldStatePlayerTreeView(this, _playerState);

			_foldoutIn = EditorGUIUtility.IconContent("IN foldout focus");
			_foldoutOut = EditorGUIUtility.IconContent("IN foldout focus on");
			_boxStyle = null;
		}

		public void AssetsChanged()
		{
			_editorTree.Reload();
			Repaint();
		}

		private void CurrentZoneChanged(ZoneData zone)
		{
			_loadPending = true;
			Repaint();
		}

		private void OnInspectorUpdate()
		{
			if (_state == ApplicationState.Transitioning)
			{
				if (EditorApplication.isPlaying)
				{
					if (WorldManager.Instance != null && WorldManager.Instance.CurrentZone != null)
					{
						_state = ApplicationState.Player;
						_playerTree.Reload();
						_playerTree.RestoreSelection();

						WorldManager.Instance.OnZoneEntered += CurrentZoneChanged;
						WorldManager.Instance.OnZoneLeft += CurrentZoneChanged;
					}
				}
				else if (!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					_state = ApplicationState.Editor;
				}
			}
			else if (_state == ApplicationState.Editor)
			{
				if (EditorApplication.isPlayingOrWillChangePlaymode)
					_state = ApplicationState.Transitioning;
			}
			else if (_state == ApplicationState.Player)
			{
				if (WorldManager.Instance == null)
				{
					_state = ApplicationState.Transitioning;
				}
				else if (EditorApplication.isPlaying)
				{
					if (_loadPending)
						_loadPending = WorldManager.Instance != null && WorldManager.Instance.IsLoading;
				}
				else
				{
					_state = ApplicationState.Editor;
				}
			}

			Repaint();
		}

		void OnGUI()
		{
			switch (_state)
			{
				case ApplicationState.Transitioning: ShowTransitioningGui(); break;
				case ApplicationState.Editor: ShowEditorGui(); break;
				case ApplicationState.Player: ShowPlayerGui(); break;
			}
		}

		private void ShowTransitioningGui()
		{
			GUILayout.Label("Updating...");
		}

		private void ShowEditorGui()
		{
			DrawTree(_editorTree);
			var splitterRect = DrawSplitter();

			using (new GUILayout.AreaScope(new Rect(_splitPosition + _splitWidth, 0, position.width - _splitPosition - _splitWidth, position.height)))
			{
				using (var scroll = new EditorGUILayout.ScrollViewScope(_editorScrollPosition))
				{
					if (DrawColumn(_editorStaticColumn))
					{
						foreach (var selection in _editorSelections)
							DrawEditorStore(selection.Asset, selection.StaticStore);
					}

					if (DrawColumn(_editorDefaultColumn))
					{
						foreach (var selection in _editorSelections)
							DrawEditorStore(selection.Asset, selection.DefaultStore);
					}

					_editorScrollPosition = scroll.scrollPosition;
				}
			}

			UpdateSplitter(splitterRect);
		}

		private void ShowPlayerGui()
		{
			DrawTree(_playerTree);
			var splitterRect = DrawSplitter();

			using (new GUILayout.AreaScope(new Rect(_splitPosition + _splitWidth, 0, position.width - _splitPosition - _splitWidth, position.height)))
			{
				using (var scroll = new EditorGUILayout.ScrollViewScope(_playerScrollPosition))
				{
					if (DrawColumn(_playerPersistentColumn))
					{
						foreach (var selection in _playerSelections)
							DrawPlayerStore(selection.PersistentStore);
					}
					
					if (DrawColumn(_playerSessionColumn))
					{
						foreach (var selection in _playerSelections)
							DrawPlayerStore(selection.SessionStore);
					}
					
					if (DrawColumn(_playerLoadedColumn))
					{
						foreach (var selection in _playerSelections)
						{
							if (selection.Zone != null && selection.Zone.Status == ZoneStatus.Loaded)
								DrawPlayerStore(selection.LoadedStore);
						}
					}

					if (DrawColumn(_playerActiveColumn))
					{
						foreach (var selection in _playerSelections)
						{
							if (selection.Zone != null && WorldManager.Instance != null && selection.Zone == WorldManager.Instance.CurrentZone)
								DrawPlayerStore(selection.ActiveStore);
						}
					}

					_playerScrollPosition = scroll.scrollPosition;
				}
			}

			UpdateSplitter(splitterRect);
		}

		private GUIStyle GetBoxStyle()
		{
			if (_boxStyle == null)
			{
				_boxStyle = new GUIStyle(GUI.skin.box);
				_boxStyle.fixedHeight = 20.0f;
				_boxStyle.stretchWidth = true;
				_boxStyle.margin = new RectOffset(0, 0, 1, 1);
			}

			return _boxStyle;
		}

		private void DrawTree(TreeView tree)
		{
			tree.OnGUI(new Rect(0, 0, _splitPosition - _splitWidth, position.height));
		}

		private Rect DrawSplitter()
		{
			var rect = new Rect(_splitPosition - _splitWidth, 0, _splitWidth * 2.0f, position.height);
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

		private bool DrawColumn(Column column)
		{
			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.grey * 1.4f;
			GUILayout.Box(GUIContent.none, GetBoxStyle());
			GUI.backgroundColor = backgroundColor;
			
			var labelRect = GUILayoutUtility.GetLastRect();
			var arrowRect = labelRect;

			arrowRect.yMin += 2;
			arrowRect.width = 20;

			labelRect.xMin += 20;
			labelRect.yMin += 3;

			var color = GUI.color;
			GUI.color = Color.black;
			EditorGUI.LabelField(arrowRect, column.Visible ? _foldoutOut : _foldoutIn);
			GUI.color = color;

			column.Visible = EditorGUI.Foldout(labelRect, column.Visible, column.Name, true, GUIStyle.none);

			return column.Visible;
		}

		private void DrawEditorStore(ScriptableObject asset, VariableStoreControl store)
		{
			GUILayout.Space(5);

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.VerticalScope())
				{
					using (new UndoScope(asset))
						store.Draw();
				}

				GUILayout.Space(10);
			}

			GUILayout.Space(5);
		}

		private void DrawPlayerStore(VariableStoreControl store)
		{
			GUILayout.Space(5);

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.VerticalScope())
					store.Draw();

				GUILayout.Space(10);
			}

			GUILayout.Space(5);
		}
	}
}
