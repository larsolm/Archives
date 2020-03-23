using System.Linq;
using System.Reflection;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace PiRhoSoft.MonsterMaker
{
	public class VariableStoreControl : PropertyControl
	{
		private VariableStore _store;
		private EditableList<VariableValue> _list = new EditableList<VariableValue>();

		public VariableStoreControl()
		{
		}

		public void Draw()
		{
			_list.DrawList();
		}

		public VariableStoreControl(VariableStore store, string name, string tooltip, bool allowAdd, bool allowRemove, UnityAction<Rect, int> customDraw = null)
		{
			_store = store;

			var list = _list.Setup(_store.Variables, name, tooltip, false, false, false, allowAdd, allowRemove, customDraw ?? DrawStoreEntry, RemoveStoreEntry);
			list.onAddDropdownCallback += CustomAdd;
		}

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo)
		{
			_store = GetObject<VariableStore>(property);

			var access = fieldInfo.GetCustomAttributes(typeof(VariableAccessAttribute), true).FirstOrDefault() as VariableAccessAttribute;
			var list = _list.Setup(_store.Variables, property.name, property.tooltip, false, false, false, access == null ? true : access.AllowAdd, access == null ? true : access.AllowRemove, DrawStoreEntry, RemoveStoreEntry);
			list.onAddDropdownCallback += CustomAdd;
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return _list.Height;
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			_list.DrawList(position);
		}

		public void DrawStoreEntry(Rect rect, int index)
		{
			var variable = _store.GetVariable(index);

			var nameWidth = rect.width * 0.3f;
			var typeWidth = rect.width * 0.3f;
			var valueWidth = rect.width - nameWidth - typeWidth - 10;

			var nameRect = new Rect(rect.x, rect.y, nameWidth, rect.height);
			var typeRect = new Rect(nameRect.xMax + 5, rect.y, typeWidth, rect.height);
			var valueRect = new Rect(typeRect.xMax + 5, rect.y, valueWidth, EditorGUIUtility.singleLineHeight);

			DrawName(nameRect, variable);
			DrawType(typeRect, variable);
			DrawValue(valueRect, variable, index);
		}

		private void RemoveStoreEntry(int index)
		{
			_store.Remove(index);
		}

		private void CustomAdd(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddToStorePopup(_store));
		}

		private class AddToStorePopup : PopupWindowContent
		{
			private GUIContent _label = new GUIContent("New Variable", "Add a new variable to the store");
			private VariableStore _store;
			private string _newName = "Name";
			private int _newType = 4;

			public AddToStorePopup(VariableStore store)
			{
				_store = store;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 5);
			}

			public override void OnGUI(Rect rect)
			{
				var typeNames = VariableValueDrawer.GetTypeNames();

				EditorGUILayout.LabelField(_label);

				var enter = GuiFields.TextEnterField("NewName", GUIContent.none, ref _newName);
				_newType = EditorGUILayout.Popup(_newType, typeNames);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((enter || create) && !string.IsNullOrEmpty(_newName))
				{
					if (CreateVariable(_newName, _newType))
					{
						editorWindow.Close();
						_newName = "Name";
					}
				}
			}

			private bool CreateVariable(string name, int type)
			{
				switch ((VariableType)type)
				{
					case VariableType.Empty: return _store.TryAddEmpty(name);
					case VariableType.Boolean: return _store.TryAdd(name, false);
					case VariableType.Integer: return _store.TryAdd(name, 0);
					case VariableType.Number: return _store.TryAdd(name, 0.0f);
					case VariableType.String: return _store.TryAdd(name, "");
					case VariableType.Asset: return _store.TryAdd(name, (ScriptableObject)null);
					case VariableType.GameObject: return _store.TryAdd(name, (GameObject)null);
					case VariableType.Other: return _store.TryAdd(name, (object)null);
				}

				return false;
			}
		}

		private void DrawName(Rect rect, VariableValue variable)
		{
			EditorGUI.LabelField(rect, variable.Name);
		}

		private void DrawType(Rect rect, VariableValue variable)
		{
			GUI.enabled = false;
			EditorGUI.EnumPopup(rect, variable.Type);
			GUI.enabled = true;
		}

		private void DrawValue(Rect rect, VariableValue variable, int index)
		{
			switch (variable.Type)
			{
				case VariableType.Boolean:
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						var value = EditorGUI.Toggle(rect, variable.GetBoolean());
						if (changes.changed)
							_store.TrySet(index, value);
					}

					break;
				}
				case VariableType.Integer:
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						var value = EditorGUI.IntField(rect, variable.GetInteger());
						if (changes.changed)
							_store.TrySet(index, value);
					}

					break;
				}
				case VariableType.Number:
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						var value = EditorGUI.FloatField(rect, variable.GetNumber());
						if (changes.changed)
							_store.TrySet(index, value);
					}

					break;
				}
				case VariableType.String:
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						var value = EditorGUI.DelayedTextField(rect, variable.GetString());
						if (changes.changed)
							_store.TrySet(index, value);
					}

					break;
				}
				case VariableType.Asset:
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						var value = EditorGUI.ObjectField(rect, variable.GetAsset(), typeof(ScriptableObject), false);
						if (changes.changed)
							_store.TrySet(index, value);
					}

					break;
				}
				case VariableType.GameObject:
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						var value = EditorGUI.ObjectField(rect, variable.GetGameObject(), typeof(GameObject), true);
						if (changes.changed)
							_store.TrySet(index, value);
					}

					break;
				}
			}
		}
	}
}
