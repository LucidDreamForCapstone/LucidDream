using UnityEngine;

public class ObjectLayerController : MonoBehaviour
{
    [SerializeField] private float _layerBorder; // 레이어 변경 기준선
    private SpriteRenderer _spriteRenderer;

    void Start() {
        // SpriteRenderer 컴포넌트 참조
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (Player2DataManager.Instance.Player2 != null) // Player_2 인스턴스가 존재할 경우
        {
            ChangeLayer(Player2DataManager.Instance.Player2.transform.position); // 거리 계산 및 레이어 변경
        }
    }

    private void ChangeLayer(Vector3 playerPosition) {
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
