using UnityEngine;

public class ShopItem_Potion : ShopItemBase {
    [Header("�� ���� ���� �� ȸ���Ǵ� ü�� ����")]
    [SerializeField] private int _recoveryAmount = 30;

    [Header("���� �Դ� ����")]
    [SerializeField] private AudioClip potionSound; // ���� ����

    protected override bool CanPurchase() {
        // �÷��̾��� ���� ü�°� �ִ� ü���� ���Ͽ� ���� ���� ���� �Ǵ�
        if (PlayerDataManager.Instance.Status._hp >= PlayerDataManager.Instance.Status._maxHp) {
            SystemMessageManager.Instance.PushSystemMessage(_warningMessage, Color.red);
            return false; // ���� �Ұ�
        }
        return true; // ���� ����
    }

    protected override void ItemEffect() {
        // ���� �Դ� ���� ���
        if (potionSound != null) {
            SoundManager.Instance.PlaySFX(potionSound.name);
        }

        PlayerDataManager.Instance.HealByMaxPercent(_recoveryAmount);
        Destroy(gameObject, 0.5f); // ���� �� 0.5�� �ڿ� ������Ʈ ����
    }
}


