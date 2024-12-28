using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private bool isdoorOpen;
    [SerializeField] private GameObject labDoor; // ���� ��
    [SerializeField] private GameObject openDoor; // ���� ��
    [SerializeField] private bool isInteractingToPortal = false;
    [SerializeField] private bool isInteractedOnce = false;
    [SerializeField] private int Stage = 0;
    [SerializeField] AudioClip _labDoorOpenSound;
    private bool isDoorOpened = false; // ���� ���ȴ��� ���¸� �����ϴ� �÷��� ����
    public bool IsInteractingToPortal { get { return isInteractingToPortal; } set { isInteractingToPortal = value; } }
    public bool IsInteractedOnce { get { return isInteractedOnce; } set { isInteractedOnce = value; } }

    public bool IsDoorOpen { get { return isdoorOpen; } set { isdoorOpen = value; } }
    protected void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && isdoorOpen && isInteractedOnce)
            DoorOpen();
    }

    private void DoorOpen() {
        if (Input.GetKey(KeyCode.G)) // 'G' Ű�� ������ ��
        {
            Player2_InteractManager.Instance.InteractCoolTime().Forget(); // ��Ÿ�� ó��
            if (labDoor != null && openDoor != null && !isDoorOpened) {
                SoundManager.Instance.PlaySFX(_labDoorOpenSound.name, false);
                labDoor.SetActive(false); // ���� �� ��Ȱ��ȭ
                openDoor.SetActive(true); // ���� �� Ȱ��ȭ
                isDoorOpened = true;    
            }
        }
    }


    public void DoorOpenOnGameClear(bool cleared) {
        isdoorOpen = cleared;
    }
}

