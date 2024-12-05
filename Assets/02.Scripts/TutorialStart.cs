using UnityEngine;
using UnityEngine.Rendering;

public class TutorialStart : MonoBehaviour {
    [SerializeField] private Volume postProcessingVolume; // Reference to Post Processing Volume
    [SerializeField] private float fadeDuration = 3f; // Fade-in duration (seconds)

    private Color startColor = new Color(0, 0, 0, 1); // Initial color (black)
    private Color targetColor = new Color(1, 1, 1, 1); // Target color (white)
    //private ColorAdjustments colorAdjustments;

    private async void Start() {
        /*
        // Try to get ColorAdjustments from the Post Processing Volume
        if (postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.overrideState = true; // Enable color filter override
            colorAdjustments.colorFilter.value = startColor; // Set initial color

            await FadeInColor(); // Start the fade-in effect
        }
        else {
            Debug.LogError("Color Adjustments is not found in the Post Processing Volume.");
        }
        */
    }
    /*
    protected async UniTask FadeInColor() {
        if (colorAdjustments == null) return;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime; // Use unscaledDeltaTime for frame-independent timing
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
    */
}
