using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchController : MonoBehaviour {
    public Volume volume;
    private bool isActive = false;
    [SerializeField] AudioClip _glitchSound;

    public async UniTask TriggerGlitchEffect(float duration = 3f) {
        SoundManager.Instance.PlaySFX(_glitchSound.name, false);
        isActive = true;
        volume.weight = 1f; // 최대 강도로 설정
        float startWeight = volume.weight; // 시작 강도
        float endWeight = 0f; // 끝 강도

        // 강도를 서서히 감소
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
            volume.weight = Mathf.Lerp(startWeight, endWeight, t / duration);
            await UniTask.NextFrame();
        }

        volume.weight = endWeight; // 최종 강도 설정
        isActive = false;
    }

    public void Glitch() {
        TriggerGlitchEffect(3f).Forget();
    }

    public bool IsActive() {
        return isActive; // 글리치 효과가 활성화되었는지 여부 반환
    }
}
