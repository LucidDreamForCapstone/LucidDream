using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance;
    private AudioSource bgmSource;

    [SerializeField] private AudioClip bgmClip;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� �ʵ��� ����
        }
        else {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ı�
            return;
        }

        bgmSource = GetComponent<AudioSource>();
    }

    private void Start() {
        PlayBGM(bgmClip);
        SceneManager.sceneLoaded += OnSceneLoaded;  // �� �ε� �� �̺�Ʈ ���
    }

    public void PlayBGM(AudioClip clip) {
        if (bgmSource.clip == clip) return; // �̹� ���� BGM�� ��� ���� ��� ����
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // ���� �ε�� �� ȣ��Ǵ� �޼���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // 04.Dungeon �������� �ı�
        if (scene.name == "04.Dungeon") {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �� �ε� �̺�Ʈ ����
    }
}
