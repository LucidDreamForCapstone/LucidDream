using UnityEngine;

public class PSM : MonoBehaviour {
    [SerializeField] private Player player1; // ù ��° ĳ����
    [SerializeField] private Player_2 player2; // �� ��° ĳ����
    private Player currentPlayer; // ���� ���� ���� ĳ����

    private void Start() {
        currentPlayer = player1; // ���� �� player1���� ����
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            SwapCharacter();
        }
    }

    private void SwapCharacter() {
        // ���� ĳ���͸� ��Ȱ��ȭ
        currentPlayer.gameObject.SetActive(false);

        // ĳ���� ����
        currentPlayer = (currentPlayer == player1) ? player2 : player1;

        // ���ο� ĳ���� Ȱ��ȭ
        currentPlayer.gameObject.SetActive(true);

        // ī�޶� ��ȯ
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            virtualCamera.Follow = currentPlayer.transform; // ���ο� ĳ���ͷ� ī�޶� ��ǥ ����
        }
    }
}
