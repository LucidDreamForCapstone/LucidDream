using UnityEngine;

public class SwapBlockTrigger : MonoBehaviour {
    [SerializeField] bool _forSwapBlock;
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (_forSwapBlock)
                PlayerSwapManager.Instance.SetCanSwap(false);
            else
                PlayerSwapManager.Instance.SetCanSwap(true);
        }
    }
}
