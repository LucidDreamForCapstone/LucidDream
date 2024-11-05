using Edgar.Unity; // TextMeshPro ���� ���ӽ����̽� �߰�
using System.Collections;
using TMPro;
using UnityEngine;

public class PSM : MonoBehaviour {
    [SerializeField] private Player player1; // ù ��° ĳ����
    [SerializeField] private Player_2 player2; // �� ��° ĳ����
    private MonoBehaviour currentPlayer; // ���� ���� ���� ĳ����
    private GlitchController glitchController; // GlitchController �ν��Ͻ�
    private bool isGlitching = false; // �۸�ġ ȿ�� ���� �� ����
    private bool fogTag = true;
    [SerializeField] private GameObject _gameObject;

    // UI �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI messageText; // TextMeshProUGUI ������Ʈ

    private void Start() {
        currentPlayer = player1; // ���� �� player1���� ����
        player1.gameObject.SetActive(true); // player1 Ȱ��ȭ
        player2.gameObject.SetActive(false); // player2 ��Ȱ��ȭ
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController ã��

        messageText.gameObject.SetActive(false); // ������ �� �޽��� ����
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching) { // �۸�ġ ���� �ƴ� ���� ĳ���� ����
            if (CanSwapCharacter()) {
                StartCoroutine(SwapCharacter());
            }
            else {
                ShowMessage("������ ���Ͱ� �־ �Ұ����մϴ�."); // �޽��� ǥ��
            }
        }
    }

    private bool CanSwapCharacter() {
        if (player1 == null) {
            Debug.LogError("Player reference is not assigned.");
            return false; // �÷��̾ �Ҵ���� ���� ���
        }
        // �÷��̾� �ֺ��� Enemy�� üũ�մϴ�.
        LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // Enemy ���̾� ����ũ ��������
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player1.transform.position, 20f, enemyLayer); // ������ 20, enemyLayer�� ����
        foreach (var collider in colliders) {
            Debug.Log($"Detected collider: {collider.gameObject.name} with tag: {collider.tag}"); // ������ �ݶ��̴� ���
            if (collider.CompareTag("Enemy")) {
                Debug.Log("Enemy found! Cannot swap character.");
                return false; // Enemy�� ���� ��� ���� �Ұ���
            }
        }
        Debug.Log("No enemies nearby. Can swap character.");
        return true; // ���� ����
    }



    private IEnumerator SwapCharacter() {
        isGlitching = true; // �۸�ġ ���� ��

        // Fog Of War ��Ȱ��ȭ
        if (fogTag) {
            _gameObject.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        // �۸�ġ ȿ���� �����մϴ�.
        if (glitchController != null) {
            yield return glitchController.TriggerGlitchEffect(); // �۸�ġ ȿ�� ����
        }

        // ���� ĳ���͸� ��Ȱ��ȭ
        currentPlayer.gameObject.SetActive(false);

        // ĳ���� ����
        currentPlayer = (currentPlayer == player1) ? (MonoBehaviour)player2 : player1;

        // ���ο� ĳ���� Ȱ��ȭ
        currentPlayer.gameObject.SetActive(true);

        // ī�޶� ��ȯ
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            // Transposer ��������
            var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            if (transposer != null) {
                // Damping ���� 0���� ����
                transposer.m_XDamping = 0;
                transposer.m_YDamping = 0;
            }

            virtualCamera.Follow = currentPlayer.transform; // ���ο� ĳ���ͷ� ī�޶� ��ǥ ����
        }

        // Fog Of War �ٽ� Ȱ��ȭ
        if (!fogTag) {
            _gameObject.GetComponent<FogOfWarGrid2D>().enabled = true; // ��Ȱ��ȭ�� ��� Ȱ��ȭ
        }

        fogTag = !fogTag; // true -> false �Ǵ� false -> true�� ��ȯ

        yield return new WaitForSeconds(0.2f); // 0.2�� ���

        // �۸�ġ ȿ���� ���� �� Damping�� �ٽ� 1�� ����
        if (virtualCamera != null) {
            var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            if (transposer != null) {
                transposer.m_XDamping = 1; // Damping �� ����
                transposer.m_YDamping = 1; // Damping �� ����
            }
        }

        isGlitching = false; // �۸�ġ ȿ�� ����
    }


    private void ShowMessage(string message) {
        messageText.text = message; // �޽��� �ؽ�Ʈ ������Ʈ
        messageText.gameObject.SetActive(true); // �޽��� ǥ��
        StartCoroutine(HideMessage()); // �޽��� ����� �ڷ�ƾ ȣ��
    }

    private IEnumerator HideMessage() {
        yield return new WaitForSeconds(2f); // 2�� ��
        messageText.gameObject.SetActive(false); // �޽��� ����
    }
}
