using UnityEngine;

public class PuzzleBase : MonoBehaviour
{
    [SerializeField]
    public bool cleared = false;

    public bool Cleared { get => cleared; }
}
