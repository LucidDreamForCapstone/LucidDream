using System.Collections;
using UnityEngine;
public class WeaponText : MonoBehaviour {
    [SerializeField] GameObject textObject;  // 자식 오브젝트로 있는 텍스트
    private Coroutine showTextCoroutine;

    private void Start() {
        if (textObject != null) {
            textObject.SetActive(false);  // 처음에는 비활성화된 상태
        }
        else
            Debug.Log($"PUT WEAPON TEXT TO THIS WEAPON PREFAB : [{transform.parent.name}]");
    }

    // 플레이어가 멀어졌을 때 텍스트 비활성화
    public void HideText() {
        if (showTextCoroutine != null) {
            StopCoroutine(showTextCoroutine);
            showTextCoroutine = null;
        }
        if (textObject != null) {
            textObject.SetActive(false); // 텍스트 비활성화
        }
    }

    // 플레이어가 가까이 다가왔을 때 텍스트 활성화
    public void ShowTextDelay(float delay) {
        if (showTextCoroutine != null) {
            StopCoroutine(showTextCoroutine);
        }
        showTextCoroutine = StartCoroutine(ShowTextCoroutine(delay));
    }

    private IEnumerator ShowTextCoroutine(float delay) {
        yield return new WaitForSeconds(delay);
        if (textObject != null) {
            textObject.SetActive(true);
        }
    }
}

