using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class ZoneSceneReferenceMaintainer : UnityEditor.AssetModificationProcessor
	{
		private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
		{
			var zones = AssetFinder.ListSubAssets<World, Zone>();

			foreach (var zone in zones)
			{
				if (zone.Scene.Path == path)
				{
					zone.Scene.Path = "";
					EditorUtility.SetDirty(zone);
				}
			}

			return AssetDeleteResult.DidNotDelete;
		}
		
		private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			var zones = AssetFinder.ListSubAssets<World, Zone>();

			foreach (var zone in zones)
			{
				if (zone.Scene.Path == sourcePath)
				{
					zone.Scene.Path = destinationPath;
					EditorUtility.SetDirty(zone);
				}
			}

			return AssetMoveResult.DidNotMove;
		}
	}

	[CustomPropertyDrawer(typeof(SceneReference))]
	public class SceneReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = EditorHelper.GetTooltip(fieldInfo);

			var dropRect = new Rect(position.x, position.y, position.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var loadRect = new Rect(dropRect.xMax + 5, position.y, position.width - dropRect.width - 5, dropRect.height);
			var pathProperty = property.FindPropertyRelative("Path");
			var paths = AssetFinder.ListScenes().ToArray();
			var names = paths.Select(path => path.Replace('/', ' ')).ToArray();
			var selected = Array.IndexOf(paths, pathProperty.stringValue);
			
			selected = EditorGUI.Popup(dropRect, label.text, selected, names);

			pathProperty.stringValue = selected >= 0 && selected < paths.Length ? paths[selected] : "";

			if (GUI.Button(loadRect, EditorHelper.LoadContent))
				EditorSceneManager.OpenScene(pathProperty.stringValue, OpenSceneMode.Additive);
		}
	}
}
