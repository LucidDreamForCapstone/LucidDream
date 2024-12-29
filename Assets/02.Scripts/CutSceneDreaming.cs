using Edgar.Unity;
using Edgar.Unity.Examples.Gungeon;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CutSceneDreaming : MonoBehaviour {
    [SerializeField] private TimeLineManager timelineManager; // Timeline Manager

    [SerializeField]
    private Transform player; // �÷��̾� ������Ʈ

    [SerializeField]
    private Transform startPoint; // �÷��̾ �̵���ų ���� ��ġ

    [SerializeField]
    private GungeonGameManager dungeonGameManager; // DungeonGameManager ����

    [SerializeField]
    private GameObject fogObject; // FogOfWarGrid2D�� ����� ������Ʈ

    [SerializeField] Volume colorVolume;
    private FogOfWarGrid2D fogComponent; // FogOfWarGrid2D ������Ʈ
    private bool hasExecutedTimeline = false; // Ÿ�Ӷ��� ���� ���� �÷���
    private ColorAdjustments colorAdjustments; // ColorAdjustments ������Ʈ

    private void Start() {
        if (fogObject != null) {
            fogComponent = fogObject.GetComponent<FogOfWarGrid2D>();
        }
        // ColorAdjustments ��������
        if (colorVolume != null && colorVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustmentsComponent)) {
            colorAdjustments = colorAdjustmentsComponent;
        }
        else {
            Debug.LogError("No color volume component or wrong volume component");
        }
    }

    private void Update() {
        // DungeonGameManager���� stage ���� ������ 2���� Ȯ��
        if (dungeonGameManager != null && dungeonGameManager.Stage == 2 && !hasExecutedTimeline) {
            hasExecutedTimeline = true; // �÷��׸� �����Ͽ� 1���� ����
            ExecuteTimeline();
        }
    }

    /// <summary>
    /// Ÿ�Ӷ��� ���� ����
    /// </summary>
    private void ExecuteTimeline() {
        if (fogComponent != null) {
            fogComponent.enabled = false; // Fog ��Ȱ��ȭ
        }

        MovePlayerToStartPoint(); // �÷��̾ ���� �������� �̵�

        if (timelineManager != null) {
            timelineManager.ResumeTimeline(); // Ÿ�Ӷ��� ����
        }
        else {
            Debug.LogError("Timeline Manager is not assigned!");
        }
        ChangeHueShift();
    }

    /// <summary>
    /// �÷��̾ ���� �������� �̵�
    /// </summary>
    private void MovePlayerToStartPoint() {
        if (player != null && startPoint != null) {
            player.position = startPoint.position;
            player.rotation = startPoint.rotation;
        }
        else {
            Debug.LogError("Player or StartPoint is not assigned!");
        }
    }


    public void ChangeHueShift() {
        // Hue Shift�� Saturation �� ����
        if (colorAdjustments != null) {
            colorAdjustments.hueShift.value = 27f; // Hue Shift ����
            colorAdjustments.saturation.value = -68f; // Saturation ����
        }
    }

    public void ChangeHueShiftReset() {
        if (colorAdjustments != null) {
            colorAdjustments.hueShift.value = 0f; // Hue Shift ����
            colorAdjustments.saturation.value = 0f; // Saturation ����
        }
    }
}
