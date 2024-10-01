using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.UI
{
    public class InventoryPage : MonoBehaviour
    {
        /// <summary>
        /// 생성할 프리팹
        /// </summary>
        [SerializeField]
        private InventoryItemUI itemPrefab;

        /// <summary>
        /// Canvas -> InGameMenu -> Inventory -> Inventory Content
        /// </summary>
        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private InventoryDescription itemDescription;

        [SerializeField]
        private MouseFollower mouseFollower;

        List<InventoryItemUI> listOfItems = new List<InventoryItemUI>();

        public event Action<int> OnDescriptionRequested,
            OnitemActionRequested,
            OnStartDragging;

        public event Action<int, int> OnSwapItems;

        private int currentlyDraggedItemIndex = -1;


        #region monos
        private void Awake() {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }
        #endregion

        /// <summary>
        /// InventorySize 만큼 UI에 격자 추가
        /// </summary>
        /// <param name="inventorysize"></param>
        public void InitInventoryUI(int inventorysize) {
            for (int i = 0; i < inventorysize; i++) {
                InventoryItemUI uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        #region Hadles
        private void HandleShowItemActions(InventoryItemUI item) {
            throw new NotImplementedException();
        }

        private void HandleEndDrag(InventoryItemUI item) {
            ResetDraggedItem();
        }

        private void HandleSwap(InventoryItemUI item) {
            int index = listOfItems.IndexOf(item);
            if (index == -1) {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        }

        private void HandleBeginDrag(InventoryItemUI item) {
            int index = listOfItems.IndexOf(item);
            if (index == -1)
                return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(item);
            OnStartDragging?.Invoke(index);
        }

        private void HandleItemSelection(InventoryItemUI item) {
            int index = listOfItems.IndexOf(item);
            if (index == -1)
                return;
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show() {
            gameObject.SetActive(true);
            itemDescription.ResetDescription();//열때 description이 있는걸 방지
            ResetSelection();
        }
        #endregion


        #region Utility
        public void Hide() {
            gameObject.SetActive(false);
            ResetDraggedItem();
            DeselectAllItems();
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity) {
            if (listOfItems.Count > itemIndex) {
                listOfItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        public void ResetDraggedItem() {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        public void CreateDraggedItem(Sprite sprtie, int quantity) {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprtie, quantity);
        }

        public void ResetSelection() {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        private void DeselectAllItems() {
            foreach (InventoryItemUI item in listOfItems) {
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite sprite, string name, string des) {
            itemDescription.SetDescription(sprite, name, des);
            DeselectAllItems();
            listOfItems[itemIndex].Select();
        }

        internal void ResetAllItems() {
            foreach (InventoryItemUI item in listOfItems) {
                item.ResetData();
                item.Deselect();
            }
        }

        #endregion
    }
}