using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventoryItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler,
        IDropHandler, IDragHandler
    {
        [SerializeField]
        private Image _itemImage;

        [SerializeField]
        private TMP_Text _quantityTxt;

        [SerializeField]
        private Image _borderImage;

        [SerializeField]
        ItemBase _itemPointer;

        public event Action<InventoryItemUI> OnItemClicked, OnItemDroppedOn,
            OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

        private bool empty = true;

        #region Property
        public Image ItemImage { get { return _itemImage; } }
        public bool Empty { get { return empty; } }
        #endregion


        public void Awake() {
            ResetData();
            Deselect();
        }

        public void ResetData() {
            _itemImage.gameObject.SetActive(false);
            empty = true;
        }
        public void Deselect() {
            _borderImage.enabled = false;
        }
        public void SetData(Sprite sprite, int quantity) {
            _itemImage.gameObject.SetActive(true);
            _itemImage.sprite = sprite;
            _quantityTxt.text = quantity + "";
            empty = false;
        }

        public void Select() {
            _borderImage.enabled = true;
        }

        public void OnPointerClick(PointerEventData eventData) {
            PointerEventData pointerData = eventData;
            if (pointerData.button == PointerEventData.InputButton.Right) {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else if (pointerData.button == PointerEventData.InputButton.Left) {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (empty)
                return;
            OnItemBeginDrag?.Invoke(this); //OnItemBegin에 구독된 함수 있다면 Invoke
        }

        public void OnEndDrag(PointerEventData eventData) {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData) {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData) {
        }
    }
}