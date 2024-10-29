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

    //���� �ڵ� �̹Ƿ� ���� ���߿� ������ �����ؼ� ����սô�.
    private IEnumerator TriggerGlitchEffect() {
        isActive = true;
        glitchEffect.intensity.value = 1f; // �ִ� ���� ����
        yield return new WaitForSeconds(3f); // 3�� ���� ����
        glitchEffect.intensity.value = 0f; // ���� ����
        isActive = false;
    }
}
