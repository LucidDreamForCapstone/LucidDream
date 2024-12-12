using UnityEngine;
using Cysharp.Threading.Tasks;

public class PortalBlinkEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Sprite Renderer 참조
    [SerializeField] private float blinkDuration = 3f; // 깜빡임 지속 시간
    [SerializeField] private float blinkInterval = 0.1f; // 깜빡임 간격

    private bool isBlinking = false;

    private void Start() {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer 자동 할당
        }
    }

    public void StartBlinking() {
        if (isBlinking) return;
        BlinkEffectAsync().Forget();
    }

    private async UniTaskVoid BlinkEffectAsync() {
        isBlinking = true;
        float elapsedTime = 0f;
        bool increase = true;

        while (elapsedTime < blinkDuration) {
            elapsedTime += blinkInterval;

            // SpriteRenderer의 Alpha 값을 증가/감소
            Color currentColor = spriteRenderer.color;
            if (increase) {
                currentColor.a += 0.1f;
                if (currentColor.a >= 1f) increase = false;
            }
            else {
                currentColor.a -= 0.1f;
                if (currentColor.a <= 0f) increase = true;
            }

            spriteRenderer.color = currentColor;

            // 지정된 간격만큼 대기
            await UniTask.Delay((int)(blinkInterval * 1000), ignoreTimeScale: true);
        }

        // 깜빡임 종료 후 Alpha 값을 0으로 설정
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;

        // 오브젝트 비활성화
        gameObject.SetActive(false);

        isBlinking = false;
    }
}
