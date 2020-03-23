using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class ShopItem : IVariableStore
	{
		[Tooltip("The Item to add to an Inventory when purchased")]
		[AssetPopup]
		public Item Item;

		[Tooltip("The number of the Item to add to an Inventory when purchased")]
		[Minimum(1)]
		public int Count = 1;

		public int PurchaseCost => Count * Item.PurchaseCost;
		public int SellCost => Count * Item.PurchaseCost;

		public bool CanPurchase(Inventory inventory)
		{
			return inventory.Money >= PurchaseCost;
		}

		public void Purchase(Inventory inventory)
		{
			inventory.Money -= PurchaseCost;
			inventory.Add(Item, Count);
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name)
		{
			switch (name)
			{
				case nameof(Count): return VariableValue.Create(Count);
				case nameof(PurchaseCost): return VariableValue.Create(PurchaseCost);
				case nameof(SellCost): return VariableValue.Create(SellCost);
				default: return Item.GetVariable(name);
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(Count): return SetVariableResult.ReadOnly;
				case nameof(PurchaseCost): return SetVariableResult.ReadOnly;
				case nameof(SellCost): return SetVariableResult.ReadOnly;
				default: return Item.SetVariable(name, value);
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { nameof(Count), nameof(PurchaseCost), nameof(SellCost) }.Concat(Item.GetVariableNames());
		}

		#endregion
	}

	[Serializable]
	public class ShopItemList : IndexedVariableStore<ShopItem> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "shop")]
	[AddComponentMenu("PiRho Soft/Ecosystem/Shop")]
	public class Shop : MonoBehaviour, IVariableStore
	{
		[Tooltip("The Items in the Shop")]
		[ListDisplay(ItemDisplay = ListItemDisplayType.Inline, EmptyText = "The Shop is empty")]
		public ShopItemList Items = new ShopItemList();

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name)
		{
			switch (name)
			{
				case nameof(Items): return VariableValue.Create(Items);
				default: return VariableValue.Empty;
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(Items): return SetVariableResult.ReadOnly;
				default: return SetVariableResult.NotFound;
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { nameof(Items) };
		}

		#endregion
	}
}
