using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    List<PuzzleBase> puzzleList;
    [SerializeField]
    PuzzleBase currentPuzzle;
    [SerializeField]
    int currentPuzzleIndex;
    [SerializeField]
    bool _isInteractingToPortal = false;

    public PuzzleBase CurrentPuzzle { get => currentPuzzle; }
    public bool IsInteractingToPortal { get { return _isInteractingToPortal; } set { _isInteractingToPortal = value; } }
    public int CurrentPuzzleIndex { get { return currentPuzzleIndex; } }
    void Start() {
        if (puzzleList.Count > 0) {
            currentPuzzle = puzzleList[0];
            currentPuzzleIndex = 0;
        }
    }

    void Update() {
        currentPuzzle.IsInteractingToPortal = IsInteractingToPortal;
    }

    public void ChangePuzzle() {
        if (currentPuzzleIndex < puzzleList.Count - 1) {
            currentPuzzleIndex++;
            currentPuzzle = puzzleList[currentPuzzleIndex];
        }
    }
    public void SetDoorInteractedOncesUnder(int stage, bool value)
    {
        for(int i=0;i<stage;i++)
        {
            puzzleList[i].DoorController.IsInteractedOnce= value;
            Debug.Log($"puzzleList{i} ON ");
        }
    }
}
