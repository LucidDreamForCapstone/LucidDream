using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class WarpPoint_Dungeon : MonoBehaviour, Interactable {
    // Start is called before the first frame update
    void Start() {

    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && InteractManager.Instance.CheckInteractable(this)) {
            if (GungeonGameManager.Instance != null) {
                GungeonGameManager.Instance.SetIsGenerating(true);
                Debug.Log("isGenerating set to true");

            }
            else {
                Debug.LogError("GGM instance is null");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (GungeonGameManager.Instance != null) {
                GungeonGameManager.Instance.SetIsGenerating(false);
                Debug.Log("isGenerating set to false");
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
        return "이동 (G)";
    }
}
