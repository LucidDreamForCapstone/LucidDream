using UnityEngine;

public class ShopItem_Key : ShopItemBase
{
    [Header("열쇠 구매한 사운드")]
    [SerializeField] private AudioClip keySound; // 포션 사운드
    protected override bool CanPurchase()
    {
        // 플레이어가 이미 Key 아이템을 가지고 있는지 확인
        if (InventoryManager.Instance.HasItem(ItemType.Key))
        {
            // 이미 Key 아이템을 가지고 있으면 구매 불가
            inGameUIController.ShowNotification("이미 열쇠 아이템을 가지고 있습니다.", 1f); // 2초 동안 문구 표시
            return false;
        }
        return true; // Key 아이템이 없으면 구매 가능
    }

    protected override void ItemEffect()
    {
        if (keySound != null)
        {
            SoundManager.Instance.PlaySFX(keySound.name);
        }
        InventoryManager.Instance.AddItem(ItemType.Key);
        Destroy(gameObject, 0.5f); // 구매 후 0.5초 뒤에 오브젝트 삭제
    }
}

