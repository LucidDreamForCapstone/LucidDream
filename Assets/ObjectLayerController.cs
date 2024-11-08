using UnityEngine;

public class ObjectLayerController : MonoBehaviour
{
    [SerializeField] private float _layerBorder; // ���̾� ���� ���ؼ�
    private SpriteRenderer _spriteRenderer;

    void Start() {
        // SpriteRenderer ������Ʈ ����
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer�� �����ϴ�.");
        }
    }

    void Update() {
        ChangeLayer(); // �� �����Ӹ��� ���̾� ����
    }

    private void ChangeLayer() {
        if (PlayerDataManager.Instance.Player == null) return; // Player�� null�� ��� ����

        var playerPosition = PlayerDataManager.Instance.Player.transform.position;
        var objectPosition = transform.position;

        // Player�� ������Ʈ�� y ��ǥ ���̿� ���� ���̾� ����
        if (playerPosition.y - objectPosition.y > _layerBorder) {
            _spriteRenderer.sortingOrder = 8; // Player�� ���ʿ� ���� ��
        }
        else {
            _spriteRenderer.sortingOrder = 4; // Player�� �Ʒ��ʿ� ���� ��
        }
    }
}
