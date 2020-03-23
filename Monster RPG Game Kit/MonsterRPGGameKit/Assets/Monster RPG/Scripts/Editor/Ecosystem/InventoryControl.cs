using System.Collections;
using System.Reflection;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class InventoryControl : ObjectControl<Inventory>
	{
		private static readonly IconButton _addButton = new IconButton(IconButton.CustomAdd, "Add an Item");
		private static readonly IconButton _editButton = new IconButton(IconButton.Edit, "Edit this Item");
		private static readonly IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this Item");
		private static readonly Label _moneyLabel = new Label(typeof(Inventory), nameof(Inventory.Money));
		private static readonly Label _itemsLabel = new Label(typeof(Inventory), nameof(Inventory.Items));
		private static readonly GUIContent _itemsEmptyLabel = new GUIContent("The Inventory is empty");

		private Object _owner;
		private Inventory _inventory;
		private ObjectListControl _listControl = new ObjectListControl();
		private GenericMenu _addMenu = new GenericMenu();

		public override void Setup(Inventory target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_inventory = target;
			_owner = property.serializedObject.targetObject;

			_listControl.Setup(_inventory.Items)
				.MakeEditable(_editButton)
				.MakeRemovable(_removeButton)
				.MakeDrawable(DrawItem)
				.MakeCollapsable(property.serializedObject.targetObject.GetType().Name + "." + property.propertyPath + ".IsOpen")
				.MakeReorderable()
				.MakeHeaderButton(_addButton, _addMenu, Color.white)
				.MakeEmptyLabel(_itemsEmptyLabel);
			
			var items = AssetHelper.GetAssetList<Item>(false, false);

			for (var i = 0; i < items.Assets.Count; i++)
			{
				if (items.Assets[i] != null)
					_addMenu.AddItem(items.Names[i], false, AddItem, items.Assets[i]);
			}
		}

		private void DrawItem(Rect rect, IList list, int index)
		{
			var item = _inventory.Items[index];
			item.Count = EditorGUI.IntField(rect, item.Item.Name, item.Count);
		}

		private void EditItem(IList list, int index)
		{
			Selection.activeObject = _inventory.Items[index].Item;
		}

		private void AddItem(object data)
		{
			var item = data as Item;

			using (new UndoScope(_owner, true))
				_inventory.Add(item, 1);
		}

		public override float GetHeight(GUIContent label)
		{
			return RectHelper.LineHeight + _listControl.GetHeight();
		}

		public override void Draw(Rect position, GUIContent label)
		{
			var moneyRect = RectHelper.TakeLine(ref position);

			_inventory.Money = EditorGUI.IntField(moneyRect, _moneyLabel.Content, _inventory.Money);
			_listControl.Draw(position, _itemsLabel.Content);
		}
	}

	[CustomPropertyDrawer(typeof(Inventory))]
	public class InventoryDrawer : ControlDrawer<InventoryControl>
	{
	}
}
