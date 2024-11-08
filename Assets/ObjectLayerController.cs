using UnityEngine;

public class ObjectLayerController : MonoBehaviour
{
    [SerializeField] private float _layerBorder; // 레이어 변경 기준선
    private SpriteRenderer _spriteRenderer;

    void Start() {
        // SpriteRenderer 컴포넌트 참조
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer가 없습니다.");
        }
    }

    void Update() {
        ChangeLayer(); // 매 프레임마다 레이어 변경
    }

    private void ChangeLayer() {
        if (PlayerDataManager.Instance.Player == null) return; // Player가 null인 경우 종료

        var playerPosition = PlayerDataManager.Instance.Player.transform.position;
        var objectPosition = transform.position;

        // Player와 오브젝트의 y 좌표 차이에 따라 레이어 변경
        if (playerPosition.y - objectPosition.y > _layerBorder) {
            _spriteRenderer.sortingOrder = 8; // Player가 위쪽에 있을 때
        }
        else {
            _spriteRenderer.sortingOrder = 4; // Player가 아래쪽에 있을 때
        }
    }
}
