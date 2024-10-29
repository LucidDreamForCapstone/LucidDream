using UnityEngine;

public class PSM : MonoBehaviour {
    [SerializeField] private Player player1; // 첫 번째 캐릭터
    [SerializeField] private Player_2 player2; // 두 번째 캐릭터
    private Player currentPlayer; // 현재 조작 중인 캐릭터

    private void Start() {
        currentPlayer = player1; // 시작 시 player1으로 설정
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            SwapCharacter();
        }
    }

    private void SwapCharacter() {
        // 현재 캐릭터를 비활성화
        currentPlayer.gameObject.SetActive(false);

        // 캐릭터 스왑
        currentPlayer = (currentPlayer == player1) ? player2 : player1;

        // 새로운 캐릭터 활성화
        currentPlayer.gameObject.SetActive(true);

        // 카메라 전환
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            virtualCamera.Follow = currentPlayer.transform; // 새로운 캐릭터로 카메라 목표 변경
        }
    }
}
