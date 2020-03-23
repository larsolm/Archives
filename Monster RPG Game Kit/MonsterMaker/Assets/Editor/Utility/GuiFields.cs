using System;
using PiRhoSoft.MonsterMaker;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	public static class GuiFields
	{
		public static void MainAssetPopup(GUIContent label, Type type, ref ScriptableObject asset, GUIStyle style = null, params GUILayoutOption[] options)
		{
			var list = AssetLister.GetMainAssetList(type);
			AssetPopup(list, label, ref asset, style, options);
		}

		public static void MainAssetPopup(Rect rect, GUIContent label, Type type, ref ScriptableObject asset, GUIStyle style = null)
		{
			var list = AssetLister.GetMainAssetList(type);
			AssetPopup(list, rect, label, ref asset, style);
		}

		public static void MainAssetPopup<AssetType>(GUIContent label, ref AssetType asset, GUIStyle style = null, params GUILayoutOption[] options) where AssetType : ScriptableObject
		{
			var list = AssetLister.GetMainAssetList<AssetType>();
			AssetPopup(list, label, ref asset, style, options);
		}

		public static void MainAssetPopup<AssetType>(Rect rect, GUIContent label, ref AssetType asset, GUIStyle style = null) where AssetType : ScriptableObject
		{
			var list = AssetLister.GetMainAssetList<AssetType>();
			AssetPopup(list, rect, label, ref asset, style);
		}

		public static void SubAssetPopup(GUIContent label, Type type, Type parentType, ref ScriptableObject asset, GUIStyle style = null, params GUILayoutOption[] options)
		{
			var list = AssetLister.GetSubAssetList(type, parentType);
			AssetPopup(list, label, ref asset, style, options);
		}

		public static void SubAssetPopup(Rect rect, GUIContent label, Type type, Type parentType, ref ScriptableObject asset, GUIStyle style = null)
		{
			var list = AssetLister.GetSubAssetList(type, parentType);
			AssetPopup(list, rect, label, ref asset, style);
		}

		public static void SubAssetPopup<ParentType, AssetType>(GUIContent label, ref AssetType asset, GUIStyle style = null, params GUILayoutOption[] options) where ParentType : ScriptableObject where AssetType : ScriptableObject
		{
			var list = AssetLister.GetSubAssetList<ParentType, AssetType>();
			AssetPopup(list, label, ref asset, style, options);
		}

		public static void SubAssetPopup<ParentType, AssetType>(Rect rect, GUIContent label, ref AssetType asset, GUIStyle style = null) where ParentType : ScriptableObject where AssetType : ScriptableObject
		{
			var list = AssetLister.GetSubAssetList<ParentType, AssetType>();
			AssetPopup(list, rect, label, ref asset, style);
		}

		private static void AssetPopup<AssetType>(AssetList list, GUIContent label, ref AssetType asset, GUIStyle style, params GUILayoutOption[] options) where AssetType : ScriptableObject
		{
			var index = ArrayUtility.IndexOf(list.Assets, asset);
			index = EditorGUILayout.Popup(label, index, list.Names, style ?? EditorStyles.popup, options);
			asset = list.Assets[index] as AssetType;
		}

		private static void AssetPopup<AssetType>(AssetList list, Rect rect, GUIContent label, ref AssetType asset, GUIStyle style) where AssetType : ScriptableObject
		{
			var index = ArrayUtility.IndexOf(list.Assets, asset);
			index = EditorGUI.Popup(rect, label, index, list.Names, style ?? EditorStyles.popup);
			asset = list.Assets[index] as AssetType;
		}

		public static bool TextEnterField(string controlName, GUIContent label, ref string text, GUIStyle style = null, params GUILayoutOption[] options)
		{
			GUI.SetNextControlName(controlName);
			text = EditorGUILayout.TextField(label, text, style ?? EditorStyles.textField, options);
			return WasEnterPressed(controlName);
		}

		public static bool FloatEnterField(string controlName, GUIContent label, ref float value, GUIStyle style = null, params GUILayoutOption[] options)
		{
			GUI.SetNextControlName(controlName);
			value = EditorGUILayout.FloatField(label, value, style ?? EditorStyles.numberField, options);
			return WasEnterPressed(controlName);
		}

		public static bool IntEnterField(string controlName, GUIContent label, ref int value, GUIStyle style = null, params GUILayoutOption[] options)
		{
			GUI.SetNextControlName(controlName);
			value = EditorGUILayout.IntField(label, value, style ?? EditorStyles.numberField, options);
			return WasEnterPressed(controlName);
		}

		private static bool WasEnterPressed(string controlName)
		{
			return Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return) && GUI.GetNameOfFocusedControl() == controlName;
		}
	}
}
