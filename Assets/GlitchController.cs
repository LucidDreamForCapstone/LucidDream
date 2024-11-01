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
        glitchEffect.intensity.value = 1f; // 최대 강도 설정
        float duration = 3f; // 총 지속 시간
        float startIntensity = glitchEffect.intensity.value; // 시작 강도
        float endIntensity = 0f; // 끝 강도

        // 강도를 서서히 감소
        for (float t = 0; t < duration; t += Time.deltaTime) {
            glitchEffect.intensity.value = Mathf.Lerp(startIntensity, endIntensity, t / duration);
            yield return null; // 한 프레임 대기
        }

        glitchEffect.intensity.value = endIntensity; // 최종 강도 설정
        isActive = false;
    }

    public bool IsActive() {
        return isActive; // 글리치 효과가 활성화되었는지 여부 반환
    }
}
