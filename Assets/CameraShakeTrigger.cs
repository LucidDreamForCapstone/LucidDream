using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class CameraShakeTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    // ��鸲 �����͸� �����ϴ� ����Ʈ
    private List<ShakeData> shakeDataList;

    private void Awake() {
        // ��鸲 ������ �ʱ�ȭ
        shakeDataList = new List<ShakeData>
        {
            new ShakeData { RepeatCount = 30, Interval = 0.1f },
            new ShakeData { RepeatCount = 5, Interval = 0.2f },
            new ShakeData { RepeatCount = 7, Interval = 0.05f }
        };
    }

    /// <summary>
    /// ��鸲 �����͸� ������� ��鸲 ȿ�� ����
    /// </summary>
    public async UniTaskVoid TriggerShakeAsync(int index) {
        if (impulseSource == null) {
            Debug.LogWarning("ImpulseSource�� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (index < 0 || index >= shakeDataList.Count) {
            Debug.LogError($"�߸��� index ��: {index}. ��� ������ 0���� {shakeDataList.Count - 1}�Դϴ�.");
            return;
        }

        // index�� �ش��ϴ� ��鸲 ������ ��������
        ShakeData shakeData = shakeDataList[index];
        for (int i = 0; i < shakeData.RepeatCount; i++) {
            // Force �� ���� (���� ��� �߰� ����)
            Vector3 customForce = new Vector3(Random.Range(-1f, 1f), Random.Range(0.3f, 0.5f), 0);

            // ��鸲 ����
            impulseSource.GenerateImpulse(customForce);

            // ������ ���ݸ�ŭ ���
            await UniTask.Delay(System.TimeSpan.FromSeconds(shakeData.Interval), ignoreTimeScale: true);
        }
    }

    /// <summary>
    /// Signal Emitter�� ���� ȣ��Ǵ� �Լ�
    /// </summary>
    public void TriggerShakeByIndex(int index) {
        Debug.Log($"TriggerShakeByIndex called with index: {index}");
        TriggerShakeAsync(index).Forget();
    }

    /// <summary>
    /// ��鸲 �����͸� �߰��� �� �ִ� �Լ� (�ʿ� �� ���)
    /// </summary>
    public void AddShakeData(int repeatCount, float interval) {
        shakeDataList.Add(new ShakeData { RepeatCount = repeatCount, Interval = interval });
    }

    // ��鸲 ������ ����ü
    private struct ShakeData
    {
        public int RepeatCount; // ��鸲 �ݺ� Ƚ��
        public float Interval;  // �� ��鸲 �� ����
    }
}
