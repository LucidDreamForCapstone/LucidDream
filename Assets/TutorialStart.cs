using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;

public class ColorFilterFadeIn : MonoBehaviour
{ 

    [SerializeField] private Volume postProcessingVolume; // Post Processing Volume 참조
    [SerializeField] private float fadeDuration = 3f; // 페이드 인 시간 (초)

    private Color startColor = new Color(0, 0, 0, 1); // 초기 색상 (검정)
    private Color targetColor = new Color(1, 1, 1, 1); // 목표 색상 (흰색)
    private ColorAdjustments colorAdjustments;

    private async void Start() {
        // Post Processing Volume에서 Color Adjustments 컴포넌트 가져오기
        if (postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            // 초기 색상 설정
            colorAdjustments.colorFilter.value = startColor;

            // 페이드 인 효과 실행
            await FadeInColorFilter();
        }
        else {
            Debug.LogError("Color Adjustments is not found in the Post Processing Volume.");
        }
    }
    protected async UniTask FadeInColorFilter() {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // 색상을 startColor에서 targetColor로 선형 보간
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, targetColor, t);
            await UniTask.Yield(); // 다음 프레임까지 대기
        }

        // 최종 색상 보장
        colorAdjustments.colorFilter.value = targetColor;
    }
}
