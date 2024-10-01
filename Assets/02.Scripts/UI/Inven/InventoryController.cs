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

        public List<InventoryItem> initialItems = new List<InventoryItem>(); //초기 데이터
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
                item.item.ItemImage, item.item.Name, item.item.Description);//select처리도 함께함
        }
        #endregion

        #region Monos
        private void Start() {
            PrepareUI();
            PrePareInventoryData();
        }
        public void Update() {
            if (InputHelper.GetKeyUp(KeyCode.I)) //I 클릭시 여러개 인벤토리 생성
            {
                if (inventoryPage.isActiveAndEnabled == false) {
                    foreach (var item in inventoryData.GetCurrentInventoryState()) { //인벤토리 열때 UI업데이트
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
            inventoryPage.InitInventoryUI(inventoryData.Size); // 아이템 갯수만큼 격자 Instance생성
            inventoryPage.OnDescriptionRequested += HandleDescriptionRequest; //ItemUI -> Page -> Controller순으로 보면됨
            inventoryPage.OnSwapItems += HandleSwapItems;
            inventoryPage.OnStartDragging += HandleDragging;
            inventoryPage.OnitemActionRequested += HadnleItemActionRequest;
        }
        private void PrePareInventoryData() {

            //inventoryData.Initialize(); //인벤토리 데이터 다 지워버리기
            inventoryData.OnInventoryUpdated += UpdateInventoryUI; //Inventory변경사항 있으면 Inventory Update하는 함수 등록
            foreach (InventoryItem item in initialItems) { //?? (뭐지?)비어서 그냥 넘어감
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        /// <summary>
        /// InventorySO 에 있는 아이템 List들을 UI에 업데이트
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