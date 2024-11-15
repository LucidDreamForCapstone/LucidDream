using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TutorialUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] tutorialUIs; // �� Ʃ�丮�� UI �гε�
    [SerializeField] private TextMeshProUGUI continueText; // "GŰ�� ��������" ����
    [SerializeField] private float delayBeforeContinueText = 1f; // "GŰ�� ��������" ���� ǥ�� ������

    [Header("Post Processing Settings")]
    [SerializeField] private Volume postProcessingVolume; // Post Processing Volume
    private ColorAdjustments colorAdjustments; // Color Adjustments ������Ʈ

    private int currentIndex = 0; // ���� Ȱ��ȭ�� UI �ε���
    private bool canContinue = false; // GŰ �Է� ��� ����

    private void Start() {
        // ��� UI �ʱ�ȭ
        foreach (var ui in tutorialUIs) {
            ui.SetActive(false);
        }
        continueText.gameObject.SetActive(false);

        // PostProcessing Volume���� ColorAdjustments ��������
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // �ʱ� ����: ��� (255, 255, 255)
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
        // GŰ �Է� ����
        if (canContinue && Input.GetKeyDown(KeyCode.G)) {
            HideCurrentTutorialUI();
            ShowNextTutorialUI();
        }
    }

    private void ShowNextTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // ���� Ʃ�丮�� UI Ȱ��ȭ
        tutorialUIs[currentIndex].SetActive(true);

        // ���� ����: ��ο� ������ ����
        SetColorFilter(new Color(68f / 255f, 67f / 255f, 66f / 255f));

        StartCoroutine(ShowContinueTextAfterDelay());
    }

    private void HideCurrentTutorialUI() {
        if (currentIndex >= tutorialUIs.Length) return;

        // ���� UI ��Ȱ��ȭ
        tutorialUIs[currentIndex].SetActive(false);
        continueText.gameObject.SetActive(false);
        canContinue = false; // ���� UI�� ��Ÿ�� ������ �Է� ��Ȱ��ȭ
        currentIndex++;

        // ��� UI�� ���� ��� ���� ����
        if (currentIndex >= tutorialUIs.Length) {
            StartCoroutine(RestoreDefaultColor());
        }
    }

    private IEnumerator ShowContinueTextAfterDelay() {
        yield return new WaitForSeconds(delayBeforeContinueText);

        // "����Ϸ��� GŰ�� ��������" ���� ǥ��
        continueText.gameObject.SetActive(true);
        canContinue = true;
    }

    /// <summary>
    /// ColorFilter�� ����
    /// </summary>
    /// <param name="targetColor">��ǥ ����</param>
    private void SetColorFilter(Color targetColor) {
        if (colorAdjustments != null) {
            colorAdjustments.colorFilter.value = targetColor;
        }
    }

    /// <summary>
    /// 1�� ���� ColorFilter�� ���� ������ ����
    /// </summary>
    private IEnumerator RestoreDefaultColor() {
        if (colorAdjustments == null) yield break;

        Color initialColor = colorAdjustments.colorFilter.value;
        Color targetColor = Color.white; // ��ǥ ����: ��� (255, 255, 255)
        float elapsedTime = 0f;

        while (elapsedTime < delayBeforeContinueText) {
            elapsedTime += Time.deltaTime;
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, targetColor, elapsedTime / delayBeforeContinueText);
            yield return null;
        }

        colorAdjustments.colorFilter.value = targetColor; // ��Ȯ�� ��ǥ �������� ����
    }
}