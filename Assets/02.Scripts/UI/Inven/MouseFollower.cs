using Inventory.UI;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private InventoryItemUI item;

    public void Awake() {
        Toggle(false);
        canvas = transform.root.GetComponent<Canvas>();
        item = GetComponentInChildren<InventoryItemUI>();
    }

    public void SetData(Sprite sprite, int quantity) {
        item.SetData(sprite, quantity);
    }
    void Update() {
        Vector2 position;
        //Ω∫≈©∏∞¡¬«•∏¶ ∆Ø¡§UI¿« ∑Œƒ√¡¬«•∑Œ πŸ≤„¡‹
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
                );
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val) {
        Debug.Log($"Item toggled {val}");
        gameObject.SetActive(val);
    }
}