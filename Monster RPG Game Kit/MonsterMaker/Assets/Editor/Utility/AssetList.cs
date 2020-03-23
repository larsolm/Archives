using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class AssetList
	{
		public Type Type;
		public GUIContent[] Names;
		public ScriptableObject[] Assets;
	}

	public class AssetLister : AssetPostprocessor
	{
		private static Dictionary<Type, AssetList> _mainAssetLists = new Dictionary<Type, AssetList>();
		private static Dictionary<Type, AssetList> _subAssetLists = new Dictionary<Type, AssetList>();

		public static AssetList GetMainAssetList<T>() where T : ScriptableObject
		{
			AssetList list;
			if (!_mainAssetLists.TryGetValue(typeof(T), out list))
			{
				list = new AssetList { Type = typeof(T) };
				_mainAssetLists.Add(typeof(T), list);
			}

			if (list.Assets == null)
			{
				var assets = AssetFinder.ListMainAssets<T>();
				BuildAssetList(list, assets);
			}

			return list;
		}

		public static AssetList GetSubAssetList<P, T>() where P : ScriptableObject where T : ScriptableObject
		{
			AssetList list;
			if (!_subAssetLists.TryGetValue(typeof(T), out list))
			{
				list = new AssetList { Type = typeof(T) };
				_subAssetLists.Add(typeof(T), list);
			}

			if (list.Assets == null)
			{
				var assets = AssetFinder.ListSubAssets<P, T>();
				BuildAssetList(list, assets);
			}

			return list;
		}

		public static AssetList GetMainAssetList(Type type)
		{
			AssetList list;
			if (!_mainAssetLists.TryGetValue(type, out list))
			{
				list = new AssetList { Type = type };
				_mainAssetLists.Add(type, list);
			}

			if (list.Assets == null)
			{
				var assets = AssetFinder.ListMainAssets(type);
				BuildAssetList(list, assets);
			}

			return list;
		}

		public static AssetList GetSubAssetList(Type type, Type parentType)
		{
			AssetList list;
			if (!_subAssetLists.TryGetValue(type, out list))
			{
				list = new AssetList { Type = type };
				_subAssetLists.Add(type, list);
			}

			if (list.Assets == null)
			{
				var assets = AssetFinder.ListSubAssets(type, parentType);
				BuildAssetList(list, assets);
			}

			return list;
		}

		private static void BuildAssetList<T>(AssetList list, List<T> assets) where T : ScriptableObject
		{
			var count = assets.Count + 2;
			list.Names = new GUIContent[count];
			list.Assets = new ScriptableObject[count];

			list.Names[0] = new GUIContent("None");
			list.Names[1] = new GUIContent("");
			list.Assets[0] = null;
			list.Assets[1] = null;

			for (var i = 0; i < assets.Count; i++)
			{
				list.Names[i + 2] = new GUIContent(assets[i].name);
				list.Assets[i + 2] = assets[i];
			}
		}

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (var list in _mainAssetLists)
			{
				list.Value.Names = null;
				list.Value.Assets = null;
			}

			foreach (var list in _subAssetLists)
			{
				list.Value.Names = null;
				list.Value.Assets = null;
			}
		}
	}
}
