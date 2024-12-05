using Edgar.Unity;
using Edgar.Unity.Examples.Gungeon;
using UnityEngine;
using UnityEngine.Rendering;

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

    [SerializeField]
    private Volume postProcessingVolume; // ����Ʈ ���μ��� ����

    private FogOfWarGrid2D fogComponent; // FogOfWarGrid2D ������Ʈ
    private bool hasExecutedTimeline = false; // Ÿ�Ӷ��� ���� ���� �÷���
    //private ColorAdjustments colorAdjustments; // ColorAdjustments ������Ʈ

    private void Start() {
        if (fogObject != null) {
            fogComponent = fogObject.GetComponent<FogOfWarGrid2D>();
        }

        /*
        // ColorAdjustments ��������
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustmentsComponent)) {
            colorAdjustments = colorAdjustmentsComponent;
        }
        else {
            Debug.LogError("ColorAdjustments component not found in the Volume!");
        }
        */
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

        /*
        // Hue Shift�� Saturation �� ����
        if (colorAdjustments != null) {
            colorAdjustments.hueShift.value = 179f; // Hue Shift ����
            colorAdjustments.saturation.value = -68f; // Saturation ����
            Debug.Log("Color adjustments applied: Hue Shift = 179, Saturation = -68");
        }
        else {
            Debug.LogError("ColorAdjustments component is not available!");
        }
        */
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
}
