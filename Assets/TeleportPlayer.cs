using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject target;
    [SerializeField] private Volume postProcessingVolume; // Post-Processing Volume
    private ColorAdjustments colorAdjustments; // ColorAdjustments ������Ʈ
    /// <summary>
    /// �÷��̾ Ư�� ������Ʈ ��ġ�� ���� �̵���ŵ�ϴ�.
    /// </summary>
    /// <param name="player">�̵��� �÷��̾� ������Ʈ</param>
    /// <param name="targetObject">��ǥ ��ġ�� ���� ������Ʈ</param>
    /// 
    private void Start() {
        // ColorAdjustments ������Ʈ ��������
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out ColorAdjustments adjustments)) {
            colorAdjustments = adjustments;
        }
        else {
            Debug.LogError("ColorAdjustments component not found in the Volume profile!");
        }
    }
    public void TeleportPlayerToTarget(GameObject player, GameObject targetObject) {
        if (player == null) {
            Debug.LogError("Player object is null!");
            return;
        }

        if (targetObject == null) {
            Debug.LogError("Target object is null!");
            return;
        }

        // �÷��̾��� Transform ��������
        Transform playerTransform = player.transform;

        // �̵��� ��ǥ Transform ��������
        Transform targetTransform = targetObject.transform;

        // �÷��̾� ��ġ �� ȸ�� ����
        playerTransform.position = targetTransform.position;
        playerTransform.rotation = targetTransform.rotation;

        Debug.Log($"Player {player.name} teleported to {targetObject.name} at position {targetTransform.position}");

        // ColorAdjustments ����
        if (colorAdjustments != null) {
            colorAdjustments.hueShift.value = 0f; // Hue Shift 0���� ����
            colorAdjustments.saturation.value = 0f; // Saturation 0���� ����
            Debug.Log("ColorAdjustments: Hue Shift and Saturation set to 0.");
        }
        else {
            Debug.LogError("ColorAdjustments component is not available!");
        }
    }

    /// <summary>
    /// Signal���� ȣ��Ǵ� �ڷ���Ʈ �Լ�
    /// </summary>
    public void SignalTP() {
        // ���̾��Ű���� "ResponsePoint"��� �̸��� ������Ʈ�� �˻�
        GameObject responsePoint = GameObject.Find("TutorialChest");

        if (responsePoint != null) {
            target = responsePoint; // target�� �Ҵ�
            Debug.Log($"ResponsePoint found and assigned as target: {responsePoint.name}");
        }
        else {
            Debug.LogError("ResponsePoint not found in the scene!");
        }
        TeleportPlayerToTarget(player, target);
    }
}
