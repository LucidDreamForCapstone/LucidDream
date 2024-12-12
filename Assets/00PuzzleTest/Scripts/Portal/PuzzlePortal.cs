using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class PuzzlePortal : MonoBehaviour, Interactable
{
    [SerializeField] string _message;
    [SerializeField] Color _messageColor;
    [SerializeField] PuzzleBase puzzle;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] bool clearedOnce = false;
    [SerializeField] GungeonGameManager gungeonGameManager;
    [SerializeField] private GameObject player;
    private GameObject finalSpawnPoint;
    private bool stageChecked = false;
    private bool preventPuzzleStageUpdate = false;

    void Start() {
        gungeonGameManager = GungeonGameManager.Instance;
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        puzzle = puzzleManager.CurrentPuzzle;
        finalSpawnPoint = GameObject.Find("FinalSpawnPoint");
    }



    void Update() {
        if (!stageChecked && gungeonGameManager.Stage == 4) {
            stageChecked = true;
            TeleportPlayerToTarget_Final(player, finalSpawnPoint);
        }

        if (puzzle.Cleared&&gungeonGameManager.Stage!=1&&!clearedOnce) {
            SystemMessageManager.Instance.PushSystemMessage("퍼즐 클리어!", Color.green, false, 2f);
            clearedOnce=true;
        }
    }

    private void HandleTrigger(Collider2D collision, bool isEntering) {
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log($"Player is Colliding to Portal {isEntering}");
            puzzleManager.IsInteractingToPortal = isEntering;
            if (puzzleManager.CurrentPuzzleIndex == 0)
            {
                puzzleManager.ChangePuzzle();
                puzzle = puzzleManager.CurrentPuzzle;
            }
            if (GungeonGameManager.Instance != null&& puzzle.Cleared) {
                GungeonGameManager.Instance.SetIsGenerating(isEntering);
                Debug.Log($"isGenerating set to {isEntering}");
                if(!preventPuzzleStageUpdate)
                {
                    puzzleManager.ChangePuzzle();
                    preventPuzzleStageUpdate = true;
                }
            }
            else {
                Debug.LogError("GGM instance is null");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) => HandleTrigger(collision, true);
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
}
