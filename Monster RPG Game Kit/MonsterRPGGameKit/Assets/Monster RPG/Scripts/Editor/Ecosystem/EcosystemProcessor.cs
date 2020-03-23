using PiRhoSoft.MonsterRpgEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class EcosystemProcessor
	{
		public class AssetImporter : AssetPostprocessor
		{
			static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
				foreach (string path in importedAssets)
				{
					var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

					switch (asset)
					{
						case Species species:
						{
							if (string.IsNullOrEmpty(species.Name))
							{
								species.Name = species.name;
								EditorUtility.SetDirty(species);
							}

							break;
						}
						case Ability ability:
						{
							if (string.IsNullOrEmpty(ability.Name))
							{
								ability.Name = ability.name;
								EditorUtility.SetDirty(ability);
							}

							break;
						}
						case Item item:
						{
							if (string.IsNullOrEmpty(item.Name))
							{
								item.Name = item.name;
								EditorUtility.SetDirty(item);
							}

							break;
						}
					}
				}
			}
		}
	}
}
