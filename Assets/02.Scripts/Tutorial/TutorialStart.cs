using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TutorialStart : MonoBehaviour {
    [SerializeField] private float fadeDuration = 3f; // Fade-in duration (seconds)
    [SerializeField] AudioClip _tutorialBGM;

    private Color startColor = new Color(0, 0, 0, 1); // Initial color (black)
    private Color targetColor = new Color(1, 1, 1, 1); // Target color (white)
    private ColorAdjustments colorAdjustments;

    private void Awake() {
        SoundManager.Instance.PlayBGM(_tutorialBGM.name);
    }

    private async void Start() {
        // OptionManager 싱글톤을 사용하여 Volume 가져오기
        Volume volume = OptionManager.Instance.GetBrightnessVolume();

        // Volume에서 ColorAdjustments 가져오기
        if (volume.profile.TryGet(out colorAdjustments)) {
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
}
