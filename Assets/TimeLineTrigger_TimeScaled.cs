using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeLineTriggerTimeScaled : TutorialStart
{
    [SerializeField] private CinemachineVirtualCamera targetCamera; // ��ȯ�� Cinemachine Virtual Camera
    [SerializeField] private CinemachineVirtualCamera defaultCamera; // �⺻ Virtual Camera
    [SerializeField] private TimelineManager timelineManager; // Ÿ�Ӷ��� �Ŵ���
    private bool hasTriggered = false; // �÷��̾ Ʈ���Ÿ� �̹� �ߵ��ߴ��� Ȯ��

    private async void OnTriggerEnter2D(Collider2D other) {
        // �÷��̾ ����
        if (!other.CompareTag("Player") || hasTriggered) {
            return;
        }

        hasTriggered = true;

        // ī�޶� ��ȯ
        ActivateCamera(targetCamera);

        await FadeInColor();
        // Ÿ�Ӷ��� �簳
        timelineManager.ResumeTimeline();

        // 2�� ��� �� ���� ī�޶�� ����
        await UniTask.Delay(System.TimeSpan.FromSeconds(2));
        RestoreDefaultCamera();
    }

    private void ActivateCamera(CinemachineVirtualCamera cameraToActivate) {
        if (cameraToActivate != null) {
            foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>()) {
                vcam.Priority = 0; // ��� ī�޶� �켱���� ����
            }

            cameraToActivate.Priority = 10; // Ȱ��ȭ�� ī�޶� �켱���� ����
        }
    }

    /// <summary>
    /// �⺻ ī�޶� ����
    /// </summary>
    private void RestoreDefaultCamera() {
        if (defaultCamera != null) {
            foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>()) {
                vcam.Priority = 0; // ��� ī�޶� �켱���� �ʱ�ȭ
            }

            defaultCamera.Priority = 10; // �⺻ ī�޶� �켱���� ����
        }
    }
}