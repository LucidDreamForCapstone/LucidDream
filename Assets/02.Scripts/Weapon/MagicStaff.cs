using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MagicStaff : WeaponBase {
    #region serialized field

    [SerializeField] float _chargeInterval;
    [SerializeField] GameObject _projectileObj;
    [SerializeField] float _projectileSpeed;
    [SerializeField] float _basicRange;
    [SerializeField] float _feverLastTime;
    [SerializeField] int _feverApPlusRate;
    [SerializeField] protected AudioClip fever_Sound;
    #endregion // serialized field



    #region private variable

    int _chargeCount;

    #endregion //private variable


    #region mono funcs

    new private void Start() {
        base.Start();
        _chargeCount = 0;
    }

    #endregion // mono funcs




    #region protected funcs

    protected override void BasicAttackAnimation() {
        _playerScript.AttackNow(_basicDelay).Forget();
        _playerScript.ArmTrigger("Staff");
    }

    protected override void Skill1Animation() {
        //_playerScript.AttackNow(_delay1).Forget();
    }

    protected override void Skill2Animation() {
        //_playerScript.AttackNow(_delay2).Forget();
    }

    protected override void FeverSkillAnimation() {
        _playerScript.AttackNow(_feverDelay).Forget();
        _playerScript.ArmTrigger("Buff");
        PlaySound(fever_Sound);
        FeverSkill();
    }

    #endregion





    #region public funcs

    public override async UniTaskVoid ActivateBasicAttack() {
        if (_isBasicReady) {
            //_isBasicReady = false; 는 BasicAttackTask 안에 있음
            await BasicAttackTask();
            await UniTask.Delay(TimeSpan.FromSeconds(_basicCoolTime));
            _isBasicReady = true;
        }
    }

    public override void BasicAttack() {
        Vector2 fireDir = _playerScript.MoveDir;
        GameObject projectile = ObjectPool.Instance.GetObject(_projectileObj);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(_projectileSpeed);
        projectileScript.SetMultiplier(_chargeCount * 0.2f);
        float lastTime = _basicRange / _projectileSpeed;
        projectileScript.SetLastTime(lastTime);
        projectile.SetActive(true);
        _chargeCount = 0;
    }

    public override void Skill1() {
    }
    public override void Skill2() {
    }
    public override void FeverSkill() {
        FeverTask().Forget();
    }
    #endregion //public funcs





    #region private funcs  
    private async UniTask BasicAttackTask() {
        float timer = 0;
        while (Input.GetKey(KeyCode.Q) && !_playerScript.CheckStun() && !_playerScript.CheckPause()) {
            timer += Time.deltaTime;
            if (timer > _chargeInterval * (_playerScript.GetChargetCount() + 1)) {
                if (_isBasicReady)
                    _isBasicReady = false;
                _playerScript.ChargeGaugeUp();
            }
            await UniTask.NextFrame();
        }

        _chargeCount = _playerScript.GetChargetCount();
        if (_chargeCount > 0) {
            BasicAttackAnimation();
            _playerScript.ResetChargeGauge();
        }
    }
    private async UniTaskVoid FeverTask() {
        PlayerDataManager.Instance.SetApPlusRate(PlayerDataManager.Instance.Status._apPlusRate + _feverApPlusRate);
        await UniTask.Delay(TimeSpan.FromSeconds(_feverLastTime));
        PlayerDataManager.Instance.SetApPlusRate(PlayerDataManager.Instance.Status._apPlusRate - _feverApPlusRate);
    }

    #endregion //private funcs
}
