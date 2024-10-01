using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventoryDescription : MonoBehaviour
    {
        [SerializeField]
        private Image _itemImage;
        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private TMP_Text _description;

        public void Awake() {
            ResetDescription();
        }

        public void ResetDescription() {
            _itemImage.gameObject.SetActive(false);
            _title.text = "";
            _description.text = "";
        }

        public void SetDescription(Sprite sprite, string itemName, string itemDes) {
            _itemImage.gameObject.SetActive(true);
            _itemImage.sprite = sprite;
            _title.text = itemName;
            _description.text = itemDes;
        }
    }
}