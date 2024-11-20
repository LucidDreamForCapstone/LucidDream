using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private float fadeDuration = 2f; // Fade in duration
    [SerializeField] private Volume postProcessingVolume; // Reference to Post Processing Volume
    private SpriteRenderer _spriteRenderer;
    private Bloom _bloom;
    private bool isTriggered = false; // Prevent multiple triggers

    void Start() {
        // Ensure the GameObject has a SpriteRenderer component
        _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer is missing from the GameObject!");
            return;
        }

        // Initialize the SpriteRenderer for fade in
        Color color = _spriteRenderer.color;
        color.a = 0f;
        _spriteRenderer.color = color;

        // Ensure Post Processing Volume is set
        if (postProcessingVolume.profile.TryGet(out _bloom)) {
            _bloom.intensity.overrideState = true; // Enable override
            _bloom.intensity.value = 0f; // Start with no bloom
        }

        _gameObject.SetActive(false); // Ensure the GameObject starts as inactive
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (isTriggered || !other.CompareTag("Player")) return; // Only trigger once and check for the Player tag

        isTriggered = true; // Prevent multiple triggers
        _gameObject.SetActive(true); // Activate the GameObject
        FadeInAsync().Forget(); // Start the fade-in process
        FadeInBloom().Forget(); // Start the Bloom fade-in process
    }

    private async UniTask FadeInAsync() {
        float elapsedTime = 0f;
        Color color = _spriteRenderer.color;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime for frame-independent timing
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration); // Interpolate alpha value
            _spriteRenderer.color = color; // Apply updated color
            await UniTask.Yield(); // Wait for the next frame
        }

        // Ensure alpha is fully set to 1
        color.a = 1f;
        _spriteRenderer.color = color;
    }

    private async UniTask FadeInBloom() {
        if (_bloom == null) return;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore Time.timeScale
            _bloom.intensity.value = Mathf.Lerp(0f, 5f, elapsedTime / fadeDuration); // Adjust target intensity
            await UniTask.Yield();
        }

        _bloom.intensity.value = 5f; // Ensure final intensity
    }
}
