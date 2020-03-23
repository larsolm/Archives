using PiRhoSoft.MonsterRpgEngine;
using UnityEditor;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class WorldProcessor
	{
		public class ZoneImporter : AssetPostprocessor
		{
			static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
				foreach (string path in importedAssets)
				{
					var zone = AssetDatabase.LoadAssetAtPath<Zone>(path);
					if (zone != null && string.IsNullOrEmpty(zone.Name))
					{
						zone.Name = zone.name;
						EditorUtility.SetDirty(zone);
					}
				}
			}
		}

		public class ZoneMaintainer : UnityEditor.AssetModificationProcessor
		{
			private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
			{
				var zone = AssetDatabase.LoadAssetAtPath<Zone>(path);

				if (zone != null && zone.World != null)
				{
					zone.World.Zones.Remove(zone);
					EditorUtility.SetDirty(zone.World);
				}

				return AssetDeleteResult.DidNotDelete;
			}
		}
	}
}
