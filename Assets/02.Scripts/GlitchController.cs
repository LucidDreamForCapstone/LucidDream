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
        glitchEffect.intensity.value = 1f; // 최대 강도 설정
        float startIntensity = glitchEffect.intensity.value; // 시작 강도
        float endIntensity = 0f; // 끝 강도

        // 강도를 서서히 감소
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
            glitchEffect.intensity.value = Mathf.Lerp(startIntensity, endIntensity, t / duration);
            await UniTask.NextFrame();
        }

        glitchEffect.intensity.value = endIntensity; // 최종 강도 설정
        isActive = false;
    }
    public void Glitch() {
        TriggerGlitchEffect(3f).Forget();
    }
    public bool IsActive() {
        return isActive; // 글리치 효과가 활성화되었는지 여부 반환
    }
}
