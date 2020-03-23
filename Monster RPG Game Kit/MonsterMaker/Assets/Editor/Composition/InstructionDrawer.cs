using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(Instruction))]
	public class InstructionDrawer : PropertyDrawer
	{
		// TODO: listen for on set added/removed

		public static void InstructionSetChanged(InstructionSet set)
		{
			foreach (var data in _popups)
			{
				if (data.Set == set) data.FlyoutNames = null;
				data.RootNames = null;
			}
		}

		private class PopupData
		{
			public InstructionSet Set;

			public string[] FlyoutNames;
			public string[] RootNames;

			public string[] GetFlyoutNames()
			{
				if (FlyoutNames == null)
					FlyoutNames = Set != null ? Set.Instructions.Where(i => i != null).Select(i => Set.name + "/" + i.name).ToArray() : new string[0];

				return FlyoutNames;
			}

			public string[] GetRootNames()
			{
				if (RootNames == null)
				{
					var sets = GetPopups().Where(p => p != this).SelectMany(p => p.GetFlyoutNames());

					if (Set != null)
					{
						var instructions = Set.Instructions.Where(i => i != null).Select(i => i.name);
						var types = GetTypes();

						var list = new List<string>(instructions);
						list.Add("");
						list.AddRange(sets);
						list.Add("");
						list.AddRange(types.Names);

						RootNames = list.ToArray();
					}
					else
					{
						RootNames = sets.ToArray();
					}
				}

				return RootNames;
			}

			public int GetIndex(Instruction instruction)
			{
				var popups = GetPopups();
				var index = Set != null ? Set.Instructions.IndexOf(instruction) : -1;

				if (index >= 0)
					return index;
				
				index = Set != null ? Set.Instructions.Count + 1 : 0;

				for (var i = 0; i < popups.Count; i++)
				{
					var popup = popups[i];
					if (popup != this && popup.Set != null)
					{
						var popupIndex = popup.Set.Instructions.IndexOf(instruction);
						if (popupIndex >= 0)
							return index + popupIndex;

						index += popup.Set.Instructions.Count;
					}
				}

				return -1;
			}

			public Instruction GetInstruction(int index)
			{
				var popups = GetPopups();
				
				if (Set != null && index < Set.Instructions.Count)
					return Set.Instructions[index];

				if (Set != null)
					index -= Set.Instructions.Count + 1;
				
				for (var i = 0; i < popups.Count; i++)
				{
					var popup = popups[i];
					if (popup != this && popup.Set != null)
					{
						if (index < popup.Set.Instructions.Count)
							return popup.Set.Instructions[index];

						index -= popup.Set.Instructions.Count;
					}
				}

				if (Set != null)
				{
					index--;
					var types = GetTypes();
					return Create(Set, null, types.Types[index]);
				}

				return null;
			}
		}

		private static TypeSelectInfo _types;
		private static List<PopupData> _popups;

		private static TypeSelectInfo GetTypes()
		{
			if (_types == null)
				_types = TypeFinder.GetDerivedTypes<Instruction>("Create/");

			return _types;
		}

		private static List<PopupData> GetPopups()
		{
			if (_popups == null)
			{
				_popups = new List<PopupData>();
				_popups.Add(new PopupData { Set = null });

				var sets = AssetFinder.FindMainAssets<InstructionSet>().OrderBy(s => s.name);

				foreach (var set in sets)
					_popups.Add(new PopupData { Set = set });
			}

			return _popups;
		}

		private static PopupData GetPopup(InstructionSet set)
		{
			return GetPopups().Single(p => p.Set == set);
		}

		public static Instruction Draw(Rect position, InstructionSet set, Instruction currentInstruction, GUIContent label)
		{
			if (label != null)
				position = EditorGUI.PrefixLabel(position, label);

			var popup = GetPopup(set);
			var names = popup.GetRootNames();
			var index = popup.GetIndex(currentInstruction);
			var left = currentInstruction != null ? new Rect(position.x, position.y, position.width * 0.5f, EditorGUIUtility.singleLineHeight) : position;

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				index = EditorGUI.Popup(left, index, names);

				if (changes.changed)
					currentInstruction = popup.GetInstruction(index);
			}

			if (currentInstruction != null)
			{
				var edit = new Rect(left.xMax + 5, position.y, (position.width - left.width - 10) * 0.5f, EditorGUIUtility.singleLineHeight);
				var clear = new Rect(edit.xMax + 5, position.y, edit.width, edit.height);

				if (GUI.Button(edit, EditorHelper.EditContent))
					InstructionBreadcrumbs.Edit(currentInstruction);

				if (GUI.Button(clear, EditorHelper.ClearContent))
					currentInstruction = null;
			}

			return currentInstruction;
		}

		public static Instruction Draw(InstructionSet set, Instruction currentInstruction, GUIContent label)
		{
			var position = GUILayoutUtility.GetLastRect();
			var rect = new Rect(position.x, position.yMax + 5, EditorGUIUtility.currentViewWidth - position.x, EditorGUIUtility.singleLineHeight);
			
			GUILayout.Space(rect.height + 5);

			return Draw(rect, set, currentInstruction, label);
		}

		public static Instruction Create(InstructionSet set, SerializedProperty property, Type type)
		{
			var existingNames = set.Instructions.Select(i => i.name).ToArray();
			var instruction = ScriptableObject.CreateInstance(type) as Instruction;
			instruction.name = ObjectNames.GetUniqueName(existingNames, type.Name);
			instruction.hideFlags = HideFlags.HideInHierarchy;
			instruction.Set = set;

			set.Instructions.Add(instruction);

			if (property != null)
			{
				property.serializedObject.Update();
				property.objectReferenceValue = instruction;
			}

			AssetDatabase.AddObjectToAsset(instruction, set);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(set));

			InstructionSetChanged(set);

			return instruction;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var instruction = (property.serializedObject.targetObject as Instruction);

			label.tooltip = EditorHelper.GetTooltip(fieldInfo);
			property.objectReferenceValue = Draw(position, instruction != null ? instruction.Set : null, property.objectReferenceValue as Instruction, label);
		}
	}
}
