using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void GameQuit() {
        Application.Quit();
    }
}
