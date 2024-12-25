using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorController : MonoBehaviour {
    Volume colorVolume;
    private ColorAdjustments colorAdjustments;     // Color Adjustments 효과

    private void Start() {
        colorVolume = GetComponent<Volume>();
        // Post-processing Volume에서 Vignette와 Color Adjustments 컴포넌트를 가져옴
        if (colorVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // 초기 Color Filter 값 설정
        }
    }

    // Color Filter 값 변경 함수
    public async UniTask TriggerColorGlitchEffect(float duration = 3f, int repetitions = 3) {
        if (colorAdjustments == null) {
            Debug.LogError("No Color Adjustments Volume!");
            return;
        }
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
