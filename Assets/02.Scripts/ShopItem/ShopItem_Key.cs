using UnityEngine;

public class ShopItem_Key : ShopItemBase {
    [Header("���� ������ ����")]
    [SerializeField] private AudioClip keySound; // ���� ����
    protected override bool CanPurchase() {
        // �÷��̾ �̹� Key �������� ������ �ִ��� Ȯ��
        if (InventoryManager.Instance.HasItem(ItemType.Key)) {
            // �̹� Key �������� ������ ������ ���� �Ұ�
            SystemMessageManager.Instance.PushSystemMessage(_warningMessage, Color.red);
            return false;
        }
        return true; // Key �������� ������ ���� ����
    }

    protected override void ItemEffect() {
        if (keySound != null) {
            SoundManager.Instance.PlaySFX(keySound.name);
        }
        InventoryManager.Instance.AddItem(ItemType.Key);
        Destroy(gameObject, 0.5f); // ���� �� 0.5�� �ڿ� ������Ʈ ����
    }
}

