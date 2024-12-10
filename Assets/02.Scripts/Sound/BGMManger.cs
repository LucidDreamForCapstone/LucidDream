using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    public static BGMManager Instance;
    private GungeonGameManager g_gameManager; // GungeonGameManager ����

    [SerializeField] public AudioClip defaultBGM;
    [SerializeField] public AudioClip stage4BGM;

    private int currentStage; // ���� Stage ���� ����

    private void Start() {
        SoundManager.Instance.PlayBGM(defaultBGM.name);
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

    private void ChangeBGMForStage() {
        // Stage�� 4�� ���� stage4BGM�� ���
        if (currentStage == 4) {
            SoundManager.Instance.PlayBGM(stage4BGM.name);
        }
        else {
            // Stage�� 4�� �ƴ� ��� �⺻ BGM ���
            SoundManager.Instance.PlayBGM(defaultBGM.name);
        }
    }
}


