using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private bool isdoorOpen;
    [SerializeField] private GameObject labDoor; // 닫힌 문
    [SerializeField] private GameObject openDoor; // 열린 문

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isdoorOpen)
            DoorOpen();
    }

    private void DoorOpen()
    {
        if (Input.GetKey(KeyCode.G)) // 'G' 키를 눌렀을 때
        {
            Player2_InteractManager.Instance.InteractCoolTime().Forget(); // 쿨타임 처리
            if (labDoor != null && openDoor != null)
            {
                labDoor.SetActive(false); // 닫힌 문 비활성화
                openDoor.SetActive(true); // 열린 문 활성화
            }
        }
    }

    public void DoorOpenOnGameClear(bool cleared)
    {
        isdoorOpen = cleared;
    }
}

