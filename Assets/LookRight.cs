using UnityEngine;

public class LookRight : MonoBehaviour
{
    private bool hasMoved = false; // �������� ���� 1ȸ�� ����ǵ��� ����
    public bool simulateRightArrow = false; // RightArrow �Է��� ������ �ùķ��̼��ϴ� �÷���

    // Trigger ���� �� ȣ��
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) // �÷��̾� �±� Ȯ��
        {
            Debug.Log("ddd");
            Input.GetKeyDown(KeyCode.RightArrow);
            hasMoved = true; // �������� ���������Ƿ� true�� ����

        }
    }
}