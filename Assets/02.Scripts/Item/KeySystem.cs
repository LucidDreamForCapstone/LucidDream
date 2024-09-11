using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class KeySystem : MonoBehaviour, Interactable {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && InteractManager.Instance.CheckInteractable(this)) {
            if (GungeonGameManager.Instance != null) {
                //GungeonDoor.
                Debug.Log("isGenerating set to true");
            }
            else {
                Debug.LogError("GGM instance is null");
            }
        }
    }

    public bool IsInteractBlock() {
        return false;
        //��ȣ�ۿ��� ���ƾ��ϴ� ��Ȳ�� ���� ����ٰ� �� ������ ���� ��
    }

    public string GetInteractText() {
        return "���� (G)";
    }
}
