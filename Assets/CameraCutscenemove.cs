using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraSwitchTrigger : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> targetCameras; // 전환할 가상 카메라 리스트
    [SerializeField] private CinemachineVirtualCamera defaultCamera; // 기본 카메라
    [SerializeField] private float switchDuration = 3f; // 각 카메라 전환 지속 시간
    private bool hasTriggered = false; // 최초 1회 실행 방지 플래그

    private void OnTriggerEnter2D(Collider2D collision) {
        // 플레이어가 트리거에 닿았는지 확인
        if (!hasTriggered && collision.CompareTag("Player")) {
            hasTriggered = true; // 최초 1회 실행 방지
            SwitchCamerasAsync().Forget(); // UniTask 실행
        }
    }

    private async UniTaskVoid SwitchCamerasAsync() {
        // 각 Target 카메라를 순서대로 활성화
        foreach (var camera in targetCameras) {
            // 타겟 카메라 활성화
            camera.Priority = 10;
            defaultCamera.Priority = 0;

            // 지정된 시간 동안 대기
            await UniTask.Delay((int)(switchDuration * 1000), ignoreTimeScale: false);

            // 타겟 카메라 비활성화
            camera.Priority = 0;
        }

        // 모든 Target 카메라 전환 후 Default 카메라로 복귀
        defaultCamera.Priority = 10;
    }

    public void CameraMoveSignal() {
        SwitchCamerasAsync().Forget();
    }
}