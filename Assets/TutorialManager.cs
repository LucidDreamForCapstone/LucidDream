using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Ȱ��ȭ�� ������Ʈ
    [SerializeField] private float delayTime = 19f;   // ���� �ð� (19��)
    [SerializeField] private float fadeDuration = 3f;// ���̵� �� ���� �ð� (1��)

    private CanvasGroup canvasGroup;

    private void Start() {
        if (targetObject != null) {
            // CanvasGroup Ȯ�� �� �߰�
            canvasGroup = targetObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = targetObject.AddComponent<CanvasGroup>();
            }

            // �ʱ� ���� ����
            targetObject.SetActive(false); // �ʱ� ��Ȱ��ȭ
            canvasGroup.alpha = 0; // ���� ����

            // 19�� �� ���̵� �� ȿ��
            ActivateWithFadeInAsync().Forget();
        }
        else {
            Debug.LogWarning("Target Object is not assigned in the GameManager!");
        }
    }

    private async UniTaskVoid ActivateWithFadeInAsync() {
        // 19�� ���
        await UniTask.Delay(System.TimeSpan.FromSeconds(delayTime));

        // ������Ʈ Ȱ��ȭ
        targetObject.SetActive(true);

        // ���̵� �� ȿ��
        await FadeInAsync();
    }

    private async UniTask FadeInAsync() {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            await UniTask.Yield(); // ���� �����ӱ��� ���
        }

        canvasGroup.alpha = 1f; // ������ ���̵��� ����
    }
}
