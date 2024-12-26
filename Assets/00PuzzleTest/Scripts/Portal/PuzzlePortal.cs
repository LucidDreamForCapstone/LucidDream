using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzlePortal : MonoBehaviour, Interactable {
    [SerializeField] string _message;
    [SerializeField] Color _messageColor;
    [SerializeField] PuzzleBase puzzle;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] bool clearedOnce = false;
    [SerializeField] GungeonGameManager gungeonGameManager;
    [SerializeField] private GameObject player;
    [SerializeField] public InputAction playerAction;
    private GameObject finalSpawnPoint;
    private bool stageChecked = false;
    private bool preventPuzzleStageUpdate = false;
    private bool isInputDisabled = false;

    void Start() {
        gungeonGameManager = GungeonGameManager.Instance;
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        puzzle = puzzleManager.CurrentPuzzle;
        finalSpawnPoint = GameObject.Find("SpawnPosition");
        player = InteractManager.Instance.gameObject;
    }



    void Update() {
        //if (!stageChecked && gungeonGameManager.Stage == 4) {
        //    stageChecked = true;
        //    TeleportPlayerToTarget_Final(player, finalSpawnPoint);
        //}

        if (puzzle.Cleared && puzzleManager.CurrentPuzzleIndex != 0 && !clearedOnce) {
            SystemMessageManager.Instance.PushSystemMessage("퍼즐 클리어!", Color.green, false, 2f);
            clearedOnce = true;
        }
    }

    private void HandleTrigger(Collider2D collision, bool isEntering) {
        puzzleManager.IsInteractingToPortal = false;
        if (collision.gameObject.CompareTag("Player")) {
            if (gungeonGameManager.Stage == 4) TeleportPlayerToTarget_Final(player, finalSpawnPoint);
            Debug.Log($"Player is Colliding to Portal {isEntering}");
            //puzzleManager.IsInteractingToPortal = isEntering;
            if (GungeonGameManager.Instance != null && puzzle.Cleared && Input.GetKey(KeyCode.G) && !isInputDisabled) {
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
            else {
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
}
