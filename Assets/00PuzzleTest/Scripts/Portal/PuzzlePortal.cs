using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class PuzzlePortal : MonoBehaviour, Interactable
{
    // Start is called before the first frame update
    [SerializeField] string _message;
    [SerializeField] Color _messageColor;
    [SerializeField] PuzzleBase puzzle;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] bool clearedOnce = false;

    void Start()
    {
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        puzzle = puzzleManager.CurrentPuzzle;
    }

    void Update()
    {
        if (puzzle.Cleared && clearedOnce == false)
        {
            puzzleManager.ChangePuzzle();
            clearedOnce = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && InteractManager.Instance.CheckInteractable(this))
        {
            if (GungeonGameManager.Instance != null)
            {
                GungeonGameManager.Instance.SetIsGenerating(true);
                SystemMessageManager.Instance.PushSystemMessage(_message, _messageColor, false, 1.5f);
                Debug.Log("isGenerating set to true");
            }
            else
            {
                Debug.LogError("GGM instance is null");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GungeonGameManager.Instance != null)
            {
                GungeonGameManager.Instance.SetIsGenerating(false);
                Debug.Log("isGenerating set to false");
            }
            else
            {
                Debug.LogError("GGM instance is null");
            }
        }
    }

    public bool IsInteractBlock()
    {
        return !clearedOnce;
        //��ȣ�ۿ��� ���ƾ��ϴ� ��Ȳ�� ���� ����ٰ� �� ������ ���� ��
    }

    public string GetInteractText()
    {
        return "�̵� (G)";
    }
}
