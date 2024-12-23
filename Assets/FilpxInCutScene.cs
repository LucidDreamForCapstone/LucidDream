using UnityEngine;

public class FilpxInCutScene : MonoBehaviour {
    [SerializeField] private GameObject _gameObject;
    public void SetFlipX() {
        // Sprite Renderer 컴포넌트를 가져옴
        SpriteRenderer spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) {
            spriteRenderer.flipX = true; // FlipX를 true로 설정
            Debug.Log($"{_gameObject.name}의 FlipX가 활성화되었습니다.");
        }
        else {
            Debug.LogError($"{_gameObject.name}에 SpriteRenderer 컴포넌트가 없습니다.");
        }
    }
}
