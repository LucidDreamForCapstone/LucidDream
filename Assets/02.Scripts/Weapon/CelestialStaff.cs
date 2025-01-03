using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CelestialStaff : WeaponBase {
    #region serialized field
    [SerializeField] float _chargeInterval;
    [SerializeField] int _healChance;
    [SerializeField] int _healAmount;
    [SerializeField] GameObject _basicProjectileObj;
    [SerializeField] float _basicProjectileSpeed;
    [SerializeField] float _basicRange;
    [SerializeField] GameObject _skillProjectileObj1;
    [SerializeField] float _skillProjectileSpeed1;
    [SerializeField] float _range1;
    [SerializeField] GameObject _skillProjectileObj2;
    [SerializeField] float _skillProjectileSpeed2;
    [SerializeField] float _range2;
    [SerializeField] float _radius2;
    [SerializeField] protected AudioClip feverSound;
    [SerializeField] protected AudioClip wSound;
    [SerializeField] protected AudioClip eSound;
    //[SerializeField] float _feverRange;

    #endregion // serialized field





    #region private variable

    int _chargeCount;

    #endregion // private variable






    #region property

    #endregion // property variable







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
        PlaySoundDelay(_normalAttackSound, 0.2f).Forget();
    }

    protected override void Skill1Animation() {
        _playerScript.AttackNow(_delay1).Forget();
        _playerScript.ArmTrigger("StaffSkill");
        PlaySoundDelay(wSound, 0.25f).Forget();
    }

    protected override void Skill2Animation() {
        _playerScript.AttackNow(_delay2).Forget();
        PlaySound(eSound);
        Skill2();
    }

    protected override void FeverSkillAnimation() {
        _playerScript.AttackNow(_feverDelay).Forget();
        _playerScript.ArmTrigger("Buff");
        PlaySound(feverSound);
        FeverSkill();
    }

    #endregion





    #region public funcs

    public override async UniTaskVoid ActivateBasicAttack() {
        if (_isBasicReady) {
            //_isBasicReady = false; 는 BasicAttackTask 안에 있음
            await BasicAttackTask();
            await UniTask.Delay(TimeSpan.FromSeconds(_basicCoolTime), ignoreTimeScale: true);
            _isBasicReady = true;
        }
    }

    public override void BasicAttack() {
        Fire(_basicProjectileObj, _playerScript.MoveDir, _chargeCount * 0.2f, _basicRange, _basicProjectileSpeed);
        HealPassive();
        _chargeCount = 0;
    }

    public override void Skill1() {
        int i;
        float angle;
        Vector2 lookAt = _playerScript.MoveDir;
        for (i = -1; i < 2; i++) {
            angle = 45 * i + Vector2.SignedAngle(Vector2.right, lookAt);
            Vector2 fireDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
            Fire(_skillProjectileObj1, fireDir, 2, _range1, _skillProjectileSpeed1);
        }
        HealPassive();
    }

    public override void Skill2() {
        FireExplosion(_skillProjectileObj2, _playerScript.MoveDir, 1.5f, _range2, _skillProjectileSpeed2, _radius2);
        HealPassive();
    }

    public override void FeverSkill() {
        FeverSkillEffect().Forget();
        // 주변 몬스터 탐지
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _feverRange, _monsterLayer);
        int i, length = possibleTargets.Length;

        if (length > 0) {
            for (i = 0; i < length; i++) {
                MonsterBase targetMonster = possibleTargets[i].GetComponent<MonsterBase>();

                if (targetMonster != null) {
                    // 피격 대상에게 마법 공격
                    _playerScript.NormalMagicalAttack(targetMonster, 3);

                    // 0.3초 대기 후 적 위치에 이펙트 생성
                    SpawnDelayedEffect(targetMonster).Forget();
                }
            }
        }
    }


    #endregion //public funcs





    #region private funcs  

    private async UniTask BasicAttackTask() {
        float timer = 0;
        while (Input.GetKey(KeyCode.Q) && !_playerScript.CheckStun() && !_playerScript.CheckPause()) {
            timer += Time.unscaledDeltaTime;
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

    private void Fire(GameObject projectileObj, Vector2 fireDir, float multiplier, float range, float speed) {
        GameObject projectile = ObjectPool.Instance.GetObject(projectileObj);
        Bullet projectileScript = projectile.GetComponent<Bullet>();
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(speed);
        projectileScript.SetMultiplier(multiplier);
        float lastTime = range / speed;
        projectileScript.SetLastTime(lastTime);
        projectile.SetActive(true);
    }

    private void FireExplosion(GameObject projectileObj, Vector2 fireDir, float multiplier, float range, float speed, float radius) {
        GameObject projectile = ObjectPool.Instance.GetObject(projectileObj);
        var projectileScript = projectile.GetComponent<ExplosiveBullet>();
        projectile.transform.right = fireDir;
        projectile.transform.position = transform.position;
        projectileScript.SetPlayer(_playerScript);
        projectileScript.SetSpeed(speed);
        projectileScript.SetMultiplier(multiplier);
        projectileScript.SetExplodeRadius(radius);
        float lastTime = range / speed;
        projectileScript.SetLastTime(lastTime);
        projectile.SetActive(true);
    }

    private void HealPassive() {
        float randomN = Random.Range(0, 1f);
        if (randomN < _healChance * 0.01f)
            PlayerDataManager.Instance.HealAbs(_healAmount);
    }

    private async UniTaskVoid FeverSkillEffect() {
        // 플레이어 위치에서 Y축으로 0.7만큼 위에서 이펙트 생성
        Vector3 effect0Position = _playerScript.transform.position + new Vector3(0, 0.7f, 0);
        GameObject playerEffect = Instantiate(_feverSkillEffect0, effect0Position, Quaternion.identity);
        // 플레이어 오브젝트의 자식으로 설정
        playerEffect.transform.SetParent(_playerScript.transform);
        // 일정 시간 후 이펙트 제거 (예: 2초)
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true);
        Destroy(playerEffect);
    }

    private async UniTaskVoid SpawnDelayedEffect(MonsterBase targetMonster) {
        // 0.3초 대기
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        // 적 위치에 이펙트 생성 (_feverSkillEffect1)
        GameObject monsterEffect = Instantiate(_feverSkillEffect1, targetMonster.transform.position, Quaternion.identity);

        // 일정 시간 후 이펙트 제거 (예: 0.5초)
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        Destroy(monsterEffect);
    }
    #endregion //private funcs
}
