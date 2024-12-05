/*using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline; // 타임라인 관리
    [SerializeField] private float timelineDuration; // 타임라인 진행 시간

    private bool isTimelineRunning = false; // 타임라인이 실행 중인지 확인
    private bool isTimelinePaused = false; // 타임라인이 멈췄는지 확인

    private void Start()
    {
        if (timeline == null)
        {
            Debug.LogError("TimelineManager: No timeline assigned!");
            return;
        }

        // 타임라인 시작
        PlayTimeline(timelineDuration).Forget();
    }

    /// <summary>
    /// 특정 시간 동안 타임라인을 실행 후 정지
    /// </summary>
    public async UniTask PlayTimeline(float duration)
    {
        if (isTimelineRunning)
        {
            Debug.LogWarning("TimelineManager: Timeline is already running!");
            return;
        }

        isTimelineRunning = true;
        isTimelinePaused = false;
        timeline.gameObject.SetActive(true); // 타임라인 활성화
        timeline.time = 0; // 타임라인 시작 시간 설정
        timeline.Play(); // 타임라인 실행

        // 지정된 시간 동안 대기
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));

        // 타임라인 멈춤
        timeline.Pause();
        timeline.gameObject.SetActive(false); // 타임라인 비활성화
        isTimelineRunning = false;
        isTimelinePaused = true;

        Debug.Log("Timeline paused and player can move now.");
    }

    /// <summary>
    /// 타임라인 재개
    /// </summary>
    public void ResumeTimeline()
    {
        if (!isTimelinePaused)
        {
            Debug.LogWarning("TimelineManager: Timeline is not paused!");
            return;
        }

        isTimelineRunning = true;
        isTimelinePaused = false;

        timeline.gameObject.SetActive(true); // 타임라인 활성화
        timeline.Play(); // 타임라인 재생

        Debug.Log("Timeline resumed.");
    }
}
*/

using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class TimeLineManager : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline; // 타임라인 관리
    [SerializeField] private List<float> timelineDurations; // 타임라인 진행 시간 리스트

    private int currentIndex = 0; // 현재 타임라인 구간 인덱스
    private bool isTimelineRunning = false; // 타임라인이 실행 중인지 확인
    private float lastSavedTime = 0f; // 이전 구간의 종료 시간

    private void Start() {
        if (timeline == null) {
            Debug.LogError("TimelineManager: No timeline assigned!");
            return;
        }

        if (timelineDurations == null || timelineDurations.Count == 0) {
            Debug.LogError("TimelineManager: No timeline durations assigned!");
            return;
        }

        // 첫 번째 구간 실행
        PlayTimeline().Forget();
    }

    /// <summary>
    /// 타임라인 실행 (이전 종료 시간부터 다음 종료 시간까지)
    /// </summary>
    public async UniTask PlayTimeline() {
        if (isTimelineRunning || currentIndex >= timelineDurations.Count) {
            Debug.LogWarning("TimelineManager: No more timeline segments to play!");
            return;
        }

        isTimelineRunning = true;

        float targetTime = timelineDurations[currentIndex]; // 현재 구간의 종료 시간

        timeline.gameObject.SetActive(true); // 타임라인 활성화
        timeline.time = lastSavedTime; // 이전 구간의 종료 시간부터 시작
        timeline.Play();

        // 타임라인이 목표 시간에 도달할 때까지 대기
        while (timeline.time < targetTime) {
            await UniTask.NextFrame();
        }

        // 타임라인 멈춤
        timeline.Pause();
        lastSavedTime = targetTime; // 현재 구간 종료 시간 저장
        isTimelineRunning = false;
        timeline.gameObject.SetActive(false); 
        Debug.Log($"Timeline paused at {targetTime} (Segment {currentIndex}).");
    }

    /// <summary>
    /// 타임라인 재개 (다음 구간 실행)
    /// </summary>
    public void ResumeTimeline() {
        if (isTimelineRunning || currentIndex >= timelineDurations.Count - 1) {
            Debug.LogWarning("TimelineManager: No more timeline segments to resume!");
            return;
        }

        currentIndex++; // 다음 구간으로 이동
        PlayTimeline().Forget();

        Debug.Log($"Timeline resumed to segment {currentIndex}.");
    }
}

