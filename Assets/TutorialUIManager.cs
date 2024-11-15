using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TutorialUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] tutorialUIs; // 각 튜토리얼 UI 패널들
    [SerializeField] private TextMeshProUGUI continueText; // "G키를 누르세요" 문구
    [SerializeField] private float delayBeforeContinueText = 1f; // "G키를 누르세요" 문구 표시 딜레이

    [Header("Post Processing Settings")]
    [SerializeField] private Volume postProcessingVolume; // Post Processing Volume
    private ColorAdjustments colorAdjustments; // Color Adjustments 컴포넌트

    private int currentIndex = 0; // 현재 활성화된 UI 인덱스
    private bool canContinue = false; // G키 입력 대기 상태

    private void Start() {
        // 모든 UI 초기화
        foreach (var ui in tutorialUIs) {
            ui.SetActive(false);
        }
        continueText.gameObject.SetActive(false);

        // PostProcessing Volume에서 ColorAdjustments 가져오기
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // 초기 색상: 흰색 (255, 255, 255)
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
        // G키 입력 감지
        if (canContinue && Input.GetKeyDown(KeyCode.G)) {
            HideCurrentTutorialUI();
            ShowNextTutorialUI();
        }
    }

    private void ShowNextTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // 현재 튜토리얼 UI 활성화
        tutorialUIs[currentIndex].SetActive(true);

        // 색상 조절: 어두운 색으로 변경
        SetColorFilter(new Color(68f / 255f, 67f / 255f, 66f / 255f));

        StartCoroutine(ShowContinueTextAfterDelay());
    }

    private void HideCurrentTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // 현재 UI 비활성화
        tutorialUIs[currentIndex].SetActive(false);
        continueText.gameObject.SetActive(false);
        canContinue = false; // 다음 UI가 나타날 때까지 입력 비활성화
        currentIndex++;

        // 모든 UI가 끝난 경우 색상 복구
        if (currentIndex >= tutorialUIs.Length) {
            StartCoroutine(RestoreDefaultColor());
        }
    }

    private IEnumerator ShowContinueTextAfterDelay() {
        yield return new WaitForSeconds(delayBeforeContinueText);

        // "계속하려면 G키를 누르세요" 문구 표시
        continueText.gameObject.SetActive(true);
        canContinue = true;
    }

    /// <summary>
    /// ColorFilter를 설정
    /// </summary>
    /// <param name="targetColor">목표 색상</param>
    private void SetColorFilter(Color targetColor) {
        if (colorAdjustments != null) {
            colorAdjustments.colorFilter.value = targetColor;
        }
    }

    /// <summary>
    /// 1초 동안 ColorFilter를 원래 값으로 복구
    /// </summary>
    private IEnumerator RestoreDefaultColor() {
        if (colorAdjustments == null) yield break;

        Color initialColor = colorAdjustments.colorFilter.value;
        Color targetColor = Color.white; // 목표 색상: 흰색 (255, 255, 255)
        float elapsedTime = 0f;

        while (elapsedTime < delayBeforeContinueText) {
            elapsedTime += Time.deltaTime;
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, targetColor, elapsedTime / delayBeforeContinueText);
            yield return null;
        }

        colorAdjustments.colorFilter.value = targetColor; // 정확히 목표 색상으로 설정
    }
}