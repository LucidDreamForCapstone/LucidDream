using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorController : MonoBehaviour {
    Volume colorVolume;
    private ColorAdjustments colorAdjustments;     // Color Adjustments ȿ��

    private void Start() {
        colorVolume = GetComponent<Volume>();
        // Post-processing Volume���� Vignette�� Color Adjustments ������Ʈ�� ������
        if (colorVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // �ʱ� Color Filter �� ����
        }
    }

    // Color Filter �� ���� �Լ�
    public async UniTask TriggerColorGlitchEffect(float duration = 3f, int repetitions = 3) {
        if (colorAdjustments == null) {
            Debug.LogError("No Color Adjustments Volume!");
            return;
        }
        Color originalColor = colorAdjustments.colorFilter.value; // ���� �� ����
        Color glitchColor = Color.black; // �۸�ġ ���� (0, 0, 0)

        for (int i = 0; i < repetitions; i++) {
            // Fade In: ��⸦ ���������� ��Ӱ� ����
            for (float t = 0; t < duration / (2 * repetitions); t += Time.unscaledDeltaTime) {
                colorAdjustments.colorFilter.value = Color.Lerp(originalColor, glitchColor, t / (duration / (2 * repetitions)));
                await UniTask.NextFrame(); // ���� ������ ���
            }

            // Fade Out: ��⸦ ���������� ���� ���·� ����
            for (float t = 0; t < duration / (2 * repetitions); t += Time.unscaledDeltaTime) {
                colorAdjustments.colorFilter.value = Color.Lerp(glitchColor, originalColor, t / (duration / (2 * repetitions)));
                await UniTask.NextFrame(); // ���� ������ ���
            }
        }

        // ���������� ���� �������� ����
        colorAdjustments.colorFilter.value = originalColor;
    }

}
