using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeLineTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera targetCamera; // 전환할 Cinemachine Virtual Camera
    [SerializeField] private CinemachineVirtualCamera defaultCamera; // 기본 Virtual Camera
    [SerializeField] private TimelineManager timelineManager; // 타임라인 매니저
    private bool hasTriggered = false; // 플레이어가 트리거를 이미 발동했는지 확인

    private async void OnTriggerEnter2D(Collider2D other) {
        // 플레이어만 감지
        if (!other.CompareTag("Player") || hasTriggered) {
            return;
        }

        hasTriggered = true;

        // 타임라인 재개
        timelineManager.ResumeTimeline();
        // 2초 대기 후 원래 카메라로 복구
        await UniTask.Delay(System.TimeSpan.FromSeconds(2));
        RestoreDefaultCamera();
        // 트리거 재사용 가능하도록 리셋 (필요 시)
        hasTriggered = false;
    }

    /// <summary>
    /// 특정 카메라 활성화
    /// </summary>
    private void ActivateCamera(CinemachineVirtualCamera cameraToActivate) {
        if (cameraToActivate != null) {
            foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>()) {
                vcam.Priority = 0; // 모든 카메라 우선순위 낮춤
            }

            cameraToActivate.Priority = 10; // 활성화할 카메라 우선순위 높임
        }
    }

    /// <summary>
    /// 기본 카메라 복구
    /// </summary>
    private void RestoreDefaultCamera() {
        if (defaultCamera != null) {
            foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>()) {
                vcam.Priority = 0; // 모든 카메라 우선순위 초기화
            }

            defaultCamera.Priority = 10; // 기본 카메라 우선순위 복구
        }
    }
}
