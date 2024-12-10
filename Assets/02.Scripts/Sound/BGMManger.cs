using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    public static BGMManager Instance;
    private GungeonGameManager g_gameManager; // GungeonGameManager 참조

    [SerializeField] public AudioClip defaultBGM;
    [SerializeField] public AudioClip stage4BGM;

    private int currentStage; // 현재 Stage 값을 저장

    private void Start() {
        SoundManager.Instance.PlayBGM(defaultBGM.name);
        g_gameManager = FindObjectOfType<GungeonGameManager>(); // GungeonGameManager 찾기
        currentStage = g_gameManager.Stage; // 현재 Stage 값 저장
    }

    private void Update() {
        // Stage 값이 바뀌었을 때 BGM을 변경
        if (g_gameManager != null && g_gameManager.Stage != currentStage) {
            currentStage = g_gameManager.Stage; // Stage 값 업데이트
            ChangeBGMForStage(); // Stage에 따라 BGM 변경
        }
    }

    private void ChangeBGMForStage() {
        // Stage가 4일 때는 stage4BGM을 재생
        if (currentStage == 4) {
            SoundManager.Instance.PlayBGM(stage4BGM.name);
        }
        else {
            // Stage가 4가 아닌 경우 기본 BGM 재생
            SoundManager.Instance.PlayBGM(defaultBGM.name);
        }
    }
}


