using UnityEngine;

public class PhantomTrigger : MonoBehaviour {
    public static PhantomTrigger Instance { get; private set; }

    private bool _canUsePhantom = false; // ���� ��� ���� ����

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public bool CanUsePhantom => _canUsePhantom; // ���� ��� ���� ���� ��ȯ

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) // Ʈ������ �±� Ȯ��
        {
            _canUsePhantom = true; // ���� ��� ���� ���·� ����
            Debug.Log("���� ��� ���� ���°� Ȱ��ȭ�Ǿ����ϴ�.");
        }
    }
}
