using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;

public class MonsterUIDeathTrigger : TutorialUIManager
{
    [SerializeField] private List<GameObject> cardInfoUI; // UI text list
    [SerializeField] private GameObject continuePromptUI; // "Press G to continue" UI
    private bool isPromptActive = false;

    public async Task TriggerDeathUI() {
        // Set Time.timeScale to 0 (Pause the game)
        Time.timeScale = 0f;

        SetColorFilter(new Color(68f / 255f, 67f / 255f, 66f / 255f)); // Adjust screen color to dark

        for (int i = 0; i < cardInfoUI.Count; i++) {
            // Activate the current UI
            cardInfoUI[i].SetActive(true);

            // Delay for 2 seconds (ignores time scale)
            await UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: true);

            // Activate "Press G to continue"
            continuePromptUI.SetActive(true);
            isPromptActive = true;

            // Wait for G key input
            while (isPromptActive) {
                if (Input.GetKeyDown(KeyCode.G)) {
                    // Deactivate the current UI
                    cardInfoUI[i].SetActive(false);

                    // Deactivate "Press G to continue"
                    continuePromptUI.SetActive(false);
                    isPromptActive = false;
                }

                // Wait for the next frame (ignores time scale)
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        // Deactivate all UI and restore settings
        continuePromptUI.SetActive(false);
        StartCoroutine(RestoreDefaultColor()); // Restore color

        // Restore Time.timeScale to 1 (Resume the game)
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Restore the ColorFilter to its original value over time
    /// </summary>
    protected override IEnumerator RestoreDefaultColor() {
        if (colorAdjustments == null) yield break;

        Color initialColor = colorAdjustments.colorFilter.value;
        Color targetColor = Color.white; // Target color: white
        float elapsedTime = 0f;

        while (elapsedTime < delayBeforeContinueText) {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to avoid time scale influence
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, targetColor, elapsedTime / delayBeforeContinueText);
            yield return null;
        }

        colorAdjustments.colorFilter.value = targetColor; // Set to the exact target color
    }
}
