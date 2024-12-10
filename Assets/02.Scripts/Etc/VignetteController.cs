using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour {
    [SerializeField] private Player player;        // Player 스크립트 참조
    private Vignette vignette;                     // Vignette 효과
    private ColorAdjustments colorAdjustments;     // Color Adjustments 효과

    private float maxVignetteIntensity = 1f;       // Vignette Intensity의 최대값
    private float minVignetteIntensity = 0.5f;     // Vignette Intensity의 최소값

    private void Start() {
        Volume volume = OptionManager.Instance.GetBrightnessVolume();
        // Post-processing Volume에서 Vignette와 Color Adjustments 컴포넌트를 가져옴
        if (volume.profile.TryGet<Vignette>(out vignette)) {
            vignette.intensity.value = minVignetteIntensity;  // 초기 Intensity 설정
        }

        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // 초기 Color Filter 값 설정
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

    // Color Filter 값 변경 함수
    public async UniTask TriggerColorGlitchEffect(float duration = 3f, int repetitions = 3) {
        if (colorAdjustments == null) return;

        Color originalColor = colorAdjustments.colorFilter.value; // 기존 값 저장
        Color glitchColor = Color.black; // 글리치 색상 (0, 0, 0)

        for (int i = 0; i < repetitions; i++) {
            // Fade In: 밝기를 점진적으로 어둡게 설정
            for (float t = 0; t < duration / (2 * repetitions); t += Time.unscaledDeltaTime) {
                colorAdjustments.colorFilter.value = Color.Lerp(originalColor, glitchColor, t / (duration / (2 * repetitions)));
                await UniTask.NextFrame(); // 다음 프레임 대기
            }

            // Fade Out: 밝기를 점진적으로 원래 상태로 복원
            for (float t = 0; t < duration / (2 * repetitions); t += Time.unscaledDeltaTime) {
                colorAdjustments.colorFilter.value = Color.Lerp(glitchColor, originalColor, t / (duration / (2 * repetitions)));
                await UniTask.NextFrame(); // 다음 프레임 대기
            }
        }

        // 최종적으로 원래 색상으로 복원
        colorAdjustments.colorFilter.value = originalColor;
    }

}
