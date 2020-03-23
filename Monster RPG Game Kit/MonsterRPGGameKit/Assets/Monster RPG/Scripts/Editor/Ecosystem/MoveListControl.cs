using System.Collections;
using System.Reflection;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class MoveListControl : ObjectControl<MoveList>
	{
		private static readonly IconButton _addButton = new IconButton(IconButton.CustomAdd, "Add a Move");
		private static readonly IconButton _editButton = new IconButton(IconButton.Edit, "Edit this Move");
		private static readonly IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this Move");
		private static readonly GUIContent _emptyLabel = new GUIContent("The Creature has no moves");

		private Object _owner;
		private MoveList _list;
		private ObjectListControl _listControl = new ObjectListControl();
		private GenericMenu _addMenu = new GenericMenu();

		public override void Setup(MoveList target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_list = target;
			_owner = property.serializedObject.targetObject;

			_listControl.Setup(_list)
				.MakeEditable(_editButton)
				.MakeRemovable(_removeButton)
				.MakeDrawable(DrawMove)
				.MakeCollapsable(property.serializedObject.targetObject.GetType() + "." + property.propertyPath + ".IsOpen")
				.MakeReorderable()
				.MakeHeaderButton(_addButton, _addMenu, Color.white)
				.MakeEmptyLabel(_emptyLabel);

			var abilities = AssetHelper.GetAssetList<Ability>(false, false);

			for (var i = 0; i < abilities.Assets.Count; i++)
			{
				if (abilities.Assets[i] != null)
					_addMenu.AddItem(abilities.Names[i], false, AddMove, abilities.Assets[i]);
			}
		}

		private void DrawMove(Rect rect, IList list, int index)
		{
			var move = _list[index];

			if (move)
				EditorGUI.LabelField(rect, move.Name);
			else
				EditorGUI.LabelField(rect, "(deleted)");
		}

		private void AddMove(object data)
		{
			var ability = data as Ability;
			var move = ability.CreateMove(_owner as Creature);

			using (new UndoScope(_owner, true))
			{
				_list.Add(move);
				Undo.RegisterCreatedObjectUndo(move, "Undo create");
			}
		}

		public override float GetHeight(GUIContent label)
		{
			return _listControl.GetHeight();
		}

		public override void Draw(Rect position, GUIContent label)
		{
			_listControl.Draw(position, label);
		}
	}

	[CustomPropertyDrawer(typeof(MoveList))]
	public class MoveListDrawer : ControlDrawer<MoveListControl>
	{
	}
}
