using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonsterBase {
    #region serialize field

    [SerializeField] protected GameObject _missileObj;
    [SerializeField] protected float _fireSpeed;
    [SerializeField] protected float _fireLastTime;
    [SerializeField] private float _fireCoolTime;
    [SerializeField] protected float _homingStartTime;
    [SerializeField] protected float _homingLastTime;
    [SerializeField] protected float _explodeRadius;
    [SerializeField] protected int _missileGroggyGaugeDecreaseAmount;
    [SerializeField] private int _groggyGaugeDecreaseAmount;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private Slider _groggySlider;
    protected GameObject _firePosObj;
    protected Vector2[] _firePosVec = { new Vector2(0.59f, 0.17f), new Vector2(-0.59f, 0.17f) };

    #endregion //serialize field





    #region private variable

    private bool _isFiring;
    private bool _isFireReady;
    private float _fireDelay;
    private bool _isGroggy;
    private float _groggyGauge;
    private GameObject _groggyEffectCache;

    #endregion //private variable


    #region mono funcs 
    private void Start() {
        _firePosObj = transform.GetChild(0).gameObject;
        _isGroggy = false;
        _groggyGauge = 100;
        UpdateGroggySlider();
    }
    new private void OnEnable() {
        _isEmbedded = true;
        base.OnEnable();
        _attackFuncList.Add(FireTask);
        _isFiring = false;
        _isFireReady = true;
        _fireDelay = 0.8f;
    }

    protected override void OnCollisionStay2D(Collision2D collision) {
        //No Body Damage
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
        await UniTask.Delay(TimeSpan.FromSeconds(_fireDelay));
        PlaySound(attackSound);
        FireHomingExplosive();
        _isFiring = false;
        _attackStateList[0] = AttackState.CoolTime;
    }

    virtual protected void FireHomingExplosive() {
        GameObject projectile = ObjectPool.Instance.GetObject(_missileObj);
        TurretMissile projectileScript = projectile.GetComponent<TurretMissile>();
        Vector2 targetDir = _playerScript.transform.position - transform.position;
        projectile.transform.right = Vector2.right;
        _firePosObj.transform.localPosition = _firePosVec[0];
        if (targetDir.x < 0) {
            projectile.transform.right *= -1;
            _firePosObj.transform.localPosition = _firePosVec[1];
        }

        projectile.transform.position = _firePosObj.transform.position;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_fireSpeed);
        projectileScript.SetDmg(_damage);
        projectileScript.SetExplodeRadius(_explodeRadius);
        projectileScript.SetLastTime(_fireLastTime);
        projectileScript.SetHomingStartTime(_homingStartTime);
        projectileScript.SetHomingLastTime(_homingLastTime);
        projectileScript.SetGroggyDecreaseAmount(_missileGroggyGaugeDecreaseAmount);
        projectile.SetActive(true);
    }

    public async UniTaskVoid DecreaseGroggyGauge(int decreaseAmount) {
        if (!_isGroggy) {
            float afterValue = _groggyGauge - decreaseAmount;
            if (afterValue < 0) {
                afterValue = 0;
                Groggy();
            }
            while (_groggyGauge >= afterValue) {
                _groggyGauge -= _groggyGaugeDecreaseAmount * Time.deltaTime;
                UpdateGroggySlider();
                await UniTask.NextFrame();
            }
            _groggyGauge = afterValue;
        }
    }

    virtual protected void Groggy() {
        _isGroggy = true;
        _useTree = false;
        _animator.SetTrigger("GroggyStart");
        StateEffectManager.Instance.SummonEffect(transform, StateType.Confusion, 0.5f, 10).Forget();
        _def = 200;
    }

    private void UpdateGroggySlider() {
        _groggySlider.value = _groggyGauge / 100.0f;
    }

    #endregion //private funcs
}
