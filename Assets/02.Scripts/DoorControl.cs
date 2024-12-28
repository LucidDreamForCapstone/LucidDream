using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private bool isdoorOpen;
    [SerializeField] private GameObject labDoor; // 닫힌 문
    [SerializeField] private GameObject openDoor; // 열린 문
    [SerializeField] private bool isInteractingToPortal = false;
    [SerializeField] private bool isInteractedOnce = false;
    [SerializeField] private int Stage = 0;
    [SerializeField] AudioClip _labDoorOpenSound;
    private bool isDoorOpened = false; // 문이 열렸는지 상태를 추적하는 플래그 변수
    public bool IsInteractingToPortal { get { return isInteractingToPortal; } set { isInteractingToPortal = value; } }
    public bool IsInteractedOnce { get { return isInteractedOnce; } set { isInteractedOnce = value; } }

    public bool IsDoorOpen { get { return isdoorOpen; } set { isdoorOpen = value; } }
    protected void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && isdoorOpen && isInteractedOnce)
            DoorOpen();
    }

    private void DoorOpen() {
        if (Input.GetKey(KeyCode.G)) // 'G' 키를 눌렀을 때
        {
            Player2_InteractManager.Instance.InteractCoolTime().Forget(); // 쿨타임 처리
            if (labDoor != null && openDoor != null && !isDoorOpened) {
                SoundManager.Instance.PlaySFX(_labDoorOpenSound.name, false);
                labDoor.SetActive(false); // 닫힌 문 비활성화
                openDoor.SetActive(true); // 열린 문 활성화
                isDoorOpened = true;    
            }
        }
    }


    public void DoorOpenOnGameClear(bool cleared) {
        isdoorOpen = cleared;
    }
}

