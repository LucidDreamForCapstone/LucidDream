using Cysharp.Threading.Tasks;
using Edgar.Unity.Examples.Gungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzlePortal : MonoBehaviour, Interactable {
    [SerializeField] string _message;
    [SerializeField] string _message2;
    [SerializeField] Color _messageColor;
    [SerializeField] PuzzleBase puzzle;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] bool clearedOnce = false;
    [SerializeField] GungeonGameManager gungeonGameManager;
    [SerializeField] private GameObject player;
    [SerializeField] public InputAction playerAction;
    [SerializeField] List<string> _messages;
    [SerializeField] List<string> _messages2;
    [SerializeField] Material _normalMat;
    [SerializeField] Material _glitchMat;
    private GameObject finalSpawnPoint;
    private SpriteRenderer _sr;
    private bool stageChecked = false;
    private bool preventPuzzleStageUpdate = false;
    private bool isInputDisabled = false;
    private bool _isPortalDialogReady = true;

    void Start() {
        _sr = GetComponent<SpriteRenderer>();
        gungeonGameManager = GungeonGameManager.Instance;
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        puzzle = puzzleManager.CurrentPuzzle;
        finalSpawnPoint = GameObject.Find("SpawnPosition");
        player = InteractManager.Instance.gameObject;
        _messages.Add("음....?\n다음 스테이지로 가는 포탈이\n알 수 없는 힘으로 깨져있는 것 같아.");
        _messages.Add("왠지 내 앞에 있는 <size=45><color=red>퍼즐을 해결하면</color></size>\n포탈을 복구할 수 있을 것 같은데?");
    }



    void Update() {
        //if (!stageChecked && gungeonGameManager.Stage == 4) {
        //    stageChecked = true;
        //    TeleportPlayerToTarget_Final(player, finalSpawnPoint);
        //}

        if (puzzle.Cleared && puzzleManager.CurrentPuzzleIndex != 0 && !clearedOnce) {
            if (gungeonGameManager.Stage < 4) {
                SystemMessageManager.Instance.ShowDialogBox("주인공", _messages2, 3).Forget();
                SystemMessageManager.Instance.PushSystemMessage("퍼즐 클리어!", Color.green, false, 2f);
            }
            clearedOnce = true;
        }

        if (puzzle.Cleared) {
            _sr.material = _normalMat;
        }
        else {
            _sr.material = _glitchMat;
        }
    }

    private void HandleTrigger(Collider2D collision, bool isEntering) {
        puzzleManager.IsInteractingToPortal = false;
        if (collision.gameObject.CompareTag("Player")) {
            if (gungeonGameManager.Stage == 4) {
                CameraManager.Instance.SetFogOfWar(false);
                TeleportPlayerToTarget_Final(player, finalSpawnPoint);
            }

            Debug.Log($"Player is Colliding to Portal {isEntering}");
            //puzzleManager.IsInteractingToPortal = isEntering;
            if (GungeonGameManager.Instance != null && puzzle.Cleared && !isInputDisabled) {
                if (Input.GetKey(KeyCode.G)) {
                    isInputDisabled = true;
                    puzzleManager.CurrentPuzzle.DoorController.IsInteractedOnce = true;
                    Debug.Log("GPressed!");
                    GungeonGameManager.Instance.SetIsGenerating(isEntering);
                    Debug.Log($"isGenerating set to {isEntering}");
                    if (!preventPuzzleStageUpdate) {
                        puzzleManager.ChangePuzzle();
                        preventPuzzleStageUpdate = true;
                    }
                }
            }
            else {
                if (Input.GetKey(KeyCode.G) && _isPortalDialogReady && !clearedOnce) {
                    _isPortalDialogReady = false;
                    ReadyWithDelay(10).Forget();
                    SystemMessageManager.Instance.ShowDialogBox("???", _messages, 4).Forget();
                }
                Debug.LogError("GGM instance is null");
            }
            //puzzleManager.IsInteractingToPortal = isEntering;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (puzzleManager.CurrentPuzzleIndex == 0) {
            puzzleManager.CurrentPuzzle.DoorController.IsDoorOpen = true;
            puzzleManager.ChangePuzzle();
            Debug.Log(puzzleManager.CurrentPuzzle.transform.name);
            puzzle = puzzleManager.CurrentPuzzle;
        }
        HandleTrigger(collision, true);
    }
    private void OnTriggerExit2D(Collider2D collision) => HandleTrigger(collision, false);

    private void TeleportPlayerToTarget_Final(GameObject player, GameObject targetObject) {
        if (player == null || targetObject == null) {
            Debug.LogError("Player or Target object is null!");
            return;
        }

        player.transform.position = targetObject.transform.position;
        player.transform.rotation = targetObject.transform.rotation;

        Debug.Log($"Player {player.name} teleported to {targetObject.name} at position {targetObject.transform.position}");
    }

    public bool IsInteractBlock() => !puzzle.Cleared;

    public string GetInteractText() => "이동 (G)";

    private IEnumerator DisableInputForSeconds(float duration) {
        isInputDisabled = true;
        playerAction.Disable();
        yield return new WaitForSecondsRealtime(duration);
        playerAction.Enable();
        isInputDisabled = false;
    }

    private async UniTask ReadyWithDelay(float waitTime) {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        SystemMessageManager.Instance.PushSystemMessage(_message2, Color.yellow, false, 4);
        _isPortalDialogReady = true;
    }
}
