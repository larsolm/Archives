using System.IO;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Definitions))]
	public class DefinitionsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Make Enum"))
			{
				var directory = "Assets/Scripts/Generated";
				var filename = directory + "/TestEnum.cs";

				Directory.CreateDirectory(directory);

				using (var writer = new StreamWriter(filename))
				{
					writer.WriteLine("public enum TestEnum");
					writer.WriteLine("{");
					writer.WriteLine("\tFirst,");
					writer.WriteLine("\tSecond");
					writer.WriteLine("}");
				}

				AssetDatabase.ImportAsset(filename);
			}
		}
	}
}
