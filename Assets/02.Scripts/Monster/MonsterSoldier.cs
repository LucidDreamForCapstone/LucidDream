using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterSoldier : MonsterBase {
    #region serialize field
    [SerializeField] private GameObject _bulletObj;
    [SerializeField] private int _fireCount;
    [SerializeField] private float _fireSpeed;
    [SerializeField] private float _fireLastTime;
    [SerializeField] private float _fireCoolTime;
    [SerializeField] private float _fireDelay;
    [SerializeField] private AudioClip attackSound;

    #endregion //serialize field





    #region private variable

    private bool _isFireReady;

    #endregion //private variable


    #region mono funcs 

    new private void OnEnable() {
        base.OnEnable();
        _attackFuncList.Add(FireTask);
        _isFireReady = true;
    }

    #endregion //mono func





    #region private funcs

    private async UniTaskVoid FireTask() {
        _isFireReady = false;
        FireMultiple().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_fireCoolTime));
        _isFireReady = true;
        _attackStateList[0] = AttackState.Ready;
    }

    private async UniTaskVoid FireMultiple() {
        _attackStateList[0] = AttackState.Attacking;
        for (int i = 0; i < _fireCount; i++) {
            SetFlipX();
            Fire();
            await UniTask.Delay(TimeSpan.FromSeconds(_fireDelay));
        }
        _attackStateList[0] = AttackState.CoolTime;
    }

    private void Fire() {
        _animator.SetTrigger("Attack");
        PlaySound(attackSound);
        GameObject fireBall = ObjectPool.Instance.GetObject(_bulletObj);
        Bullet fireBallScript = fireBall.GetComponent<Bullet>();
        Vector2 fireDir = _playerScript.transform.position - transform.position;
        fireBall.transform.right = fireDir;
        fireBall.transform.position = transform.position + fireBall.transform.right;
        fireBallScript.SetPlayer(_playerScript);
        fireBallScript.SetSpeed(_fireSpeed);
        fireBallScript.SetDmg(_damage);
        fireBallScript.SetLastTime(_fireLastTime);
        fireBall.SetActive(true);
    }

    private void SetFlipX() {
        Vector2 fireDir = _playerScript.transform.position - transform.position;
        if (fireDir.x >= 0)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;
    }

    #endregion //private funcs
}
