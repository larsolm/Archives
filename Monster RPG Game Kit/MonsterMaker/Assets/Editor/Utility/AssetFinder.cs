using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.MonsterMaker
{
	public static class AssetFinder
	{
		public static T GetMainAsset<T>() where T : ScriptableObject
		{
			return FindMainAssets<T>().FirstOrDefault();
		}

		public static List<T> ListMainAssets<T>() where T : ScriptableObject
		{
			return FindMainAssets<T>().ToList();
		}

		public static IEnumerable<T> FindMainAssets<T>() where T : ScriptableObject
		{
			var query = string.Format("t:{0}", typeof(T));
			return AssetDatabase.FindAssets(query).Select(id => GetAssetWithId<T>(id));
		}

		public static T GetAssetWithId<T>(string id) where T : ScriptableObject
		{
			var path = AssetDatabase.GUIDToAssetPath(id);
			return GetAssetAtPath<T>(path);
		}

		public static T GetAssetAtPath<T>(string path) where T : ScriptableObject
		{
			return AssetDatabase.LoadAssetAtPath<T>(path);
		}

		public static List<AssetType> ListSubAssets<ParentType, AssetType>() where ParentType : ScriptableObject where AssetType : ScriptableObject
		{
			var query = string.Format("t:{0}", typeof(ParentType));
			var parents = AssetDatabase.FindAssets(query);
			var assets = new List<AssetType>();

			foreach (var parent in parents)
			{
				var path = AssetDatabase.GUIDToAssetPath(parent);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				foreach (var child in children)
				{
					if (child is AssetType)
						assets.Add(child as AssetType);
				}
			}

			return assets;
		}

		public static ScriptableObject GetMainAsset(Type type)
		{
			return FindMainAssets(type).FirstOrDefault();
		}

		public static List<ScriptableObject> ListMainAssets(Type type)
		{
			return FindMainAssets(type).ToList();
		}

		public static IEnumerable<ScriptableObject> FindMainAssets(Type type)
		{
			var query = string.Format("t:{0}", type);
			return AssetDatabase.FindAssets(query).Select(id => GetAssetWithId(id, type));
		}

		public static ScriptableObject GetAssetWithId(string id, Type type)
		{
			var path = AssetDatabase.GUIDToAssetPath(id);
			return GetAssetAtPath(path, type);
		}

		public static ScriptableObject GetAssetAtPath(string path, Type type)
		{
			return AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
		}

		public static List<ScriptableObject> ListSubAssets(Type type, Type parentType)
		{
			var query = string.Format("t:{0}", parentType);
			var parents = AssetDatabase.FindAssets(query);
			var assets = new List<ScriptableObject>();

			foreach (var parent in parents)
			{
				var path = AssetDatabase.GUIDToAssetPath(parent);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				foreach (var child in children)
				{
					if (type.IsAssignableFrom(child.GetType()))
						assets.Add(child as ScriptableObject);
				}
			}

			return assets;
		}

		public static List<string> ListScenes()
		{
			var count = SceneManager.sceneCountInBuildSettings;
			var scenes = new List<string>();

			for (var i = 0; i < count; i++)
			{
				var scene = SceneUtility.GetScenePathByBuildIndex(i);
				scenes.Add(scene);
			}

			return scenes;
		}
	}
}
