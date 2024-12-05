using Edgar.Unity;
using Edgar.Unity.Examples.Gungeon;
using UnityEngine;
using UnityEngine.Rendering;

public class CutSceneDreaming : MonoBehaviour {
    [SerializeField] private TimeLineManager timelineManager; // Timeline Manager

    [SerializeField]
    private Transform player; // 플레이어 오브젝트

    [SerializeField]
    private Transform startPoint; // 플레이어를 이동시킬 시작 위치

    [SerializeField]
    private GungeonGameManager dungeonGameManager; // DungeonGameManager 참조

    [SerializeField]
    private GameObject fogObject; // FogOfWarGrid2D가 연결된 오브젝트

    [SerializeField]
    private Volume postProcessingVolume; // 포스트 프로세싱 볼륨

    private FogOfWarGrid2D fogComponent; // FogOfWarGrid2D 컴포넌트
    private bool hasExecutedTimeline = false; // 타임라인 실행 여부 플래그
    //private ColorAdjustments colorAdjustments; // ColorAdjustments 컴포넌트

    private void Start() {
        if (fogObject != null) {
            fogComponent = fogObject.GetComponent<FogOfWarGrid2D>();
        }

        /*
        // ColorAdjustments 가져오기
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustmentsComponent)) {
            colorAdjustments = colorAdjustmentsComponent;
        }
        else {
            Debug.LogError("ColorAdjustments component not found in the Volume!");
        }
        */
    }

    private void Update() {
        // DungeonGameManager에서 stage 값을 가져와 2인지 확인
        if (dungeonGameManager != null && dungeonGameManager.Stage == 2 && !hasExecutedTimeline) {
            hasExecutedTimeline = true; // 플래그를 설정하여 1번만 실행
            ExecuteTimeline();
        }
    }

    /// <summary>
    /// 타임라인 실행 로직
    /// </summary>
    private void ExecuteTimeline() {
        if (fogComponent != null) {
            fogComponent.enabled = false; // Fog 비활성화
        }

        MovePlayerToStartPoint(); // 플레이어를 시작 지점으로 이동

        if (timelineManager != null) {
            timelineManager.ResumeTimeline(); // 타임라인 실행
        }
        else {
            Debug.LogError("Timeline Manager is not assigned!");
        }

        /*
        // Hue Shift와 Saturation 값 변경
        if (colorAdjustments != null) {
            colorAdjustments.hueShift.value = 179f; // Hue Shift 설정
            colorAdjustments.saturation.value = -68f; // Saturation 설정
            Debug.Log("Color adjustments applied: Hue Shift = 179, Saturation = -68");
        }
        else {
            Debug.LogError("ColorAdjustments component is not available!");
        }
        */
    }

    /// <summary>
    /// 플레이어를 시작 지점으로 이동
    /// </summary>
    private void MovePlayerToStartPoint() {
        if (player != null && startPoint != null) {
            player.position = startPoint.position;
            player.rotation = startPoint.rotation;
        }
        else {
            Debug.LogError("Player or StartPoint is not assigned!");
        }
    }
}
