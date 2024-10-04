using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterDemon : MonsterBase {

    #region serialize field
    [SerializeField] private GameObject _fireballObj;
    [SerializeField] private float _fireSpeed;
    [SerializeField] private float _fireLastTime;
    [SerializeField] private float _searchRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _fireCoolTime;
    [SerializeField] private AudioClip attackSound;

    #endregion //serialize field





    #region private variable

    private bool _isFiring;
    private bool _isFireReady;
    private float _fireDelay;

    #endregion //private variable


    #region mono funcs 

    new private void OnEnable() {
        base.OnEnable();
        Spawn().Forget();
        _isFiring = false;
        _isFireReady = true;
        _fireDelay = 0.5f;
        _tree._monster = this;
        _tree = _tree.Clone();
    }

    private void Update() {
        //AttackMove();
        _tree.Update();
    }

    #endregion //mono func

    public override async UniTaskVoid Attack() {
        await FireTask();
    }

    #region protected funcs

    protected override void AttackMove() {
        double dist = CalculateManhattanDist(transform.position, _playerScript.transform.position);
        if (!_isFiring && !_isDead && !_isStun && _isSpawnComplete && dist < _searchRange) {
            Vector2 moveVec = _playerScript.transform.position - transform.position;

            if (moveVec.x < 0)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;

            if (dist > _attackRange)
                _rigid.velocity = moveVec.normalized * _moveSpeed;
            else if (_isFireReady)
                FireTask().Forget();
            else
                _rigid.velocity = Vector2.zero;
        }
        else
            _rigid.velocity = Vector2.zero;
    }

    #endregion //protected funcs




    #region private funcs

    private async UniTask FireTask() {
        _isFireReady = false;
        Fire().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_fireCoolTime));
        _isFireReady = true;
        _attackState = AttackState.Ready;
    }

    private async UniTaskVoid Fire() {
        _attackState = AttackState.Attacking;
        _isFiring = true;
        _animator.SetTrigger("Attack");
        PlaySound(attackSound);
        GameObject fireBall = ObjectPool.Instance.GetObject(_fireballObj);
        Bullet fireBallScript = fireBall.GetComponent<Bullet>();
        Vector2 fireDir = _playerScript.transform.position - transform.position;
        fireBall.transform.right = fireDir;
        fireBall.transform.position = transform.position + fireBall.transform.right;
        fireBallScript.SetPlayer(_playerScript);
        fireBallScript.SetSpeed(_fireSpeed);
        fireBallScript.SetDmg(_damage);
        fireBallScript.SetLastTime(_fireLastTime);
        fireBall.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_fireDelay));
        _isFiring = false;
        _attackState = AttackState.CoolTime;
    }

    private double CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion //private funcs
}