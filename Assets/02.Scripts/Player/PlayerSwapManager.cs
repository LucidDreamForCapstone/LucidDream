using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro 관련 네임스페이스 추가
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerSwapManager : MonoBehaviour
{
    [SerializeField] private Player player1; // 첫 번째 캐릭터
    [SerializeField] private Player_2 player2; // 두 번째 캐릭터
    [SerializeField] GameObject _miniMapObj;
    private MonoBehaviour currentPlayer; // 현재 조작 중인 캐릭터
    private int currentPlayerNum;
    private GlitchController glitchController; // GlitchController 인스턴스
    private VignetteController vignetteController; //글리치에 화면 효과 부여
    private bool isGlitching = false; // 글리치 효과 진행 중 여부
    private bool fogTag = true;
    [SerializeField] private GameObject _cameraObj;
    [SerializeField] private Canvas player1UICanvas; // Player1 UI Canvas
    [SerializeField] private Canvas player2UICanvas; // Player2 UI Canvas
    [SerializeField] private TextMeshProUGUI messageText; // TextMeshProUGUI 컴포넌트
    private bool canSwap = true; // 플레이어 스왑 가능 여부

    private void Start() {
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 활성화
        if (Display.displays.Length > 2) Display.displays[2].Activate(); // Display 3 활성화
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController 찾기
        vignetteController = FindObjectOfType<VignetteController>();
        messageText.gameObject.SetActive(false); // 시작할 때 메시지 숨김
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching && canSwap) { // 글리치 중이 아니며 스왑 가능할 때만
            if (CanSwapCharacter()) {
                SwapCharacter().Forget();
            }
            else {
                ShowMessage("주위에 몬스터가 있어서 불가능합니다."); // 메시지 표시
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && !canSwap) {
            ShowMessage("현재 플레이어를 스왑할 수 없습니다."); // 스왑 불가능 메시지 표시
        }
    }

    private bool CanSwapCharacter() {
        if (player1 == null) {
            Debug.LogError("Player reference is not assigned.");
            return false; // 플레이어가 할당되지 않은 경우
        }
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player1.transform.position, 20f, enemyLayer);
        foreach (var collider in colliders) {
            if (collider.CompareTag("Enemy")) {
                return false; // Enemy가 있을 경우 스왑 불가능
            }
        }
        return true; // 스왑 가능
    }

    private async UniTaskVoid SwapCharacter() {
        isGlitching = true;

        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        if (glitchController != null) {
            await UniTask.WhenAll(
                glitchController.TriggerGlitchEffect(),
                vignetteController.TriggerColorGlitchEffect(3f, 3)
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

    private void ShowMessage(string message) {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideMessage());
    }

    private IEnumerator HideMessage() {
        yield return new WaitForSeconds(2f);
        messageText.gameObject.SetActive(false);
    }

    private async UniTaskVoid Player2ActiveDelay(float waitTime) {
        player2.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        player2.gameObject.SetActive(true);
    }

    public void SetCanSwap(bool value) {
        canSwap = value; // 스왑 가능 여부를 설정
    }

    public bool GetCanSwap() {
        return canSwap; // 현재 스왑 가능 여부 반환
    }
}
