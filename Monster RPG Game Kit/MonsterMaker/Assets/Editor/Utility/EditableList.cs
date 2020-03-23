using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace PiRhoSoft.MonsterMaker
{
	public class EditableList<T>
	{
		public float Height
		{
			get
			{
				var height = EditorGUIUtility.singleLineHeight * 1.5f;

				if (_canFoldout && !Visible)
					return height;
				
				if (_canAdd)
					height += EditorGUIUtility.singleLineHeight * 1.5f;

				return height + (Mathf.Max(1, _list.count) * _elementHeight) + 5;
			}
		}

		public int Count { get { return _list.count; } }
		public bool Visible { get; private set; }

		private ReorderableList _list;
		private GUIContent _content;
		private GUIContent _foldoutIn;
		private GUIContent _foldoutOut;
		private UnityAction<Rect, SerializedProperty, int> _customDrawElement = null;
		private UnityAction<SerializedProperty, int> _customRemove = null;
		private UnityAction<Rect, int> _customDrawListElement = null;
		private UnityAction<int> _customRemoveListElement = null;

		private bool _canReorder;
		private bool _canAdd;
		private bool _canRemove;
		private bool _canFoldout;
		private bool _selectable;
		private float _elementHeight;
		private float _footerHeight;

		public ReorderableList Setup(SerializedProperty list, string name, string tooltip, bool foldout = true, bool canReorder = true, bool selectable = false, bool canAdd = true, bool canRemove = true, UnityAction<Rect, SerializedProperty, int> customDraw = null, UnityAction<SerializedProperty, int> customRemove = null)
		{
			_list = new ReorderableList(list.serializedObject, list, canReorder, true, canAdd, false);
			_list.drawElementCallback += DrawPropertyElement;
		
			_customDrawElement = customDraw;
			_customRemove = customRemove;
			
			SetupDefaults(name ?? list.name, tooltip ?? list.tooltip, foldout, canRemove, selectable, canAdd, canRemove);

			return _list;
		}

		public ReorderableList Setup(IList list, string name, string tooltip, bool foldout, bool canReorder, bool selectable, bool canAdd, bool canRemove, UnityAction<Rect, int> customDraw, UnityAction<int> customRemove = null)
		{
			_list = new ReorderableList(list, typeof(T), canReorder, true, canAdd, false);
			_list.drawElementCallback += DrawListElement;

			_customDrawListElement = customDraw;
			_customRemoveListElement = customRemove;

			SetupDefaults(name, tooltip, foldout, canReorder, selectable, canAdd, canRemove);

			return _list;
		}
	
		private void SetupDefaults(string name, string tooltip, bool foldout, bool canReorder, bool selectable, bool canAdd, bool canRemove)
		{
			_content = new GUIContent(ObjectNames.NicifyVariableName(name), tooltip);

			_foldoutIn = EditorGUIUtility.IconContent("IN foldout focus");
			_foldoutOut = EditorGUIUtility.IconContent("IN foldout focus on");
			
			_canReorder = canReorder;
			_canAdd = canAdd;
			_canRemove = canRemove;
			_canFoldout = foldout;
			_selectable = selectable;

			Visible = true;
			
			_list.drawHeaderCallback += DrawHeader;
			_elementHeight = _list.elementHeight;
			_footerHeight = _list.footerHeight;

			if (!_selectable)
				_list.drawElementBackgroundCallback += DrawBackground;
		}

		private void DrawHeader(Rect rect)
		{
			var labelRect = _canFoldout ? new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height) : rect;

			EditorGUI.LabelField(labelRect, _content);

			if (_canFoldout)
			{
				EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.height, rect.height), Visible ? _foldoutOut : _foldoutIn);

				if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
					Visible = !Visible;
			}

			if (Visible)
			{
				_list.elementHeight = _elementHeight;
				_list.displayAdd = _canAdd;
				_list.draggable = _canReorder;
				_list.footerHeight = _footerHeight;
			}
			else
			{
				_list.elementHeight = 0;
				_list.footerHeight = 0;
				_list.displayAdd = false;
				_list.draggable = false;
			}
		}

		private void DrawBackground(Rect rect, int index, bool isActive, bool isFocused)
		{
		}

		private void DrawPropertyElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (Visible &&  index < _list.serializedProperty.arraySize)
			{
				var propertyRect = _canRemove ? new Rect(rect.x, rect.y, rect.width - EditorGUIUtility.singleLineHeight - 10, rect.height) : rect;

				if (_customDrawElement == null) // TODO: ADD A DRAW INDEX FIELD?
					EditorGUI.PropertyField(propertyRect, _list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
				else
					_customDrawElement(propertyRect, _list.serializedProperty, index);

				if (_canRemove)
				{
					if (GUI.Button(new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), ReorderableList.defaultBehaviours.iconToolbarMinus, GUIStyle.none))
					{
						GUI.FocusControl("");

						if (_customRemove == null)
						{
							if (_list.serializedProperty.propertyType == SerializedPropertyType.ObjectReference &&_list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue != null)
								_list.serializedProperty.DeleteArrayElementAtIndex(index); // DO THIS TWICE FOR OBJECT REFERENCES SO IT ACTUALLY REMOVES THE ENTRY FROM THE ARRAY (STRANGE UNITY BEHAVIOUR).

							_list.serializedProperty.DeleteArrayElementAtIndex(index);
						}
						else
						{
							_customRemove(_list.serializedProperty, index);
						}
					}
				}
			}
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (Visible &&  index < _list.list.Count)
			{
				var propertyRect = _canRemove ? new Rect(rect.x, rect.y, rect.width - EditorGUIUtility.singleLineHeight - 10, rect.height) : rect;

				if (_customDrawListElement != null)
					_customDrawListElement(propertyRect, index);

				if (_canRemove)
				{
					if (GUI.Button(new Rect(rect.x + rect.width - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), ReorderableList.defaultBehaviours.iconToolbarMinus, GUIStyle.none))
					{
						GUI.FocusControl("");

						if (_customRemoveListElement == null)
							_list.list.RemoveAt(index);
						else
							_customRemoveListElement(index);
					}
				}
			}
		}

		public void DrawList()
		{
			_list.DoLayoutList();

		}

		public void DrawList(Rect rect)
		{
			_list.DoList(rect);
		}
	}
}
