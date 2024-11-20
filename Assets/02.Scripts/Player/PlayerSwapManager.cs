using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro ���� ���ӽ����̽� �߰�
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerSwapManager : MonoBehaviour {
    [SerializeField] private Player player1; // ù ��° ĳ����
    [SerializeField] private Player_2 player2; // �� ��° ĳ����
    [SerializeField] GameObject _miniMapObj;
    private MonoBehaviour currentPlayer; // ���� ���� ���� ĳ����
    private int currentPlayerNum;
    private GlitchController glitchController; // GlitchController �ν��Ͻ�
    private VignetteController vignetteController; //�۸�ġ�� ȭ�� ȿ�� �ο�
    private bool isGlitching = false; // �۸�ġ ȿ�� ���� �� ����
    private bool fogTag = true;
    [SerializeField] private GameObject _cameraObj;
    [SerializeField] private Canvas player1UICanvas; // Player1 UI Canvas
    [SerializeField] private Canvas player2UICanvas; // Player2 UI Canvas

    // UI �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI messageText; // TextMeshProUGUI ������Ʈ

    private void Start() {
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 Ȱ��ȭ
        if (Display.displays.Length > 2) Display.displays[2].Activate(); // Display 3 Ȱ��ȭ
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController ã��
        vignetteController = FindObjectOfType<VignetteController>();
        messageText.gameObject.SetActive(false); // ������ �� �޽��� ����
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 Ȱ��ȭ
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

        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        // �۸�ġ ȿ���� �����մϴ�.
        if (glitchController != null) {
            await UniTask.WhenAll(
                glitchController.TriggerGlitchEffect(), // �۸�ġ ȿ��
                vignetteController.TriggerColorGlitchEffect(3f, 3) // 3�ʰ� 3ȸ �ݺ� ȭ�� ��� ��ȭ
            );
        }

        // ���� ĳ���� ��Ȱ��ȭ �� ���ο� ĳ���� Ȱ��ȭ
        if (currentPlayerNum == 1) {
            player1.SetEnable(false);
            player2.SetEnable(true);
            currentPlayer = player2;
            player1UICanvas.targetDisplay = 1; // Player1 UI�� Display 2�� ��ȯ
            player2UICanvas.targetDisplay = 0; // Player2 UI�� Display 1�� ��ȯ
            currentPlayerNum = 2;
            _miniMapObj.SetActive(false);
        }
        else if (currentPlayerNum == 2) {
            player2.SetEnable(false);
            player1.SetEnable(true);
            currentPlayer = player1;
            currentPlayerNum = 1;
            player1UICanvas.targetDisplay = 0; // Player1 UI�� Display 1�� ��ȯ
            player2UICanvas.targetDisplay = 1; // Player2 UI�� Display 2�� ��ȯ
            _miniMapObj.SetActive(true);
        }
        // ���÷��� ������Ʈ
        Canvas.ForceUpdateCanvases();

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
