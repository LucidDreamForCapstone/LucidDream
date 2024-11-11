using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterDemon : MonsterBase {

    #region serialize field
    [SerializeField] private GameObject _fireballObj;
    [SerializeField] private float _fireSpeed;
    [SerializeField] private float _fireLastTime;
    //[SerializeField] private float _searchRange;
    //[SerializeField] private float _attackRange;
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
        _attackFuncList.Add(FireTask);
        _isFiring = false;
        _isFireReady = true;
        _fireDelay = 0.5f;
    }

    #endregion //mono func





    #region private funcs

    private async UniTaskVoid FireTask() {
        _isFireReady = false;
        Fire().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_fireCoolTime));
        _isFireReady = true;
        _attackStateList[0] = AttackState.Ready;
    }

    private async UniTaskVoid Fire() {
        _attackStateList[0] = AttackState.Attacking;
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
        _attackStateList[0] = AttackState.CoolTime;
    }

    private double CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion //private funcs
}