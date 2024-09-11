using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance;
    private AudioSource bgmSource;

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

        bgmSource = GetComponent<AudioSource>();
    }

    private void Start() {
        PlayBGM(bgmClip);
        SceneManager.sceneLoaded += OnSceneLoaded;  // 씬 로드 시 이벤트 등록
    }

    public void PlayBGM(AudioClip clip) {
        if (bgmSource.clip == clip) return; // 이미 같은 BGM이 재생 중일 경우 무시
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
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
