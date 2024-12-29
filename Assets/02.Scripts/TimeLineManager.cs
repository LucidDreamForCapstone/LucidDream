using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour {
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline; // 타임라인 관리
    [SerializeField] private List<float> timelineDurations; // 타임라인 진행 시간 리스트
    [SerializeField] private Canvas player1UICanvas; // Player1 UI Canvas
    [SerializeField] private Canvas player2UICanvas; // Player2 UI Canvas
    [SerializeField] private PlayerSwapManager playerSwapManager; // PlayerSwapManager 참조
    private int currentIndex = 0; // 현재 타임라인 구간 인덱스
    private bool isTimelineRunning = false; // 타임라인이 실행 중인지 확인
    private float lastSavedTime = 0f; // 이전 구간의 종료 시간
    [SerializeField] private Player _player;
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
        playerSwapManager.SetCanSwap(false); // 타임라인 중 글리치 및 스왑 불가능
        float targetTime = timelineDurations[currentIndex]; // 현재 구간의 종료 시간
        _player.PlayerPause();
        SwitchToPlayer2UI(); // Player2 UI로 전환
        timeline.gameObject.SetActive(true); // 타임라인 활성화
        timeline.time = lastSavedTime; // 이전 구간의 종료 시간부터 시작
        timeline.Play();

        // 타임라인이 목표 시간에 도달할 때까지 대기
        while (timeline.time < targetTime) {
            await UniTask.NextFrame();
        }

        // 타임라인 멈춤
        timeline.Pause();
        _player.PlayerUnPause();
        lastSavedTime = targetTime; // 현재 구간 종료 시간 저장
        isTimelineRunning = false;
        timeline.gameObject.SetActive(false);
        SwitchToPlayer1UI(); // Player1 UI로 다시 전환
        playerSwapManager.SetCanSwap(true); // 타임라인 종료 후 글리치 및 스왑 가능
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

    /// <summary>
    /// Player2 UI로 전환
    /// </summary>
    public void SwitchToPlayer2UI() {
        player1UICanvas.targetDisplay = 1;
        player2UICanvas.targetDisplay = 0;
        Debug.Log("Switched to Player2 UI.");
    }

    /// <summary>
    /// Player1 UI로 전환
    /// </summary>
    public void SwitchToPlayer1UI() {
        player1UICanvas.targetDisplay = 0;
        player2UICanvas.targetDisplay = 1;
        Debug.Log("Switched back to Player1 UI.");
    }
}

