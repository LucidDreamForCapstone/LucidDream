using Cysharp.Threading.Tasks;
using Edgar.Unity; // TextMeshPro 관련 네임스페이스 추가
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PSM : MonoBehaviour {
    [SerializeField] private Player player1; // 첫 번째 캐릭터
    [SerializeField] private Player_2 player2; // 두 번째 캐릭터
    [SerializeField] GameObject _miniMapObj;
    private MonoBehaviour currentPlayer; // 현재 조작 중인 캐릭터
    private int currentPlayerNum;
    private GlitchController glitchController; // GlitchController 인스턴스
    private bool isGlitching = false; // 글리치 효과 진행 중 여부
    private bool fogTag = true;
    [SerializeField] private GameObject _cameraObj;

    // UI 텍스트
    [SerializeField] private TextMeshProUGUI messageText; // TextMeshProUGUI 컴포넌트

    private void Start() {
        currentPlayerNum = 1;
        _miniMapObj.SetActive(true);
        Player2ActiveDelay(2).Forget();
        glitchController = FindObjectOfType<GlitchController>(); // GlitchController 찾기

        messageText.gameObject.SetActive(false); // 시작할 때 메시지 숨김
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F) && !isGlitching) { // 글리치 중이 아닐 때만 캐릭터 스왑
            if (CanSwapCharacter()) {
                SwapCharacter().Forget();
            }
            else {
                ShowMessage("주위에 몬스터가 있어서 불가능합니다."); // 메시지 표시
            }
        }
    }

    private bool CanSwapCharacter() {
        if (player1 == null) {
            Debug.LogError("Player reference is not assigned.");
            return false; // 플레이어가 할당되지 않은 경우
        }
        // 플레이어 주변의 Enemy를 체크합니다.
        LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // Enemy 레이어 마스크 가져오기
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player1.transform.position, 20f, enemyLayer); // 반지름 20, enemyLayer만 감지
        foreach (var collider in colliders) {
            Debug.Log($"Detected collider: {collider.gameObject.name} with tag: {collider.tag}"); // 감지된 콜라이더 출력
            if (collider.CompareTag("Enemy")) {
                Debug.Log("Enemy found! Cannot swap character.");
                return false; // Enemy가 있을 경우 스왑 불가능
            }
        }
        Debug.Log("No enemies nearby. Can swap character.");
        return true; // 스왑 가능
    }



    private async UniTaskVoid SwapCharacter() {
        isGlitching = true; // 글리치 시작 중

        if (fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = false;
        }

        // 글리치 효과를 시작합니다.
        if (glitchController != null) {
            await glitchController.TriggerGlitchEffect(); // 글리치 효과 시작
        }

        // 기존 캐릭터 비활성화 및 새로운 캐릭터 활성화
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

        // 카메라 전환
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null) {
            virtualCamera.Follow = currentPlayer.transform; // 새로운 캐릭터로 카메라 목표 변경
        }

        // Fog Of War 다시 활성화
        if (!fogTag) {
            _cameraObj.GetComponent<FogOfWarGrid2D>().enabled = true; // 비활성화된 경우 활성화
        }

        fogTag = !fogTag; // true -> false 또는 false -> true로 전환
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        isGlitching = false; // 글리치 효과 종료
    }

    private void ShowMessage(string message) {
        messageText.text = message; // 메시지 텍스트 업데이트
        messageText.gameObject.SetActive(true); // 메시지 표시
        StartCoroutine(HideMessage()); // 메시지 숨기기 코루틴 호출
    }

    private IEnumerator HideMessage() {
        yield return new WaitForSeconds(2f); // 2초 후
        messageText.gameObject.SetActive(false); // 메시지 숨김
    }

    //To prevent Edgar targeting the player2 as the main player while doing map creating
    private async UniTaskVoid Player2ActiveDelay(float waitTime) {
        player2.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        player2.gameObject.SetActive(true);
    }
}
