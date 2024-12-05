using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro 관련 네임스페이스 추가
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerSwapManager : MonoBehaviour {
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

    // UI 텍스트
    [SerializeField] string _message;
    [SerializeField] Color _messageColor;

    private void Start() {
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 활성화
        if (Display.displays.Length > 2) Display.displays[2].Activate(); // Display 3 활성화
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController 찾기
        vignetteController = FindObjectOfType<VignetteController>();
        if (Display.displays.Length > 1) Display.displays[1].Activate(); // Display 2 활성화
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching) { // 글리치 중이 아닐 때만 캐릭터 스왑
            if (CanSwapCharacter()) {
                SwapCharacter().Forget();
            }
            else {
                SystemMessageManager.Instance.PushSystemMessage(_message, _messageColor);
            }
        }
    }

    private bool CanSwapCharacter() {
        if (player1 == null) {
            Debug.LogError("Player reference is not assigned.");
            return false; // 플레이어가 할당되지 않은 경우
        }
        // 플레이어 주변의 Enemy를 체크합니다.
        if (!IsFinalBossScene()) {
            LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // Enemy 레이어 마스크 가져오기
            Collider2D[] colliders = Physics2D.OverlapCircleAll(player1.transform.position, 20f, enemyLayer); // 반지름 20, enemyLayer만 감지
            foreach (var collider in colliders) {
                Debug.Log($"Detected collider: {collider.gameObject.name} with tag: {collider.tag}"); // 감지된 콜라이더 출력
                if (collider.CompareTag("Enemy")) {
                    Debug.Log("Enemy found! Cannot swap character.");
                    return false; // Enemy가 있을 경우 스왑 불가능
                }
            }
        }
        Debug.Log("No enemies nearby. Can swap character.");
        return true; // 스왑 가능
    }

    private bool IsFinalBossScene() {
        string finalBossScene = "BossTestScene";//after change it
        return SceneManager.GetActiveScene().name == finalBossScene;
    }


    private async UniTaskVoid SwapCharacter() {
        isGlitching = true; // 글리치 시작 중

        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        // 글리치 효과를 시작합니다.
        if (glitchController != null) {
            float glitchTime = 3;
            int flashCount = 3;
            if (IsFinalBossScene()) {
                glitchTime = 1;
                flashCount = 2;
            }

            await UniTask.WhenAll(
                glitchController.TriggerGlitchEffect(glitchTime), // 글리치 효과
                vignetteController.TriggerColorGlitchEffect(glitchTime, flashCount) // 3초간 3회 반복 화면 밝기 변화
            );
        }

        // 기존 캐릭터 비활성화 및 새로운 캐릭터 활성화
        if (currentPlayerNum == 1) {
            player1.SetEnable(false);
            player2.SetEnable(true);
            currentPlayer = player2;
            player1UICanvas.targetDisplay = 1; // Player1 UI를 Display 2로 전환
            player2UICanvas.targetDisplay = 0; // Player2 UI를 Display 1로 전환
            currentPlayerNum = 2;
            _miniMapObj.SetActive(false);
        }
        else if (currentPlayerNum == 2) {
            player2.SetEnable(false);
            player1.SetEnable(true);
            currentPlayer = player1;
            currentPlayerNum = 1;
            player1UICanvas.targetDisplay = 0; // Player1 UI를 Display 1로 전환
            player2UICanvas.targetDisplay = 1; // Player2 UI를 Display 2로 전환
            _miniMapObj.SetActive(true);
        }
        // 디스플레이 업데이트
        Canvas.ForceUpdateCanvases();

        // 카메라 전환
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            // Transposer 가져오기
            var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            if (transposer != null) {
                // Damping 값을 0으로 설정
                transposer.m_XDamping = 0;
                transposer.m_YDamping = 0;
            }

            virtualCamera.Follow = currentPlayer.transform; // 새로운 캐릭터로 카메라 목표 변경
        }


        // Fog Of War 다시 활성화
        if (!fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = true; // 비활성화된 경우 활성화
        }

        fogTag = !fogTag; // true -> false 또는 false -> true로 전환
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        // 글리치 효과가 끝난 후 Damping을 다시 1로 설정
        if (virtualCamera != null) {
            var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            if (transposer != null) {
                transposer.m_XDamping = 1; // Damping 값 복원
                transposer.m_YDamping = 1; // Damping 값 복원
            }
        }
        isGlitching = false; // 글리치 효과 종료
    }

    //To prevent Edgar targeting the player2 as the main player while doing map creating
    private async UniTaskVoid Player2ActiveDelay(float waitTime) {
        player2.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        player2.gameObject.SetActive(true);
    }

}
