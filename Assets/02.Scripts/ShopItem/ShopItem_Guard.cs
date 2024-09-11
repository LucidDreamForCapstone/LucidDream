using UnityEngine;

public class ShopItem_Guard : ShopItemBase
{
    [SerializeField] private AudioClip guardSound; // ���� ����

    protected override bool CanPurchase()
    {
        // �÷��̾ �̹� Guard �������� ������ �ִ��� Ȯ��
        if (InventoryManager.Instance.HasItem(ItemType.Guard))
        {
            // �̹� Guard �������� ������ ������ ���� �Ұ�
            inGameUIController.ShowNotification("�̹� ��ȣ�� ��� �������� ������ �ֽ��ϴ�.", 1f); // 2�� ���� ���� ǥ��
            return false;
        }
        return true; // Guard �������� ������ ���� ����
    }

    protected override void ItemEffect()
    {
        // Guard ������ �߰�
        InventoryManager.Instance.AddItem(ItemType.Guard);
        if (guardSound != null)
        {
            SoundManager.Instance.PlaySFX(guardSound.name);
        }
        inGameUIController.ShowGuard();
        Destroy(gameObject, 0.5f); // ���� �� 0.5�� �ڿ� ������Ʈ ����
    }
}



