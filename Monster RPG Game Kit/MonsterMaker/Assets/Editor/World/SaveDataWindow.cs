using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class SaveDataWindow : EditorWindow
	{
		private static string[] _tabs = new string[] { "Game", "World", "Player" };
		private string _filename;
		private int _tab;
		private Vector2 _gameScrollPosition;
		private Vector2 _worldScrollPosition;
		private Vector2 _playerScrollPosition;

		[MenuItem("Window/Monster Maker/Save Data")]
		public static void Open()
		{
			GetWindow<SaveDataWindow>("Save Data").Show();
		}
		
		void OnEnable()
		{
		}
		
		void OnInspectorUpdate()
		{
		}

		void OnGUI()
		{
			DrawFileGui();

			if (!string.IsNullOrEmpty(_filename))
			{
				_tab = EditorDrawer.TabBar(_tab, _tabs);

				switch (_tab)
				{
					case 0: DrawGameGui(); break;
					case 1: DrawWorldGui(); break;
					case 2: DrawPlayerGui(); break;
				}
			}
		}

		private void DrawFileGui()
		{
			// disable if game is running

			GUILayout.Space(5);

			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Open")) OpenFile();
				if (GUILayout.Button("New")) NewFile();

				GUILayout.FlexibleSpace();

				if (!string.IsNullOrEmpty(_filename))
				{
					if (GUILayout.Button("Save")) SaveFile();
					if (GUILayout.Button("Save As")) SaveFileAs();
					if (GUILayout.Button("Close")) CloseFile();
				}
			}

			GUILayout.Space(5);

			if (!string.IsNullOrEmpty(_filename))
			{
				EditorGUILayout.SelectableLabel(_filename);
			}
		}

		private void OpenFile()
		{
			var path = EditorUtility.OpenFilePanel("Open Save File", "", "save");
			if (!string.IsNullOrEmpty(path))
				LoadFile(path);
		}

		private void NewFile()
		{
			var path = EditorUtility.SaveFilePanel("Create Save File", "", "Editor.save", "save");
			if (!string.IsNullOrEmpty(path))
				LoadFile(path);
		}

		private void SaveFile()
		{
			SaveFile(_filename);
		}

		private void SaveFileAs()
		{
			var path = EditorUtility.SaveFilePanel("Save File", "", "Editor.save", "save");
			if (!string.IsNullOrEmpty(path))
				SaveFile(path);
		}

		private void CloseFile()
		{
			_filename = null;
		}

		private void SaveFile(string path)
		{
		}

		private void LoadFile(string path)
		{
			_filename = path;
		}

		private World _world;
		private Zone _zone;
		private SpawnPoint _spawn;

		private int _zoneIndex;
		private string[] _zoneNames;

		private void DrawGameGui()
		{
			// disable if game is running

			using (var scroll = new EditorGUILayout.ScrollViewScope(_gameScrollPosition))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					_world = (World)EditorGUILayout.ObjectField("World to Load", _world, typeof(World), false);

					if (changes.changed)
						_zoneNames = _world == null ? null : _world.Zones.Select(zone => zone.name).ToArray();
				}

				if (_zoneNames != null)
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						_zoneIndex = EditorGUILayout.Popup("Starting Zone", _zoneIndex, _zoneNames);

						if (changes.changed)
							_zone = _world != null && _zoneIndex >= 0 ? _world.Zones[_zoneIndex] : null;
					}
				}

				if (_zone != null)
				{
				}

				_gameScrollPosition = scroll.scrollPosition;
			}
		}

		private void DrawWorldGui()
		{
			// load file into variable stores while editing
			// show sessions states while playing

			using (var scroll = new EditorGUILayout.ScrollViewScope(_worldScrollPosition))
			{
				for (var i = 0; i < 50; i++)
					EditorGUILayout.LabelField("World");

				_worldScrollPosition = scroll.scrollPosition;
			}
		}

		private void DrawPlayerGui()
		{
			using (var scroll = new EditorGUILayout.ScrollViewScope(_playerScrollPosition))
			{
				for (var i = 0; i < 50; i++)
					EditorGUILayout.LabelField("Player");

				_playerScrollPosition = scroll.scrollPosition;
			}
		}
	}
}
