using System.Collections;
using UnityEngine;
public class WeaponText : MonoBehaviour {
    [SerializeField] GameObject textObject;  // �ڽ� ������Ʈ�� �ִ� �ؽ�Ʈ
    private Coroutine showTextCoroutine;

    private void Start() {
        if (textObject != null) {
            textObject.SetActive(false);  // ó������ ��Ȱ��ȭ�� ����
        }
        else
            Debug.Log($"PUT WEAPON TEXT TO THIS WEAPON PREFAB : [{transform.parent.name}]");
    }

    // �÷��̾ �־����� �� �ؽ�Ʈ ��Ȱ��ȭ
    public void HideText() {
        if (showTextCoroutine != null) {
            StopCoroutine(showTextCoroutine);
            showTextCoroutine = null;
        }
        if (textObject != null) {
            textObject.SetActive(false); // �ؽ�Ʈ ��Ȱ��ȭ
        }
    }

    // �÷��̾ ������ �ٰ����� �� �ؽ�Ʈ Ȱ��ȭ
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

