using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        InventorySO val = null;

        public void Initialize() {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++) {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public void AddItem(ItemSO item, int quantity) {
            for (int i = 0; i < inventoryItems.Count; i++) {
                if (inventoryItems[i].IsEmpty) {
                    inventoryItems[i] = new InventoryItem {
                        item = item,
                        quantity = quantity
                    };
                    return;
                }
            }
        }

        public void AddItem(InventoryItem item) {
            AddItem(item.item, item.quantity);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState() {
            Dictionary<int, InventoryItem> currentInvenState = new Dictionary<int, InventoryItem>();
            for (int i = 0; i < inventoryItems.Count; i++) {
                if (inventoryItems[i].IsEmpty)
                    continue;
                currentInvenState[i] = inventoryItems[i];
            }
            return currentInvenState;
        }

        public InventoryItem GetItemAt(int itemIndex) {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex1, int itemIndex2) {
            InventoryItem item1 = inventoryItems[itemIndex1];
            inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
            inventoryItems[itemIndex2] = item1;
            InformAboutChange();
        }

        private void InformAboutChange() {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }


    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;

        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity) {
            return new InventoryItem {
                item = this.item,
                quantity = newQuantity,
            };
        }

        public static InventoryItem GetEmptyItem() => new InventoryItem {
            item = null,
            quantity = 0,
        };
    }
}