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
        //상호작용을 막아야하는 상황이 오면 여기다가 그 조건을 넣을 것
    }

    public string GetInteractText() {
        return "열기 (G)";
    }
}
