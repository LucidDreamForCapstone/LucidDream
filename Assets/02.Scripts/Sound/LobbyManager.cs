using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance;
    [SerializeField] private AudioClip bgmClip;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 파괴
            return;
        }
    }

    private void Start() {
        SoundManager.Instance.PlayBGM(bgmClip.name);
        SceneManager.sceneLoaded += OnSceneLoaded;  // 씬 로드 시 이벤트 등록
    }

    // 씬이 로드될 때 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // 04.Dungeon 씬에서는 파괴
        if (scene.name == "04.Dungeon") {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 제거
    }
}
