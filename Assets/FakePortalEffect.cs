using UnityEngine;
using Cysharp.Threading.Tasks;

public class PortalBlinkEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Sprite Renderer ����
    [SerializeField] private float blinkDuration = 3f; // ������ ���� �ð�
    [SerializeField] private float blinkInterval = 0.1f; // ������ ����

    private bool isBlinking = false;

    private void Start() {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer �ڵ� �Ҵ�
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

            // SpriteRenderer�� Alpha ���� ����/����
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

            // ������ ���ݸ�ŭ ���
            await UniTask.Delay((int)(blinkInterval * 1000), ignoreTimeScale: true);
        }

        // ������ ���� �� Alpha ���� 0���� ����
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;

        // ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);

        isBlinking = false;
    }
}
