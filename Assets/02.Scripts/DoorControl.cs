using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private bool isdoorOpen;
    [SerializeField] private GameObject labDoor; // ���� ��
    [SerializeField] private GameObject openDoor; // ���� ��
    [SerializeField] private bool isInteractingToPortal = false;
    public bool IsInteractingToPortal { get { return isInteractingToPortal; } set { isInteractingToPortal = value; } }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isdoorOpen&&isInteractingToPortal)
            DoorOpen();
    }

    private void DoorOpen()
    {
        if (Input.GetKey(KeyCode.G)) // 'G' Ű�� ������ ��
        {
            Player2_InteractManager.Instance.InteractCoolTime().Forget(); // ��Ÿ�� ó��
            if (labDoor != null && openDoor != null)
            {
                labDoor.SetActive(false); // ���� �� ��Ȱ��ȭ
                openDoor.SetActive(true); // ���� �� Ȱ��ȭ
            }
        }
    }


    public void DoorOpenOnGameClear(bool cleared)
    {
        isdoorOpen = cleared;
    }
}

