using Edgar.Unity.Examples;
using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        #region Field
        [SerializeField]
        private InventoryPage inventoryPage; //Inventory UI

        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryItem> initialItems = new List<InventoryItem>(); //�ʱ� ������
        #endregion

        public void RootItem(GameObject go) {
            Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
            InventoryItem invenItem = InventoryItem.GetEmptyItem();
            invenItem.item = new ItemSO();
            invenItem.item.ItemImage = sprite;
            invenItem.quantity = 1;
            inventoryData.AddItem(invenItem);
        }


        #region Handler
        private void HadnleItemActionRequest(int itemIndex) {

        }

        private void HandleDragging(int itemIndex) {
            InventoryItem invenItem = inventoryData.GetItemAt(itemIndex);
            if (invenItem.IsEmpty)
                return;
            inventoryPage.CreateDraggedItem(invenItem.item.ItemImage, invenItem.quantity);
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2) {
            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDescriptionRequest(int itemIndex) {
            InventoryItem item = inventoryData.GetItemAt(itemIndex);
            if (item.IsEmpty) {
                inventoryPage.ResetSelection();
                return;
            }
            ItemSO temp = item.item;
            inventoryPage.UpdateDescription(itemIndex,
                item.item.ItemImage, item.item.Name, item.item.Description);//selectó���� �Բ���
        }
        #endregion

        #region Monos
        private void Start() {
            PrepareUI();
            PrePareInventoryData();
        }
        public void Update() {
            if (InputHelper.GetKeyUp(KeyCode.I)) //I Ŭ���� ������ �κ��丮 ����
            {
                if (inventoryPage.isActiveAndEnabled == false) {
                    foreach (var item in inventoryData.GetCurrentInventoryState()) { //�κ��丮 ���� UI������Ʈ
                        inventoryPage.UpdateData(item.Key,
                            item.Value.item.ItemImage,
                            item.Value.quantity);
                    }
                    inventoryPage.Show();
                }
                else {
                    inventoryPage.Hide();
                }
            }
        }
        #endregion

        #region Utils
        private void PrepareUI() {
            inventoryPage.InitInventoryUI(inventoryData.Size); // ������ ������ŭ ���� Instance����
            inventoryPage.OnDescriptionRequested += HandleDescriptionRequest; //ItemUI -> Page -> Controller������ �����
            inventoryPage.OnSwapItems += HandleSwapItems;
            inventoryPage.OnStartDragging += HandleDragging;
            inventoryPage.OnitemActionRequested += HadnleItemActionRequest;
        }
        private void PrePareInventoryData() {

            //inventoryData.Initialize(); //�κ��丮 ������ �� ����������
            inventoryData.OnInventoryUpdated += UpdateInventoryUI; //Inventory������� ������ Inventory Update�ϴ� �Լ� ���
            foreach (InventoryItem item in initialItems) { //?? (����?)�� �׳� �Ѿ
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        /// <summary>
        /// InventorySO �� �ִ� ������ List���� UI�� ������Ʈ
        /// </summary>
        /// <param name="invenState"></param>
        private void UpdateInventoryUI(Dictionary<int, InventoryItem> invenState) {
            inventoryPage.ResetAllItems();
            foreach (var item in invenState) {
                inventoryPage.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }
        #endregion

    }

}