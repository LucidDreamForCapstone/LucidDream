using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour {
    [SerializeField] private Player player;        // Player ��ũ��Ʈ ����
    [SerializeField] private Volume postProcessingVolume;  // ����Ʈ ���μ��� ����
    private Vignette vignette;                     // Vignette ȿ��

    private float maxVignetteIntensity = 1f;       // Vignette Intensity�� �ִ밪
    private float minVignetteIntensity = 0.5f;     // Vignette Intensity�� �ּҰ�

    private void Start() {
        // Post-processing Volume���� Vignette ������Ʈ�� ������
        if (postProcessingVolume.profile.TryGet<Vignette>(out vignette)) {
            vignette.intensity.value = minVignetteIntensity;  // �ʱ� Intensity ����
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
}


