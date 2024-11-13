using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // 활성화할 오브젝트
    [SerializeField] private float delayTime = 19f;   // 지연 시간 (19초)
    [SerializeField] private float fadeDuration = 3f;// 페이드 인 지속 시간 (1초)

    private CanvasGroup canvasGroup;

    private void Start() {
        if (targetObject != null) {
            // CanvasGroup 확인 및 추가
            canvasGroup = targetObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = targetObject.AddComponent<CanvasGroup>();
            }

            // 초기 상태 설정
            targetObject.SetActive(false); // 초기 비활성화
            canvasGroup.alpha = 0; // 투명 상태

            // 19초 후 페이드 인 효과
            ActivateWithFadeInAsync().Forget();
        }
        else {
            Debug.LogWarning("Target Object is not assigned in the GameManager!");
        }
    }

    private async UniTaskVoid ActivateWithFadeInAsync() {
        // 19초 대기
        await UniTask.Delay(System.TimeSpan.FromSeconds(delayTime));

        // 오브젝트 활성화
        targetObject.SetActive(true);

        // 페이드 인 효과
        await FadeInAsync();
    }

    private async UniTask FadeInAsync() {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            await UniTask.Yield(); // 다음 프레임까지 대기
        }

        canvasGroup.alpha = 1f; // 완전히 보이도록 설정
    }
}
