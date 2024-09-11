using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour {
    [SerializeField] private Player player;        // Player 스크립트 참조
    [SerializeField] private Volume postProcessingVolume;  // 포스트 프로세싱 볼륨
    private Vignette vignette;                     // Vignette 효과

    private float maxVignetteIntensity = 1f;       // Vignette Intensity의 최대값
    private float minVignetteIntensity = 0.5f;     // Vignette Intensity의 최소값

    private void Start() {
        // Post-processing Volume에서 Vignette 컴포넌트를 가져옴
        if (postProcessingVolume.profile.TryGet<Vignette>(out vignette)) {
            vignette.intensity.value = minVignetteIntensity;  // 초기 Intensity 설정
        }
    }

    private void Update() {
        // PlayerDataManager에서 플레이어 체력 정보를 가져옴
        if (PlayerDataManager.Instance != null && vignette != null) {
            float currentHP = PlayerDataManager.Instance.Status._hp;
            float maxHP = PlayerDataManager.Instance.Status._maxHp;

            // 체력에 비례해서 Vignette Intensity 값 조절 (체력이 0일 때 Intensity는 최대값)
            float normalizedHP = currentHP / maxHP;
            float vignetteIntensity = Mathf.Lerp(maxVignetteIntensity, minVignetteIntensity, normalizedHP);

            vignette.intensity.value = vignetteIntensity;
        }
    }
}


