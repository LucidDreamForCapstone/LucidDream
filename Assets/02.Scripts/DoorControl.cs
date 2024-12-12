using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private bool isdoorOpen;
    [SerializeField] private GameObject labDoor; // ´ÝÈù ¹®
    [SerializeField] private GameObject openDoor; // ¿­¸° ¹®
    [SerializeField] private bool isInteractingToPortal = false;
    [SerializeField]private bool isInteractedOnce = false;
    [SerializeField] private int Stage = 0;
    public bool IsInteractingToPortal { get { return isInteractingToPortal; } set { isInteractingToPortal = value; } }
    public bool IsInteractedOnce { get { return isInteractedOnce; } set { isInteractedOnce= value; } }
    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isdoorOpen&& isInteractedOnce)
            DoorOpen();
    }

    private void DoorOpen()
    {
        if (Input.GetKey(KeyCode.G)) // 'G' Å°¸¦ ´­·¶À» ¶§
        {
            Player2_InteractManager.Instance.InteractCoolTime().Forget(); // ÄðÅ¸ÀÓ Ã³¸®
            if (labDoor != null && openDoor != null)
            {
                labDoor.SetActive(false); // ´ÝÈù ¹® ºñÈ°¼ºÈ­
                openDoor.SetActive(true); // ¿­¸° ¹® È°¼ºÈ­
            }
        }
    }


    public void DoorOpenOnGameClear(bool cleared)
    {
        isdoorOpen = cleared;
    }
}

