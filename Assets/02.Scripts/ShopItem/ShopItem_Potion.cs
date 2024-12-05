using UnityEngine;

public class ShopItem_Potion : ShopItemBase {
    [Header("이 포션 구매 시 회복되는 체력 비율")]
    [SerializeField] private int _recoveryAmount = 30;

    [Header("포션 먹는 사운드")]
    [SerializeField] private AudioClip potionSound; // 포션 사운드

    protected override bool CanPurchase() {
        // 플레이어의 현재 체력과 최대 체력을 비교하여 구매 가능 여부 판단
        if (PlayerDataManager.Instance.Status._hp >= PlayerDataManager.Instance.Status._maxHp) {
            SystemMessageManager.Instance.PushSystemMessage(_warningMessage, Color.red);
            return false; // 구매 불가
        }
        return true; // 구매 가능
    }

    protected override void ItemEffect() {
        // 포션 먹는 사운드 재생
        if (potionSound != null) {
            SoundManager.Instance.PlaySFX(potionSound.name);
        }

        PlayerDataManager.Instance.HealByMaxPercent(_recoveryAmount);
        Destroy(gameObject, 0.5f); // 구매 후 0.5초 뒤에 오브젝트 삭제
    }
}


