using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterStoneGolem : MonsterBase {
    #region serialize field

    [SerializeField] float _shieldCoolTime;
    [SerializeField] float _shieldLastTime;
    [SerializeField] int _defPlus;

    #endregion //serialize field





    #region private variable

    private bool _isShieldReady;
    private int _posIndex;

    #endregion //private variable


    #region mono funcs 

    new private void OnEnable() {
        base.OnEnable();
        Spawn().Forget();
        _isShieldReady = true;
        Shield().Forget();
    }

    #endregion //mono funcs


    #region public funcs

    public void SetPosIndex(int index) { _posIndex = index; }

    #endregion


    #region protected funcs

    protected override void AttackMove() {

    }

    protected override async UniTaskVoid Die() {
        _isDead = true;
        _hp = 0;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        _animator.SetTrigger("Die");
        PlaySound(_deathSound);
        PlayerDataManager.Instance.SetFeverGauge(PlayerDataManager.Instance.Status._feverGauge + _feverAmount);
        await UniTask.Delay(TimeSpan.FromSeconds(_dieDelay));
        BossHades.ReturnGolemPosUsing(_posIndex);
        Destroy(gameObject);
    }

    #endregion //protected funcs




    #region private funcs

    private async UniTaskVoid Shield() {
        await UniTask.Delay(TimeSpan.FromSeconds(4));

        while (!_isDead) {
            if (_isShieldReady) {
                _isShieldReady = false;
                _animator.SetBool("Shield", true);
                _def += _defPlus;
                await UniTask.Delay(TimeSpan.FromSeconds(_shieldLastTime));
                _def -= _defPlus;
                _animator.SetBool("Shield", false);
                await UniTask.Delay(TimeSpan.FromSeconds(_shieldCoolTime));
                _isShieldReady = true;
            }
            else
                await UniTask.NextFrame();
        }
    }

    #endregion //private funcs
}