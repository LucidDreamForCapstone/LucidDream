using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake() {
        _instance = this;
    }

    public void GameQuit() {
        Application.Quit();
    }
}
