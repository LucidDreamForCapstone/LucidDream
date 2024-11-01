using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchController : MonoBehaviour {
    public Volume volume;
    private GlitchEffect glitchEffect;
    private bool isActive = false;

    void Start() {
        volume.profile.TryGet(out glitchEffect);
    }

    public IEnumerator TriggerGlitchEffect() {
        isActive = true;
        glitchEffect.intensity.value = 1f; // �ִ� ���� ����
        float duration = 3f; // �� ���� �ð�
        float startIntensity = glitchEffect.intensity.value; // ���� ����
        float endIntensity = 0f; // �� ����

        // ������ ������ ����
        for (float t = 0; t < duration; t += Time.deltaTime) {
            glitchEffect.intensity.value = Mathf.Lerp(startIntensity, endIntensity, t / duration);
            yield return null; // �� ������ ���
        }

        glitchEffect.intensity.value = endIntensity; // ���� ���� ����
        isActive = false;
    }

    public bool IsActive() {
        return isActive; // �۸�ġ ȿ���� Ȱ��ȭ�Ǿ����� ���� ��ȯ
    }
}
