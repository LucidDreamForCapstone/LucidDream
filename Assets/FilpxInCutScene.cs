using UnityEngine;

public class FilpxInCutScene : MonoBehaviour {
    [SerializeField] private GameObject _gameObject;
    public void SetFlipX() {
        // Sprite Renderer ������Ʈ�� ������
        SpriteRenderer spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) {
            spriteRenderer.flipX = true; // FlipX�� true�� ����
            Debug.Log($"{_gameObject.name}�� FlipX�� Ȱ��ȭ�Ǿ����ϴ�.");
        }
        else {
            Debug.LogError($"{_gameObject.name}�� SpriteRenderer ������Ʈ�� �����ϴ�.");
        }
    }
}
