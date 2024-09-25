using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    public static BGMManager Instance;
    private AudioSource bgmSource;
    private GungeonGameManager g_gameManager; // GungeonGameManager ����

    [SerializeField] public AudioClip defaultBGM;
    [SerializeField] public AudioClip stage4BGM;

    private int currentStage; // ���� Stage ���� ����

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // BGM ����
        }
        else {
            Destroy(gameObject);
            return;
        }

        bgmSource = GetComponent<AudioSource>();
    }

    private void Start() {
        PlayBGM(defaultBGM);
        g_gameManager = FindObjectOfType<GungeonGameManager>(); // GungeonGameManager ã��
        currentStage = g_gameManager.Stage; // ���� Stage �� ����
    }

    private void Update() {
        // Stage ���� �ٲ���� �� BGM�� ����
        if (g_gameManager != null && g_gameManager.Stage != currentStage) {
            currentStage = g_gameManager.Stage; // Stage �� ������Ʈ
            ChangeBGMForStage(); // Stage�� ���� BGM ����
        }
    }

    public void PlayBGM(AudioClip clip) {
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PauseBGM() {
        bgmSource.Pause();
    }

    public void ResumeBGM() {
        bgmSource.UnPause();
    }

    public void StopBGM() {
        bgmSource.Stop();
    }

    public void SetVolume(float volume) {
        bgmSource.volume = volume;
    }

    private void ChangeBGMForStage() {
        // Stage�� 4�� ���� stage4BGM�� ���
        if (currentStage == 4) {
            PlayBGM(stage4BGM);
        }
        else {
            // Stage�� 4�� �ƴ� ��� �⺻ BGM ���
            PlayBGM(defaultBGM);
        }
    }
}


