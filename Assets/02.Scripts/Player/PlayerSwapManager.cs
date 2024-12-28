using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro ���� ���ӽ����̽� �߰�
using Edgar.Unity.Examples.Gungeon;
using System;
using UnityEngine;


public class PlayerSwapManager : MonoBehaviour {
    private static PlayerSwapManager _instance;
    public static PlayerSwapManager Instance { get { return _instance; } }

    [SerializeField] private Player player1; // ù ��° ĳ����
    [SerializeField] private Player_2 player2; // �� ��° ĳ����
    [SerializeField] GameObject _miniMapObj;
    [SerializeField] GungeonGameManager _dungeonManager;
    [SerializeField] float _portalSearchRadius;
    private MonoBehaviour currentPlayer; // ���� ���� ���� ĳ����
    private int currentPlayerNum;
    private GlitchController glitchController; // GlitchController �ν��Ͻ�
    private ColorController colorController; //�۸�ġ�� ȭ�� ȿ�� �ο�
    private bool isGlitching = false; // �۸�ġ ȿ�� ���� �� ����
    private bool fogTag = true;
    [SerializeField] private GameObject _cameraObj;
    [SerializeField] private Canvas player1UICanvas; // Player1 UI Canvas
    [SerializeField] private Canvas player2UICanvas; // Player2 UI Canvas

    // UI �ؽ�Ʈ
    [SerializeField] string _message;
    [SerializeField] string _message2;
    [SerializeField] Color _messageColor;
    [SerializeField] private bool swapFlag = false;
    private bool canSwap = true; // �÷��̾� ���� ���� ����
    private LayerMask _portalLayer;

    public bool SwapFlag { get => swapFlag; set => swapFlag = value; }

    private void Awake() {
        _instance = this;
    }
    private void Start() {
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 Ȱ��ȭ
        if (Display.displays.Length > 2) Display.displays[2].Activate(); // Display 3 Ȱ��ȭ
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController ã��
        colorController = FindObjectOfType<ColorController>();
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 Ȱ��ȭ
        _portalLayer = LayerMask.GetMask("Portal");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching && canSwap && swapFlag) { // �۸�ġ ���� �ƴϸ� ���� ������ ����
            if (CanSwapCharacter()) {
                SwapCharacter().Forget();
            }
            else {
                SystemMessageManager.Instance.PushSystemMessage(_message, _messageColor);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && !canSwap) {
            SystemMessageManager.Instance.PushSystemMessage(_message2, _messageColor);
        }
    }

    private bool CanSwapCharacter() {
        if (player1 == null) {
            Debug.LogError("Player reference is not assigned.");
            return false; // �÷��̾ �Ҵ���� ���� ���
        }
        if (!IsBossStage() && CurrentPlayerIsOne()) {
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

    private bool IsBossStage() {
        return _dungeonManager.Stage == 4;
    }


    private async UniTaskVoid SwapCharacter() {
        isGlitching = true;

        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        if (glitchController != null) {
            float glitchTime = 1;
            int flashCount = 2;

            await UniTask.WhenAll(
                glitchController.TriggerGlitchEffect(glitchTime), // �۸�ġ ȿ��
                colorController.TriggerColorGlitchEffect(glitchTime, flashCount) // 3�ʰ� 3ȸ �ݺ� ȭ�� ��� ��ȭ
            );
        }

        if (currentPlayerNum == 1) {
            player1.SetEnable(false);
            if (!IsBossStage())
                player1.ColliderOff();
            player2.SetEnable(true);
            player2.ColliderOn();
            currentPlayer = player2;
            player1UICanvas.targetDisplay = 1;
            player2UICanvas.targetDisplay = 0;
            currentPlayerNum = 2;
            _miniMapObj.SetActive(false);
            GungeonGameManager.Instance.SetIsGenerating(false);
        }
        else if (currentPlayerNum == 2) {
            player2.SetEnable(false);
            player2.ColliderOff();
            player1.SetEnable(true);
            player1.ColliderOn();
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
        return canSwap && swapFlag; // ���� ���� ���� ���� ��ȯ
    }

    public bool CurrentPlayerIsOne() {
        if (currentPlayer == player1)
            return true;

        return false;
    }

}
