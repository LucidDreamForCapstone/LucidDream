/*using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TutorialUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] tutorialUIs; // Tutorial UI panels
    [SerializeField] private TextMeshProUGUI continueText; // "Press G to continue" text
    [SerializeField] private float delayBeforeContinueText = 1f; // Delay before showing "Press G to continue" text

    [Header("Post Processing Settings")]
    [SerializeField] private Volume postProcessingVolume; // Post Processing Volume
    private ColorAdjustments colorAdjustments; // Color Adjustments component

    private int currentIndex = 0; // Current active UI index
    private bool canContinue = false; // Waiting for G key input
    protected float previousMoveSpeed; // Store previous move speed 

    private void Start() {
        // Initialize all UI
        foreach (var ui in tutorialUIs) {
            ui.SetActive(false);
        }
        continueText.gameObject.SetActive(false);

        // Get ColorAdjustments from PostProcessing Volume
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // Initial color: white (255, 255, 255)
        }
        else {
            Debug.LogError("Post Processing Volume or Color Adjustments not set!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && currentIndex < tutorialUIs.Length) {
            ShowNextTutorialUI();
        }
    }

    private void Update() {
        // Detect G key input
        if (canContinue && Input.GetKeyDown(KeyCode.G)) {
            HideCurrentTutorialUI();
            ShowNextTutorialUI();
        }
    }

    private void ShowNextTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;
        // Activate the current tutorial UI
        tutorialUIs[currentIndex].SetActive(true);
        // Set time scale to 0 (pause the game)
        Time.timeScale = 0f;
        // Adjust color: switch to darker color
        SetColorFilter(new Color(68f / 255f, 67f / 255f, 66f / 255f));

        StartCoroutine(ShowContinueTextAfterDelay());
    }

    private void HideCurrentTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // Deactivate the current UI
        tutorialUIs[currentIndex].SetActive(false);
        continueText.gameObject.SetActive(false);
        canContinue = false; // Disable input until the next UI is shown
        currentIndex++;
        // If all UIs are finished, restore color and time scale
        if (currentIndex >= tutorialUIs.Length) {
            Time.timeScale = 1f; // Restore time scale
            StartCoroutine(RestoreDefaultColor());
        }
    }

    private IEnumerator ShowContinueTextAfterDelay() {
        yield return new WaitForSeconds(delayBeforeContinueText);

        // Show "Press G to continue" text
        continueText.gameObject.SetActive(true);
        canContinue = true;
    }

    /// <summary>
    /// Set the ColorFilter
    /// </summary>
    /// <param name="targetColor">Target color</param>
    protected void SetColorFilter(Color targetColor) {
        if (colorAdjustments != null) {
            colorAdjustments.colorFilter.value = targetColor;
        }
    }

    /// <summary>
    /// Restore the ColorFilter to its original value over 1 second
    /// </summary>
    protected IEnumerator RestoreDefaultColor() {
        if (colorAdjustments == null) yield break;

        Color initialColor = colorAdjustments.colorFilter.value;
        Color targetColor = Color.white; // Target color: white (255, 255, 255)
        float elapsedTime = 0f;

        while (elapsedTime < delayBeforeContinueText) {
            elapsedTime += Time.deltaTime;
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, targetColor, elapsedTime / delayBeforeContinueText);
            yield return null;
        }

        colorAdjustments.colorFilter.value = targetColor; // Set to the exact target color
    }
}*/
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TutorialUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] tutorialUIs; // Tutorial UI panels
    [SerializeField] private TextMeshProUGUI continueText; // "Press G to continue" text
    [SerializeField] protected float delayBeforeContinueText = 1f; // Delay before showing "Press G to continue" text

    [Header("Post Processing Settings")]
    [SerializeField] private Volume postProcessingVolume; // Post Processing Volume
    protected ColorAdjustments colorAdjustments; // Color Adjustments component

    private int currentIndex = 0; // Current active UI index
    private bool canContinue = false; // Waiting for G key input
    protected float previousMoveSpeed; // Store previous move speed 

    private void Start() {
        // Initialize all UI
        foreach (var ui in tutorialUIs) {
            ui.SetActive(false);
        }
        continueText.gameObject.SetActive(false);

        // Get ColorAdjustments from PostProcessing Volume
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // Initial color: white (255, 255, 255)
        }
        else {
            Debug.LogError("Post Processing Volume or Color Adjustments not set!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && currentIndex < tutorialUIs.Length) {
            ShowNextTutorialUI();
        }
    }

    private void Update() {
        // Detect G key input
        if (canContinue && Input.GetKeyDown(KeyCode.G)) {
            HideCurrentTutorialUI();
            ShowNextTutorialUI();
        }
    }

    private void ShowNextTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // Activate the current tutorial UI
        tutorialUIs[currentIndex].SetActive(true);

        // Set time scale to 0 (pause the game)
        Time.timeScale = 0f;

        // Adjust color: switch to darker color
        SetColorFilter(new Color(68f / 255f, 67f / 255f, 66f / 255f));

        StartCoroutine(ShowContinueTextAfterDelay());
    }

    private void HideCurrentTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // Deactivate the current UI
        tutorialUIs[currentIndex].SetActive(false);
        continueText.gameObject.SetActive(false);
        canContinue = false; // Disable input until the next UI is shown
        currentIndex++;

        // If all UIs are finished, restore color and time scale
        if (currentIndex >= tutorialUIs.Length) {
            Time.timeScale = 1f; // Restore time scale
            StartCoroutine(RestoreDefaultColor());
        }
    }

    private IEnumerator ShowContinueTextAfterDelay() {
        // Use WaitForSecondsRealtime to avoid time scale influence
        yield return new WaitForSecondsRealtime(delayBeforeContinueText);

        // Show "Press G to continue" text
        continueText.gameObject.SetActive(true);
        canContinue = true;
    }

    /// <summary>
    /// Set the ColorFilter
    /// </summary>
    /// <param name="targetColor">Target color</param>
    protected void SetColorFilter(Color targetColor) {
        if (colorAdjustments != null) {
            colorAdjustments.colorFilter.value = targetColor;
        }
    }

    /// <summary>
    /// Restore the ColorFilter to its original value over 1 second
    /// </summary>
    protected virtual IEnumerator RestoreDefaultColor() {
        if (colorAdjustments == null) yield break;

        Color initialColor = colorAdjustments.colorFilter.value;
        Color targetColor = Color.white; // Target color: white (255, 255, 255)
        float elapsedTime = 0f;

        while (elapsedTime < delayBeforeContinueText) {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to avoid time scale influence
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, targetColor, elapsedTime / delayBeforeContinueText);
            yield return null;
        }

        colorAdjustments.colorFilter.value = targetColor; // Set to the exact target color
    }
}
