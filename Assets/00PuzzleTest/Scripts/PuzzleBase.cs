using UnityEngine;

public class PuzzleBase : MonoBehaviour {
    [SerializeField]
    protected bool cleared = false;
    [SerializeField]
    protected bool isInteractingToPortal = false;
    public bool Cleared { get { return cleared; } set { cleared = value; } }
    public bool IsInteractingToPortal { get { return isInteractingToPortal; } set { isInteractingToPortal = value; } }
}
