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

    public PuzzleBase CurrentPuzzle { get => currentPuzzle; }
    void Start()
    {
        if (puzzleList.Count > 0)
        {
            currentPuzzle = puzzleList[0];
            currentPuzzleIndex = 0;
        }
    }

    void Update()
    {
    }

    public void ChangePuzzle()
    {
        if (currentPuzzleIndex < puzzleList.Count - 1)
        {
            currentPuzzleIndex++;
            currentPuzzle = puzzleList[currentPuzzleIndex];
        }
    }
}
