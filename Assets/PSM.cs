using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro ���� ���ӽ����̽� �߰�
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PSM : MonoBehaviour {
    [SerializeField] private Player player1; // ù ��° ĳ����
    [SerializeField] private Player_2 player2; // �� ��° ĳ����
    [SerializeField] GameObject _miniMapObj;
    private MonoBehaviour currentPlayer; // ���� ���� ���� ĳ����
    private int currentPlayerNum;
    private GlitchController glitchController; // GlitchController �ν��Ͻ�
    private bool isGlitching = false; // �۸�ġ ȿ�� ���� �� ����
    private bool fogTag = true;
    [SerializeField] private GameObject _cameraObj;

    // UI �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI messageText; // TextMeshProUGUI ������Ʈ

    private void Start() {
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController ã��

        messageText.gameObject.SetActive(false); // ������ �� �޽��� ����
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching) { // �۸�ġ ���� �ƴ� ���� ĳ���� ����
            if (CanSwapCharacter()) {
                SwapCharacter().Forget();
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



    private async UniTaskVoid SwapCharacter() {
        isGlitching = true; // �۸�ġ ���� ��

        // Fog Of War ��Ȱ��ȭ
        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        // �۸�ġ ȿ���� �����մϴ�.
        if (glitchController != null) {
            await glitchController.TriggerGlitchEffect(); // �۸�ġ ȿ�� ����
        }

        // ���� ĳ���� ��Ȱ��ȭ �� ���ο� ĳ���� Ȱ��ȭ
        if (currentPlayerNum == 1) {
            player1.SetEnable(false);
            player2.SetEnable(true);
            currentPlayer = player2;
            currentPlayerNum = 2;
            _miniMapObj.SetActive(false);
        }
        else if (currentPlayerNum == 2) {
            player2.SetEnable(false);
            player1.SetEnable(true);
            currentPlayer = player1;
            currentPlayerNum = 1;
            _miniMapObj.SetActive(true);
        }

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
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = true; // ��Ȱ��ȭ�� ��� Ȱ��ȭ
        }

        fogTag = !fogTag; // true -> false �Ǵ� false -> true�� ��ȯ
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));

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

    //To prevent Edgar targeting the player2 as the main player while doing map creating
    private async UniTaskVoid Player2ActiveDelay(float waitTime) {
        player2.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        player2.gameObject.SetActive(true);
    }
}
