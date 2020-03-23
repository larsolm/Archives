using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class ListenerCategoryWindow : EditorWindow
	{
		public static void ShowWindow()
		{
			var window = GetWindow<ListenerCategoryWindow>(true, "Listener Categories", true);
			window.Show();
		}
		
		public static void Draw()
		{
			var categories = GetCategories();
			categories.DrawList();
		}

		private static EditableList<string> GetCategories()
		{
			if (_categories == null)
			{
				_categories = new EditableList<string>();
				var categories = _categories.Setup(UiPreferences.Instance.ListenerCategories, "Listener Categories", "The list of categories of listeners that Promt Instructions can trigger.", false, true, false, true, true, DrawCategory);
				categories.onAddCallback += AddCategory;
			}

			return _categories;
		}

		private static void AddCategory(ReorderableList list)
		{
			UiPreferences.Instance.ListenerCategories.Add("UI Category");
		}

		private static void DrawCategory(Rect rect, int index)
		{
			UiPreferences.Instance.ListenerCategories[index] = EditorGUI.TextField(rect, UiPreferences.Instance.ListenerCategories[index]);
		}

		private void OnGUI()
		{
			Draw();
		}

		private static EditableList<string> _categories;
	}
}
