using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class CameraShakeTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    // 흔들림 데이터를 저장하는 리스트
    private List<ShakeData> shakeDataList;

    private void Awake() {
        // 흔들림 데이터 초기화
        shakeDataList = new List<ShakeData>
        {
            new ShakeData { RepeatCount = 30, Interval = 0.1f },
            new ShakeData { RepeatCount = 5, Interval = 0.2f },
            new ShakeData { RepeatCount = 7, Interval = 0.05f }
        };
    }

    /// <summary>
    /// 흔들림 데이터를 기반으로 흔들림 효과 생성
    /// </summary>
    public async UniTaskVoid TriggerShakeAsync(int index) {
        if (impulseSource == null) {
            Debug.LogWarning("ImpulseSource가 설정되지 않았습니다.");
            return;
        }

        if (index < 0 || index >= shakeDataList.Count) {
            Debug.LogError($"잘못된 index 값: {index}. 허용 범위는 0에서 {shakeDataList.Count - 1}입니다.");
            return;
        }

        // index에 해당하는 흔들림 데이터 가져오기
        ShakeData shakeData = shakeDataList[index];
        for (int i = 0; i < shakeData.RepeatCount; i++) {
            // Force 값 생성 (랜덤 요소 추가 가능)
            Vector3 customForce = new Vector3(Random.Range(-1f, 1f), Random.Range(0.3f, 0.5f), 0);

            // 흔들림 생성
            impulseSource.GenerateImpulse(customForce);

            // 지정된 간격만큼 대기
            await UniTask.Delay(System.TimeSpan.FromSeconds(shakeData.Interval), ignoreTimeScale: true);
        }
    }

    /// <summary>
    /// Signal Emitter를 통해 호출되는 함수
    /// </summary>
    public void TriggerShakeByIndex(int index) {
        Debug.Log($"TriggerShakeByIndex called with index: {index}");
        TriggerShakeAsync(index).Forget();
    }

    /// <summary>
    /// 흔들림 데이터를 추가할 수 있는 함수 (필요 시 사용)
    /// </summary>
    public void AddShakeData(int repeatCount, float interval) {
        shakeDataList.Add(new ShakeData { RepeatCount = repeatCount, Interval = interval });
    }

    // 흔들림 데이터 구조체
    private struct ShakeData
    {
        public int RepeatCount; // 흔들림 반복 횟수
        public float Interval;  // 각 흔들림 간 간격
    }
}
