using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	class SubassetPicker<ParentType, AssetType> : AssetPicker<AssetType> where ParentType : ScriptableObject where AssetType: ScriptableObject
	{
		protected override void FindAssets()
		{
			_assets = AssetFinder.ListSubAssets<ParentType, AssetType>();
		}
	}

	class AssetPicker<AssetType> where AssetType : ScriptableObject
	{
		protected List<AssetType> _assets;

		private List<string> _options = new List<string>();
		private GUIContent _content;
		private bool _allowMultiObjectSelection;

		public void Setup(GUIContent content, bool allowMultiObjectSelection)
		{
			FindAssets();

			_content = content;
			_allowMultiObjectSelection = allowMultiObjectSelection;

			foreach (var asset in _assets)
				_options.Add(asset.name);

			_options.Add("None");
			_options.Add("different values");
		}

		public bool DrawDropDownList(ref AssetType asset, bool hasDifferentOptions = false, params GUILayoutOption[] layout)
		{
			var options = _allowMultiObjectSelection && hasDifferentOptions ? _options.ToArray() : _options.GetRange(0, _options.Count - 1).ToArray();
			var currentIndex = asset ? _allowMultiObjectSelection && hasDifferentOptions ? _options.Count - 1 : _assets.IndexOf(asset) : _options.Count - 2;
			var selectedIndex = EditorGUILayout.Popup(_content, currentIndex, options, layout);

			if (currentIndex != selectedIndex)
				asset = selectedIndex == _options.Count - 2 ? null : _assets[selectedIndex];

			return selectedIndex != currentIndex;
		}

		protected virtual void FindAssets()
		{
			_assets = AssetFinder.ListMainAssets<AssetType>();
		}
	}
}
