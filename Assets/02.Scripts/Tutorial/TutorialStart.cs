using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TutorialStart : MonoBehaviour {
    [SerializeField] private float fadeDuration = 3f; // Fade-in duration (seconds)
    [SerializeField] private Volume colorVolume;
    private Color startColor = new Color(0, 0, 0, 1); // Initial color (black)
    private Color targetColor = new Color(1, 1, 1, 1); // Target color (white)
    private ColorAdjustments colorAdjustments;

    private async void Start() {
        // OptionManager �̱����� ����Ͽ� Volume ��������
        // Volume���� ColorAdjustments ��������
        if (colorVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.overrideState = true; // Enable color filter override
            colorAdjustments.colorFilter.value = startColor; // Set initial color

            await FadeInColor(); // Start the fade-in effect
        }
        else {
            Debug.LogError("Color Adjustments is not found in the Volume.");
        }
    }

    protected async UniTask FadeInColor() {
        if (colorAdjustments == null) return;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime; // Frame-independent timing
            float t = elapsedTime / fadeDuration; // Calculate normalized time
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, targetColor, t); // Interpolate color
            await UniTask.Yield(); // Wait for the next frame
        }

        // Ensure the final color is precisely set
        colorAdjustments.colorFilter.value = targetColor;
    }

    protected async UniTask FadeInColor_unscaled() {
        if (colorAdjustments == null) return;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime for frame-independent timing
            float t = elapsedTime / fadeDuration; // Calculate normalized time
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, targetColor, t); // Interpolate color
            await UniTask.Yield(); // Wait for the next frame
        }

        // Ensure the final color is precisely set
        colorAdjustments.colorFilter.value = targetColor;
    }

    protected async UniTask FadeOutColor() {
        if (colorAdjustments == null) return;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime; // Frame-independent timing
            float t = elapsedTime / fadeDuration; // Calculate normalized time
            colorAdjustments.colorFilter.value = Color.Lerp(targetColor, startColor, t); // Interpolate color
            await UniTask.Yield(); // Wait for the next frame
        }

        // Ensure the final color is precisely set
        colorAdjustments.colorFilter.value = startColor;
    }

    public void FadeinSignal() {
        FadeInColor().Forget();
    }

    public void FadeOutSignal() {
        FadeOutColor().Forget();
    }
}
