/*using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline; // Ÿ�Ӷ��� ����
    [SerializeField] private float timelineDuration; // Ÿ�Ӷ��� ���� �ð�

    private bool isTimelineRunning = false; // Ÿ�Ӷ����� ���� ������ Ȯ��
    private bool isTimelinePaused = false; // Ÿ�Ӷ����� ������� Ȯ��

    private void Start()
    {
        if (timeline == null)
        {
            Debug.LogError("TimelineManager: No timeline assigned!");
            return;
        }

        // Ÿ�Ӷ��� ����
        PlayTimeline(timelineDuration).Forget();
    }

    /// <summary>
    /// Ư�� �ð� ���� Ÿ�Ӷ����� ���� �� ����
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
        timeline.gameObject.SetActive(true); // Ÿ�Ӷ��� Ȱ��ȭ
        timeline.time = 0; // Ÿ�Ӷ��� ���� �ð� ����
        timeline.Play(); // Ÿ�Ӷ��� ����

        // ������ �ð� ���� ���
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));

        // Ÿ�Ӷ��� ����
        timeline.Pause();
        timeline.gameObject.SetActive(false); // Ÿ�Ӷ��� ��Ȱ��ȭ
        isTimelineRunning = false;
        isTimelinePaused = true;

        Debug.Log("Timeline paused and player can move now.");
    }

    /// <summary>
    /// Ÿ�Ӷ��� �簳
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

        timeline.gameObject.SetActive(true); // Ÿ�Ӷ��� Ȱ��ȭ
        timeline.Play(); // Ÿ�Ӷ��� ���

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
    [SerializeField] private PlayableDirector timeline; // Ÿ�Ӷ��� ����
    [SerializeField] private List<float> timelineDurations; // Ÿ�Ӷ��� ���� �ð� ����Ʈ

    private int currentIndex = 0; // ���� Ÿ�Ӷ��� ���� �ε���
    private bool isTimelineRunning = false; // Ÿ�Ӷ����� ���� ������ Ȯ��
    private float lastSavedTime = 0f; // ���� ������ ���� �ð�

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

        float targetTime = timelineDurations[currentIndex]; // ���� ������ ���� �ð�

        timeline.gameObject.SetActive(true); // Ÿ�Ӷ��� Ȱ��ȭ
        timeline.time = lastSavedTime; // ���� ������ ���� �ð����� ����
        timeline.Play();

        // Ÿ�Ӷ����� ��ǥ �ð��� ������ ������ ���
        while (timeline.time < targetTime) {
            await UniTask.NextFrame();
        }

        // Ÿ�Ӷ��� ����
        timeline.Pause();
        lastSavedTime = targetTime; // ���� ���� ���� �ð� ����
        isTimelineRunning = false;
        timeline.gameObject.SetActive(false); 
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
}

