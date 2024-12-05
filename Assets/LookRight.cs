using UnityEngine;

public class LookRight : MonoBehaviour
{
    private bool hasMoved = false; // 움직임이 최초 1회만 실행되도록 제어
    public bool simulateRightArrow = false; // RightArrow 입력을 강제로 시뮬레이션하는 플래그

    // Trigger 진입 시 호출
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) // 플레이어 태그 확인
        {
            Debug.Log("ddd");
            Input.GetKeyDown(KeyCode.RightArrow);
            hasMoved = true; // 움직임을 실행했으므로 true로 설정

        }
    }
}