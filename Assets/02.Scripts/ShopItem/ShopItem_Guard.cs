using UnityEngine;

public class ShopItem_Guard : ShopItemBase {
    [SerializeField] private AudioClip guardSound; // 가드 사운드

    protected override bool CanPurchase() {
        // 플레이어가 이미 Guard 아이템을 가지고 있는지 확인
        if (InventoryManager.Instance.HasItem(ItemType.Guard)) {
            // 이미 Guard 아이템을 가지고 있으면 구매 불가
            SystemMessageManager.Instance.PushSystemMessage(_warningMessage, Color.red);
            return false;
        }
        return true; // Guard 아이템이 없으면 구매 가능
    }

    protected override void ItemEffect() {
        // Guard 아이템 추가
        InventoryManager.Instance.AddItem(ItemType.Guard);
        if (guardSound != null) {
            SoundManager.Instance.PlaySFX(guardSound.name);
        }
        inGameUIController.ShowGuard();
        Destroy(gameObject, 0.5f); // 구매 후 0.5초 뒤에 오브젝트 삭제
    }
}



