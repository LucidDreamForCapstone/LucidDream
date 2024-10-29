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

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            StartCoroutine(TriggerGlitchEffect());
        }
    }

    //예시 코드 이므로 추후 개발에 적당히 수정해서 사용합시다.
    private IEnumerator TriggerGlitchEffect() {
        isActive = true;
        glitchEffect.intensity.value = 1f; // 최대 강도 설정
        yield return new WaitForSeconds(3f); // 3초 동안 유지
        glitchEffect.intensity.value = 0f; // 강도 해제
        isActive = false;
    }
}
