using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(RandomTile))]
	public class RandomTileEditor : Editor
	{
		private static readonly IconButton _createTilesButton = new IconButton(IconButton.Edit, "Add all the sprites from the same sheet as the first sprite");
		private static readonly IconButton _addTileButton = new IconButton(IconButton.Add, "Add a tile to the list");
		private static readonly IconButton _removeTileButton = new IconButton(IconButton.Remove, "Remove this tile from the list");
		private static readonly Label _tilesContent = new Label(typeof(RandomTile), nameof(RandomTile.Tiles));

		private RandomTile _randomTile;
		private SerializedProperty _noiseScaleProperty;
		private PropertyListControl _tiles = new PropertyListControl();

		void OnEnable()
		{
			_randomTile = target as RandomTile;

			_noiseScaleProperty = serializedObject.FindProperty(nameof(RandomTile.NoiseScale));
			_tiles.Setup(serializedObject.FindProperty(nameof(_randomTile.Tiles)))
				.MakeAddable(_addTileButton)
				.MakeRemovable(_removeTileButton)
				.MakeHeaderButton(_createTilesButton, CreateTiles, Color.white)
				.MakeCustomHeight(GetHeight)
				.MakeReorderable();
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_noiseScaleProperty);
				_tiles.Draw(_tilesContent.Content);
			}
		}

		private float GetHeight(int index)
		{
			return TileTransformInfoControl.Height;
		}

		private void CreateTiles(Rect rect)
		{
			var reference = _randomTile.Tiles.FirstOrDefault().Sprite;
			if (reference)
			{
				var path = AssetDatabase.GetAssetPath(reference.texture);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				_randomTile.Tiles.Clear();

				foreach (var child in children)
				{
					if (child is Sprite sprite)
						_randomTile.Tiles.Add(new TileTransformInfo { Sprite = sprite });
				}
			}
		}
	}
}