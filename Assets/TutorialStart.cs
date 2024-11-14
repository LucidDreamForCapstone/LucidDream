using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;

<<<<<<< Updated upstream
public class ColorFilterFadeIn : MonoBehaviour
=======
public class TutorialStart : MonoBehaviour
>>>>>>> Stashed changes
{
    [SerializeField] private Volume postProcessingVolume; // Post Processing Volume ����
    [SerializeField] private float fadeDuration = 3f; // ���̵� �� �ð� (��)

    private Color startColor = new Color(0, 0, 0, 1); // �ʱ� ���� (����)
    private Color targetColor = new Color(1, 1, 1, 1); // ��ǥ ���� (���)
    private ColorAdjustments colorAdjustments;

    private async void Start() {
        // Post Processing Volume���� Color Adjustments ������Ʈ ��������
        if (postProcessingVolume.profile.TryGet(out colorAdjustments)) {
            // �ʱ� ���� ����
            colorAdjustments.colorFilter.value = startColor;

            // ���̵� �� ȿ�� ����
            await FadeInColorFilter();
        }
        else {
            Debug.LogError("Color Adjustments is not found in the Post Processing Volume.");
        }
    }

<<<<<<< Updated upstream
    private async UniTask FadeInColorFilter() {
=======
    protected async UniTask FadeInColorFilter() {
>>>>>>> Stashed changes
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // ������ startColor���� targetColor�� ���� ����
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, targetColor, t);
            await UniTask.Yield(); // ���� �����ӱ��� ���
        }

        // ���� ���� ����
        colorAdjustments.colorFilter.value = targetColor;
    }
}
