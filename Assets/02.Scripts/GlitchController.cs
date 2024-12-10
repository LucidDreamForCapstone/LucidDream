using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchController : MonoBehaviour {
    public Volume volume;
    private bool isActive = false;

    public async UniTask TriggerGlitchEffect(float duration = 3f) {
        isActive = true;
        volume.weight = 1f; // �ִ� ������ ����
        float startWeight = volume.weight; // ���� ����
        float endWeight = 0f; // �� ����

        // ������ ������ ����
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
            volume.weight = Mathf.Lerp(startWeight, endWeight, t / duration);
            await UniTask.NextFrame();
        }

        volume.weight = endWeight; // ���� ���� ����
        isActive = false;
    }

    public void Glitch() {
        TriggerGlitchEffect(3f).Forget();
    }

    public bool IsActive() {
        return isActive; // �۸�ġ ȿ���� Ȱ��ȭ�Ǿ����� ���� ��ȯ
    }
}
