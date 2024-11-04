using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance;
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
    }

    private void Start() {
        SoundManager.Instance.PlayBGM(bgmClip.name);
        SceneManager.sceneLoaded += OnSceneLoaded;  // �� �ε� �� �̺�Ʈ ���
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
