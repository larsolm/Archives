using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.SnippetsEditor
{
	[InitializeOnLoad]
	public static class SelectionHistory
	{
		private const string _moveBackMenu = "Edit/Navigation/Move Back &LEFT";
		private const string _moveForwardMenu = "Edit/Navigation/Move Forward &RIGHT";

		public static int Current { get; private set; }
		public static List<Object[]> History { get; } = new List<Object[]>();

		private const int _capacity = 100;
		private static bool _skipNextSelection = false;

		static SelectionHistory()
		{
			Selection.selectionChanged += SelectionChanged;
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		[MenuItem(_moveBackMenu, validate = true)]
		public static bool CanMoveBack()
		{
			return Current > 0;
		}

		[MenuItem(_moveForwardMenu, validate = true)]
		public static bool CanMoveForward()
		{
			return Current < History.Count - 1;
		}

		[MenuItem(_moveBackMenu, priority = 1)]
		public static void MoveBack()
		{
			if (CanMoveBack())
				Select(--Current);
		}

		[MenuItem(_moveForwardMenu, priority = 2)]
		public static void MoveForward()
		{
			if (CanMoveForward())
				Select(++Current);
		}

		public static void GoTo(int index)
		{
			if (index != Current && index >= 0 && index < History.Count)
			{
				Current = index;
				Select(index);
			}
		}

		public static void Clear()
		{
			Current = 0;
			History.Clear();
		}

		private static void SelectionChanged()
		{
			if (!_skipNextSelection)
			{
				var trailing = History.Count - Current - 1;

				if (trailing > 0)
					History.RemoveRange(Current + 1, trailing);

				if (Current == _capacity)
					History.RemoveAt(0);

				History.Add(Selection.objects); // it doesn't seem like Unity ever reuses this array but if it does it would need to be copied here
				Current = History.Count - 1;
			}
			else
			{
				_skipNextSelection = false;
			}
		}

		private static void Select(int index)
		{
			_skipNextSelection = true;
			Selection.objects = History[index];
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			switch (state)
			{
				case PlayModeStateChange.ExitingEditMode: Clear(); break;
				case PlayModeStateChange.EnteredEditMode: Clear(); break;
			}
		}
	}
	
	public class SelectionHistoryWindow : EditorWindow
	{
		private const string _window = "History";
		private const string _showMenu = "Window/PiRho Soft/Selection History";

		private const string _emptyLabel = "(nothing)";
		private const string _multipleLabel = "(multiple)";
		private const string _deletedLabel = "(deleted)";

		private static readonly GUIContent _backButton = new GUIContent("< Back", "Edit the previous object in the history");
		private static readonly GUIContent _forwardButton = new GUIContent("Forward >", "Edit the next object in the history");

		private Vector2 _scrollPosition;

		[MenuItem(_showMenu)]
		public static void Open()
		{
			GetWindow<SelectionHistoryWindow>(_window).Show();
		}

		private void OnEnable()
		{
			Selection.selectionChanged += SelectionChanged;
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= SelectionChanged;
		}

		private void SelectionChanged()
		{
			_scrollPosition.y = SelectionHistory.Current * EditorGUIUtility.singleLineHeight;
			Repaint();
		}

		void OnGUI()
		{
			var brightness = 0.8549f; // same as the top of the Inspector window
			var rect = new Rect(0, 0, position.width, EditorGUIUtility.singleLineHeight + 10);
			EditorGUI.DrawRect(rect, new Color(brightness, brightness, brightness));

			GUILayout.Space(5);

			using (new EditorGUILayout.HorizontalScope())
			{
				using (new EditorGUI.DisabledScope(!SelectionHistory.CanMoveBack()))
				{
					if (GUILayout.Button(_backButton))
						SelectionHistory.MoveBack();
				}

				GUILayout.FlexibleSpace();

				using (new EditorGUI.DisabledScope(!SelectionHistory.CanMoveForward()))
				{
					if (GUILayout.Button(_forwardButton))
						SelectionHistory.MoveForward();
				}
			}

			GUILayout.Space(1);

			using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 26)))
			{
				for (var i = 0; i < SelectionHistory.History.Count; i++)
				{
					var selection = SelectionHistory.History[i];
					var text = GetName(selection);

					if (GUILayout.Button(new GUIContent(text), i == SelectionHistory.Current ? EditorStyles.boldLabel : EditorStyles.label))
						SelectionHistory.GoTo(i);
				}

				_scrollPosition = scroll.scrollPosition;
			}
		}

		private string GetName(Object[] objects)
		{
			if (objects == null || objects.Length == 0)
				return _emptyLabel;

			if (objects.Length > 1)
				return _multipleLabel;
			
			if (objects[0] == null)
				return _deletedLabel;

			if (string.IsNullOrEmpty(objects[0].name))
				return string.Format("({0})", objects[0].GetType().Name);

			return objects[0].name;
		}
	}
}
