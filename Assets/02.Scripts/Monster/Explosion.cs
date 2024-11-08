using UnityEngine;

public class Explosion : MonoBehaviour {
    [SerializeField] float _radius;
    Animator _animator;
    LayerMask _playerLayer;
    Player _player;
    int _damage;

    private void Start() {
        _animator = GetComponent<Animator>();
        _playerLayer = LayerMask.GetMask("Player");
    }

    public void ExplodeTrigger() {
        _animator.SetTrigger("Explosion");

    }

    public void Explode() {
        Debug.Log("Exploding");
        var target = Physics2D.OverlapCircle(transform.position, _radius, _playerLayer);
        if (target != null) {
            _player.DamagedAbs(_damage);
        }
    }

    public void FadeTrigger() {
        _animator.SetTrigger("Fade");
    }

    public void SetDamage(int damage) {
        _damage = damage;
    }

    public void SetPlayer(Player player) {
        _player = player;
    }
}
