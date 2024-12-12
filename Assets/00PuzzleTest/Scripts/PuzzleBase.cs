using UnityEngine;

public class PuzzleBase : MonoBehaviour {
    [SerializeField]
    protected bool cleared = false;
    [SerializeField]
    protected bool isInteractingToPortal = false;
    [SerializeField]
    DoorControl doorController;
    public bool Cleared { get { return cleared; } set { cleared = value; } }
    public bool IsInteractingToPortal { get { return isInteractingToPortal; } set { isInteractingToPortal = value; } }
    public DoorControl DoorController { get { return doorController; } }
}
