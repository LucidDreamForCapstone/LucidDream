using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour {
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline; // Ÿ�Ӷ��� ����
    [SerializeField] private List<float> timelineDurations; // Ÿ�Ӷ��� ���� �ð� ����Ʈ
    [SerializeField] private Canvas player1UICanvas; // Player1 UI Canvas
    [SerializeField] private Canvas player2UICanvas; // Player2 UI Canvas
    [SerializeField] private PlayerSwapManager playerSwapManager; // PlayerSwapManager ����
    private int currentIndex = 0; // ���� Ÿ�Ӷ��� ���� �ε���
    private bool isTimelineRunning = false; // Ÿ�Ӷ����� ���� ������ Ȯ��
    private float lastSavedTime = 0f; // ���� ������ ���� �ð�
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

        // ù ��° ���� ����
        PlayTimeline().Forget();
    }

    /// <summary>
    /// Ÿ�Ӷ��� ���� (���� ���� �ð����� ���� ���� �ð�����)
    /// </summary>
    public async UniTask PlayTimeline() {
        if (isTimelineRunning || currentIndex >= timelineDurations.Count) {
            Debug.LogWarning("TimelineManager: No more timeline segments to play!");
            return;
        }

        isTimelineRunning = true;
        playerSwapManager.SetCanSwap(false); // Ÿ�Ӷ��� �� �۸�ġ �� ���� �Ұ���
        float targetTime = timelineDurations[currentIndex]; // ���� ������ ���� �ð�
        _player.PlayerPause();
        SwitchToPlayer2UI(); // Player2 UI�� ��ȯ
        timeline.gameObject.SetActive(true); // Ÿ�Ӷ��� Ȱ��ȭ
        timeline.time = lastSavedTime; // ���� ������ ���� �ð����� ����
        timeline.Play();

        // Ÿ�Ӷ����� ��ǥ �ð��� ������ ������ ���
        while (timeline.time < targetTime) {
            await UniTask.NextFrame();
        }

        // Ÿ�Ӷ��� ����
        timeline.Pause();
        _player.PlayerUnPause();
        lastSavedTime = targetTime; // ���� ���� ���� �ð� ����
        isTimelineRunning = false;
        timeline.gameObject.SetActive(false);
        SwitchToPlayer1UI(); // Player1 UI�� �ٽ� ��ȯ
        playerSwapManager.SetCanSwap(true); // Ÿ�Ӷ��� ���� �� �۸�ġ �� ���� ����
        Debug.Log($"Timeline paused at {targetTime} (Segment {currentIndex}).");
    }

    /// <summary>
    /// Ÿ�Ӷ��� �簳 (���� ���� ����)  
    /// </summary>
    public void ResumeTimeline() {

        if (isTimelineRunning || currentIndex >= timelineDurations.Count - 1) {
            Debug.LogWarning("TimelineManager: No more timeline segments to resume!");
            return;
        }

        currentIndex++; // ���� �������� �̵�
        PlayTimeline().Forget();

        Debug.Log($"Timeline resumed to segment {currentIndex}.");
    }

    /// <summary>
    /// Player2 UI�� ��ȯ
    /// </summary>
    public void SwitchToPlayer2UI() {
        player1UICanvas.targetDisplay = 1;
        player2UICanvas.targetDisplay = 0;
        Debug.Log("Switched to Player2 UI.");
    }

    /// <summary>
    /// Player1 UI�� ��ȯ
    /// </summary>
    public void SwitchToPlayer1UI() {
        player1UICanvas.targetDisplay = 0;
        player2UICanvas.targetDisplay = 1;
        Debug.Log("Switched back to Player1 UI.");
    }
}

