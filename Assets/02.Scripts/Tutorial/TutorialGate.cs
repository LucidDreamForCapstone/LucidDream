using UnityEngine;

public class TutorialGate : MonoBehaviour {
    private void OnTriggerStay2D(Collider2D collision) {
        if (Input.GetKey(KeyCode.G)) {
            GameManager.Instance.SaveFlagData();
            GameSceneManager.Instance.LoadStageScene(0);
        }
    }
}
