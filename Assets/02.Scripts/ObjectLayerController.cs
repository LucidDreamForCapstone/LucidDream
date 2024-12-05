using UnityEngine;

public class ObjectLayerController : MonoBehaviour
{
    [SerializeField] private float _layerBorder; // ���̾� ���� ���ؼ�
    private SpriteRenderer _spriteRenderer;

    void Start() {
        // SpriteRenderer ������Ʈ ����
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (Player2DataManager.Instance.Player2 != null) // Player_2 �ν��Ͻ��� ������ ���
        {
            ChangeLayer(Player2DataManager.Instance.Player2.transform.position); // �Ÿ� ��� �� ���̾� ����
        }
    }

    private void ChangeLayer(Vector3 playerPosition) {
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
