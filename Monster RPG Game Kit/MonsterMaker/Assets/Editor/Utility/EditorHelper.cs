using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.MonsterMaker
{
	public static class EditorHelper
	{
		static EditorHelper()
		{
		//	ActionEditors = TypeFinder.GetCustomEditors<Action, ActionEditor, CustomActionControlAttribute>();

			_separatorStyle = new GUIStyle { border = new RectOffset(0, 0, 1, 1) };
			_separatorStyle.normal.background = EditorGUIUtility.whiteTexture;
			_separatorStyle.fixedHeight = 1;
			_separatorStyle.margin = new RectOffset(50, 50, 0, 0);
		}

		public static GUIContent EditContent
		{
			get
			{
				if (_edit == null)
				{
					_edit = new GUIContent("Edit", "Edit this object.");
					_edit.image = EditorGUIUtility.IconContent("UnityEditor.InspectorWindow").image;
				}

				return _edit;
			}
		}

		public static GUIContent ClearContent
		{
			get
			{
				if (_clear == null)
				{
					_clear = new GUIContent("Clear", "Clear this object.");
					_clear.image = EditorGUIUtility.IconContent("winbtn_win_close").image;
				}

				return _clear;
			}
		}
		
		public static GUIContent CreateContent
		{
			get
			{
				if (_create == null)
				{
					_create = new GUIContent("Create", "Create this object.");
					_create.image = EditorGUIUtility.IconContent("Toolbar Plus").image;
				}

				return _create;
			}
		}

		public static GUIContent LoadContent
		{
			get
			{
				if (_load == null)
				{
					_load= new GUIContent("Load", "Load this scene.");
					_load.image = EditorGUIUtility.IconContent("SceneLoadIn").image;
				}

				return _load;
			}
		}

		public static GUIContent UnloadContent
		{
			get
			{
				if (_unload == null)
				{
					_unload = new GUIContent("Unload", "Unload this scene.");
					_unload.image = EditorGUIUtility.IconContent("SceneLoadOut").image;
				}

				return _unload;
			}
		}

		public static GUIContent BackContent
		{
			get
			{
				if (_back == null)
				{
					_back = new GUIContent("Back", "Return to the previous object.");
					_back.image = EditorGUIUtility.IconContent("SceneLoadOut").image;
				}

				return _back;
			}
		}

		public static GUIContent ForwardContent
		{
			get
			{
				if (_forward == null)
				{
					_forward = new GUIContent("Forward", "Return to the object you came back from.");
					_forward.image = EditorGUIUtility.IconContent("SceneLoadIn").image;
				}

				return _forward;
			}
		}

		public static GUIContent RootContent
		{
			get
			{
				if (_root == null)
				{
					_root = new GUIContent("Root", "Return to root instruction set.");
					_root.image = EditorGUIUtility.IconContent("d_preAudioLoopOff").image;
				}

				return _root;
			}
		}

		public static GUIContent RefreshContent
		{
			get
			{
				if (_refresh == null)
				{
					_refresh = new GUIContent("Refresh", "Refresh this object.");
					_refresh.image = EditorGUIUtility.IconContent("d_preAudioLoopOff").image;
				}

				return _refresh;
			}
		}

		public static CustomEditorInfo ActionEditors { get; private set; }

		public static void Separator(Color color)
		{
			EditorGUILayout.Space();

			var c = GUI.color;
			GUI.color = color;
			GUILayout.Box(GUIContent.none, _separatorStyle);
			GUI.color = c;

			EditorGUILayout.Space();
		}

		public static int ActionTypePicker(int selectedIndex)
		{
			return EditorGUILayout.Popup(selectedIndex, ActionEditors.TypeNames);
		}

		public static Action CreateAction(int typeIndex)
		{
			var type = ActionEditors.ItemTypes[typeIndex];
			return Activator.CreateInstance(type) as Action;
		}

		public static string GetTooltip(FieldInfo fieldInfo)
		{
			var tooltip = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() as TooltipAttribute;
			return tooltip == null ? "" : tooltip.tooltip;
		}

		public static void Edit(UnityObject toEdit)
		{
			Selection.activeObject = toEdit;
		}

		//public static ActionEditor CreateActionEditor(Action action)
		//{
		//	for (var i = 0; i < ActionEditors.Count; i++)
		//	{
		//		if (action.GetType() == ActionEditors.ItemTypes[i])
		//			return Activator.CreateInstance(ActionEditors.EditorTypes[i]) as ActionEditor;
		//	}
		//
		//	return null;
		//}

		private static GUIContent _edit;
		private static GUIContent _clear;
		private static GUIContent _create;
		private static GUIContent _load;
		private static GUIContent _unload;
		private static GUIContent _back;
		private static GUIContent _forward;
		private static GUIContent _root;
		private static GUIContent _refresh;
		private static GUIStyle _separatorStyle;
	}
}
