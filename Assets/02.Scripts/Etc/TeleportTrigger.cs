using UnityEngine;

public class TeleportTrigger : MonoBehaviour {
    [SerializeField] GameObject _teleportObj;
    [SerializeField] Player _player;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            _player.transform.position = _teleportObj.transform.position;
        }
    }
}
