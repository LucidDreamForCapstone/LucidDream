using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro ���� ���ӽ����̽� �߰�
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    [SerializeField] string _message;
    [SerializeField] Color _messageColor;
    private bool canSwap = true; // �÷��̾� ���� ���� ����

    private void Start() {
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 Ȱ��ȭ
        if (Display.displays.Length > 2) Display.displays[2].Activate(); // Display 3 Ȱ��ȭ
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController ã��
        vignetteController = FindObjectOfType<VignetteController>();
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 Ȱ��ȭ
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching && canSwap) { // �۸�ġ ���� �ƴϸ� ���� ������ ����
            if (CanSwapCharacter()) {
                SwapCharacter().Forget();
            }
            else {
                SystemMessageManager.Instance.PushSystemMessage(_message, _messageColor);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && !canSwap) {
            ShowMessage("���� �÷��̾ ������ �� �����ϴ�."); // ���� �Ұ��� �޽��� ǥ��
        }
    }

    private bool CanSwapCharacter() {
        if (player1 == null) {
            Debug.LogError("Player reference is not assigned.");
            return false; // �÷��̾ �Ҵ���� ���� ���
        }
        if (!IsFinalBossScene()) {
            LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // Enemy ���̾� ����ũ ��������
            Collider2D[] colliders = Physics2D.OverlapCircleAll(player1.transform.position, 20f, enemyLayer); // ������ 20, enemyLayer�� ����
            foreach (var collider in colliders) {
                if (collider.CompareTag("Enemy")) {
                    return false; // Enemy�� ���� ��� ���� �Ұ���
                }
            }
        }
        return true; // ���� ����
    }

    private bool IsFinalBossScene() {
        string finalBossScene = "BossTestScene";//after change it
        return SceneManager.GetActiveScene().name == finalBossScene;
    }


    private async UniTaskVoid SwapCharacter() {
        isGlitching = true;

        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        if (glitchController != null) {
            float glitchTime = 3;
            int flashCount = 3;
            if (IsFinalBossScene()) {
                glitchTime = 1;
                flashCount = 2;
            }

            await UniTask.WhenAll(
                glitchController.TriggerGlitchEffect(glitchTime), // �۸�ġ ȿ��
                vignetteController.TriggerColorGlitchEffect(glitchTime, flashCount) // 3�ʰ� 3ȸ �ݺ� ȭ�� ��� ��ȭ
            );
        }

        if (currentPlayerNum == 1) {
            player1.SetEnable(false);
            player2.SetEnable(true);
            currentPlayer = player2;
            player1UICanvas.targetDisplay = 1;
            player2UICanvas.targetDisplay = 0;
            currentPlayerNum = 2;
            _miniMapObj.SetActive(false);
        }
        else if (currentPlayerNum == 2) {
            player2.SetEnable(false);
            player1.SetEnable(true);
            currentPlayer = player1;
            currentPlayerNum = 1;
            player1UICanvas.targetDisplay = 0;
            player2UICanvas.targetDisplay = 1;
            _miniMapObj.SetActive(true);
        }

        Canvas.ForceUpdateCanvases();

        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            if (transposer != null) {
                transposer.m_XDamping = 0;
                transposer.m_YDamping = 0;
            }

            virtualCamera.Follow = currentPlayer.transform;
        }

        if (!fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = true;
        }

        fogTag = !fogTag;
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));

        if (virtualCamera != null) {
            var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            if (transposer != null) {
                transposer.m_XDamping = 1;
                transposer.m_YDamping = 1;
            }
        }
        isGlitching = false;
    }

    //To prevent Edgar targeting the player2 as the main player while doing map creating
    private async UniTaskVoid Player2ActiveDelay(float waitTime) {
        player2.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        player2.gameObject.SetActive(true);
    }

    public void SetCanSwap(bool value) {
        canSwap = value; // ���� ���� ���θ� ����
    }

    public bool GetCanSwap() {
        return canSwap; // ���� ���� ���� ���� ��ȯ
    }
}
