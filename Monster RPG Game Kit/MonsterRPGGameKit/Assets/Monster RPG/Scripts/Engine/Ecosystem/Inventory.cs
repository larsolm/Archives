using PiRhoSoft.CompositionEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class InventoryItem : IVariableStore
	{
		[Tooltip("The Item to add in the Inventory")] public Item Item;
		[Tooltip("The number of the Item in the Inventory")] public int Count;

		[NonSerialized] private Inventory _inventory;

		public Inventory Inventory
		{
			get => _inventory;
			internal set => _inventory = value;
		}

		#region Usage

		public IEnumerator UseInWorld(InstructionStore variables)
		{
			if (Item.Type == ItemType.Consumable)
				_inventory.Remove(this, 1);

			yield return Item.UseInWorld(variables);
		}

		public IEnumerator UseInBattle(InstructionStore variables)
		{
			if (Item.Type == ItemType.Consumable)
				_inventory.Remove(this, 1);

			yield return Item.UseInBattle(variables);
		}

		public void Toss(int amount)
		{
			if (Item.Type != ItemType.Key)
				_inventory.Remove(this, amount);
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name)
		{
			switch (name)
			{
				case nameof(Count): return VariableValue.Create(Count);
				default: return Item.GetVariable(name);
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(Count): return SetVariableResult.ReadOnly;
				default: return Item.SetVariable(name, value);
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { nameof(Count) }.Concat(Item.GetVariableNames());
		}

		#endregion
	}

	[Serializable]
	public class InventoryItemList : IndexedVariableStore<InventoryItem>
	{
	}

	[Serializable]
	public class Inventory : IVariableStore
	{
		private const string _removingItemWarning = "(EIRI) An Item on Inventory '{0}' was deleted and has been removed";

		[Tooltip("The amount of money the owner of the Inventory has")]
		public int Money;

		[Tooltip("The Items in the Inventory")]
		public InventoryItemList Items = new InventoryItemList();

		public void Setup(Object owner)
		{
			for (var i = 0; i < Items.Count; i++)
			{
				if (Items[i].Item == null)
				{
					Debug.LogWarningFormat(owner, _removingItemWarning, owner.name);
					Items.RemoveAt(i--);
				}
				else
				{
					Items[i].Inventory = this;
				}
			}
		}

		public void Add(Item item, int amount)
		{
			var inventoryItem = Find(item);

			if (inventoryItem == null)
			{
				inventoryItem = new InventoryItem { Item = item, Count = 0, Inventory = this };
				Items.Add(inventoryItem);
			}

			inventoryItem.Count += amount;
		}

		public void Remove(InventoryItem item, int amount)
		{
			item.Count -= amount;

			if (item.Count < 1)
			{
				for (var i = 0; i < Items.Count; i++)
				{
					if (item == Items[i])
					{
						Items.RemoveAt(i);
						break;
					}
				}
			}
		}

		public bool Contains(Item item, int amount)
		{
			var inventoryItem = Find(item);
			return inventoryItem?.Count >= amount;
		}

		private InventoryItem Find(Item item)
		{
			foreach (var inventoryItem in Items)
			{
				if (inventoryItem.Item == item)
					return inventoryItem;
			}

			return null;
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name)
		{
			switch (name)
			{
				case nameof(Money): return VariableValue.Create(Money);
				case nameof(Items): return VariableValue.Create(Items);
				default: return VariableValue.Empty;
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(Money): return value.TryGetInteger(out Money) ? SetVariableResult.Success : SetVariableResult.TypeMismatch;
				case nameof(Items): return SetVariableResult.ReadOnly;
				default: return SetVariableResult.NotFound;
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { nameof(Money), nameof(Items) };
		}

		#endregion
	}
}
