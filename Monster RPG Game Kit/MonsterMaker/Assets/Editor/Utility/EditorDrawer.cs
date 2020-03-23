using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public static class EditorDrawer
	{
		private static GUIStyle _boxStyle;
		private static GUIStyle _tabStyle;

		public static void Box(Rect box, Color color)
		{
			var style = GetBoxStyle();

			using (ColorScope.Color(color))
				GUI.Box(box, GUIContent.none, style);
		}

		public static void Outline(Rect box, Color color, float thickness)
		{
			var style = GetBoxStyle();
			var topRect = new Rect(box.x, box.y, box.width, thickness);
			var bottomRect = new Rect(box.x, box.y + box.height - thickness, box.width, thickness);
			var leftRect = new Rect(box.x, box.y + thickness, thickness, box.height - thickness - thickness);
			var rightRect = new Rect(box.x + box.width - thickness, box.y + thickness, thickness, box.height - thickness - thickness);

			using (ColorScope.BackgroundColor(color))
			{
				GUI.Box(topRect, GUIContent.none, style);
				GUI.Box(bottomRect, GUIContent.none, style);
				GUI.Box(leftRect, GUIContent.none, style);
				GUI.Box(rightRect, GUIContent.none, style);
			}
		}

		public static void LayoutBox(Color color, float padding, float height)
		{
			GUILayout.Space(padding);

			var rect = GUILayoutUtility.GetRect(0.0f, height);
			rect.x += padding;
			rect.width -= 2 * padding;

			Box(rect, color);

			GUILayout.Space(padding);
		}

		public static void LayoutOutline(Color color, float padding, float height, float thickness)
		{
			GUILayout.Space(padding);

			var rect = GUILayoutUtility.GetRect(0.0f, height);
			rect.x += padding;
			rect.width -= 2 * padding;

			Outline(rect, color, thickness);

			GUILayout.Space(padding);
		}

		public static int TabBar(int active, string[] tabs)
		{
			var tabStyle = GetTabStyle();
			var inactiveColor = new Color(0.4f, 0.4f, 0.4f);
			var selectedColor = new Color(0.7725491f, 0.764706f, 0.7686275f);

			using (new GUILayout.HorizontalScope())
			{
				for (var i = 0; i < tabs.Length; i++)
				{
					using (ColorScope.BackgroundColor(i == active ? Color.white : inactiveColor))
					{
						if (GUILayout.Button(tabs[i], tabStyle))
							active = i;
					}
				}
			}

			var lineRect = GUILayoutUtility.GetLastRect();
			lineRect.y += lineRect.height - 2;
			lineRect.height = 2;

			Box(lineRect, selectedColor);

			return active;
		}

		private static GUIStyle GetBoxStyle()
		{
			if (_boxStyle == null)
			{
				_boxStyle = new GUIStyle();
				_boxStyle.normal.background = EditorGUIUtility.whiteTexture;
			}

			return _boxStyle;
		}

		private static GUIStyle GetTabStyle()
		{
			if (_tabStyle == null)
			{
				_tabStyle = new GUIStyle(GUI.skin.button);
				_tabStyle.padding.bottom = 5;
			}

			return _tabStyle;
		}
	}
}
