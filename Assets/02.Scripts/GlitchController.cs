using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
public class GlitchController : MonoBehaviour {
    public Volume volume;
    private GlitchEffect glitchEffect;
    private bool isActive = false;

    void Start() {
        volume.profile.TryGet(out glitchEffect);
    }

    public async UniTask TriggerGlitchEffect(float duration = 3f) {
        isActive = true;
        glitchEffect.intensity.value = 1f; // �ִ� ���� ����
        float startIntensity = glitchEffect.intensity.value; // ���� ����
        float endIntensity = 0f; // �� ����

        // ������ ������ ����
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
            glitchEffect.intensity.value = Mathf.Lerp(startIntensity, endIntensity, t / duration);
            await UniTask.NextFrame();
        }

        glitchEffect.intensity.value = endIntensity; // ���� ���� ����
        isActive = false;
    }
    public void Glitch() {
        TriggerGlitchEffect(3f).Forget();
    }
    public bool IsActive() {
        return isActive; // �۸�ġ ȿ���� Ȱ��ȭ�Ǿ����� ���� ��ȯ
    }
}
