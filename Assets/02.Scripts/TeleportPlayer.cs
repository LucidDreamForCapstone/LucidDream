using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerTeleporter : MonoBehaviour {
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject target;
    private ColorAdjustments colorAdjustments; // ColorAdjustments 컴포넌트
    /// <summary>
    /// 플레이어를 특정 오브젝트 위치로 순간 이동시킵니다.
    /// </summary>
    /// <param name="player">이동할 플레이어 오브젝트</param>
    /// <param name="targetObject">목표 위치를 가진 오브젝트</param>
    /// 
    private void Start() {
        Volume volume = OptionManager.Instance.GetBrightnessVolume();
        // ColorAdjustments 컴포넌트 가져오기
        if (volume!= null && volume.profile.TryGet(out ColorAdjustments adjustments)) {
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

        // 플레이어의 Transform 가져오기
        Transform playerTransform = player.transform;

        // 이동할 목표 Transform 가져오기
        Transform targetTransform = targetObject.transform;

        // 플레이어 위치 및 회전 변경
        playerTransform.position = targetTransform.position;
        playerTransform.rotation = targetTransform.rotation;

        Debug.Log($"Player {player.name} teleported to {targetObject.name} at position {targetTransform.position}");
        
        // ColorAdjustments 변경
        if (colorAdjustments != null) {
            colorAdjustments.hueShift.value = 0f; // Hue Shift 0으로 설정
            colorAdjustments.saturation.value = 0f; // Saturation 0으로 설정
            Debug.Log("ColorAdjustments: Hue Shift and Saturation set to 0.");
        }
        else {
            Debug.LogError("ColorAdjustments component is not available!");
        }
        
    }

    /// <summary>
    /// Signal에서 호출되는 텔레포트 함수
    /// </summary>
    public void SignalTP() {
        // 하이어라키에서 "ResponsePoint"라는 이름의 오브젝트를 검색
        GameObject responsePoint = GameObject.Find("TutorialChest");

        if (responsePoint != null) {
            target = responsePoint; // target에 할당
            Debug.Log($"ResponsePoint found and assigned as target: {responsePoint.name}");
        }
        else {
            Debug.LogError("ResponsePoint not found in the scene!");
        }
        TeleportPlayerToTarget(player, target);
    }
}
