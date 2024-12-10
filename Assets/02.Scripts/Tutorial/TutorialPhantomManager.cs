using UnityEngine;

public class TutorialPhantomManager : MonoBehaviour {
    [SerializeField] bool _forPhantomLock;
    [SerializeField] Player _player;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
            PhantomManage();
    }

    private void PhantomManage() {
        if (_forPhantomLock) {
            _player.PhantomForceLock();
        }
        else {
            _player.PhantomForceUnLock();
        }
    }
}
