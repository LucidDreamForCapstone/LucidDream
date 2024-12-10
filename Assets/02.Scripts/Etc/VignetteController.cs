using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour {
    [SerializeField] private Player player;        // Player ��ũ��Ʈ ����
    private Vignette vignette;                     // Vignette ȿ��
    private ColorAdjustments colorAdjustments;     // Color Adjustments ȿ��

    private float maxVignetteIntensity = 1f;       // Vignette Intensity�� �ִ밪
    private float minVignetteIntensity = 0.5f;     // Vignette Intensity�� �ּҰ�

    private void Start() {
        Volume volume = OptionManager.Instance.GetBrightnessVolume();
        // Post-processing Volume���� Vignette�� Color Adjustments ������Ʈ�� ������
        if (volume.profile.TryGet<Vignette>(out vignette)) {
            vignette.intensity.value = minVignetteIntensity;  // �ʱ� Intensity ����
        }

        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.white; // �ʱ� Color Filter �� ����
        }
    }

    private void Update() {
        // PlayerDataManager���� �÷��̾� ü�� ������ ������
        if (PlayerDataManager.Instance != null && vignette != null) {
            float currentHP = PlayerDataManager.Instance.Status._hp;
            float maxHP = PlayerDataManager.Instance.Status._maxHp;

            // ü�¿� ����ؼ� Vignette Intensity �� ���� (ü���� 0�� �� Intensity�� �ִ밪)
            float normalizedHP = currentHP / maxHP;
            float vignetteIntensity = Mathf.Lerp(maxVignetteIntensity, minVignetteIntensity, normalizedHP);

            vignette.intensity.value = vignetteIntensity;
        }
    }

    // Color Filter �� ���� �Լ�
    public async UniTask TriggerColorGlitchEffect(float duration = 3f, int repetitions = 3) {
        if (colorAdjustments == null) return;

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
