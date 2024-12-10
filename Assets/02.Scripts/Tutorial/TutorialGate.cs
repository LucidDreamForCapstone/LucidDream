using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialGate : MonoBehaviour {
    private void OnTriggerStay2D(Collider2D collision) {
        if (Input.GetKey(KeyCode.G)) {
            MoveToDungeon();
        }
    }

    private void MoveToDungeon() {
        SceneManager.LoadScene("04.Dungeon");
    }
}
