using UnityEngine;

public class PuzzleBase : MonoBehaviour
{
    [SerializeField]
    private bool cleared = false;

    public bool Cleared { get { return cleared; } set { cleared=value; } }
}
