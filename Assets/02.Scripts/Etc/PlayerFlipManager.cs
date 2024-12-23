using UnityEngine;

public class PlayerFlipManager : MonoBehaviour {
    [SerializeField] bool _forAutoFlipDisable;
    [SerializeField] Player _player;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
            PlayerFlipManage();
    }

    private void PlayerFlipManage() {
        if (_forAutoFlipDisable) {
            _player.AutoFlipDisable();
            _player.SetFlipX(true);
        }
        else {
            _player.AutoFlipEnable();
        }
    }
}
