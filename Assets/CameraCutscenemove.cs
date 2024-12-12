using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraSwitchTrigger : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> targetCameras; // ��ȯ�� ���� ī�޶� ����Ʈ
    [SerializeField] private CinemachineVirtualCamera defaultCamera; // �⺻ ī�޶�
    [SerializeField] private float switchDuration = 3f; // �� ī�޶� ��ȯ ���� �ð�
    private bool hasTriggered = false; // ���� 1ȸ ���� ���� �÷���

    private void OnTriggerEnter2D(Collider2D collision) {
        // �÷��̾ Ʈ���ſ� ��Ҵ��� Ȯ��
        if (!hasTriggered && collision.CompareTag("Player")) {
            hasTriggered = true; // ���� 1ȸ ���� ����
            SwitchCamerasAsync().Forget(); // UniTask ����
        }
    }

    private async UniTaskVoid SwitchCamerasAsync() {
        // �� Target ī�޶� ������� Ȱ��ȭ
        foreach (var camera in targetCameras) {
            // Ÿ�� ī�޶� Ȱ��ȭ
            camera.Priority = 10;
            defaultCamera.Priority = 0;

            // ������ �ð� ���� ���
            await UniTask.Delay((int)(switchDuration * 1000), ignoreTimeScale: false);

            // Ÿ�� ī�޶� ��Ȱ��ȭ
            camera.Priority = 0;
        }

        // ��� Target ī�޶� ��ȯ �� Default ī�޶�� ����
        defaultCamera.Priority = 10;
    }

    public void CameraMoveSignal() {
        SwitchCamerasAsync().Forget();
    }
}