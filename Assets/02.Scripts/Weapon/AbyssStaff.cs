using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

public class AbyssStaff : WeaponBase {
    #region serialized field

    [SerializeField] float _chargeInterval;
    [SerializeField] GameObject _basicProjectileObj;
    [SerializeField] float _basicProjectileSpeed;
    [SerializeField] float _basicRange;
    [SerializeField] GameObject _skillProjectileObj;
    [SerializeField] float _skillProjectileSpeed;
    [SerializeField] float _range1;
    [SerializeField] protected AudioClip wSound;
    [SerializeField] protected AudioClip fever_Sound;
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
    }

    protected override void Skill1Animation() {
        _playerScript.AttackNow(_delay1).Forget();
        PlaySound(wSound);
        Skill1();
    }

    protected override void Skill2Animation() {
        //_playerScript.AttackNow(_delay2).Forget();
    }

    protected override void FeverSkillAnimation() {
        _playerScript.AttackNow(_feverDelay).Forget();
        PlaySound(fever_Sound);
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
        _chargeCount = 0;
    }

    public override void Skill1() {
        int i;
        float angle;
        Vector2 lookAt = _playerScript.MoveDir;
        for (i = -1; i < 2; i++) {
            angle = 30 * i + Vector2.SignedAngle(Vector2.right, lookAt);
            Vector2 fireDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
            Fire(_skillProjectileObj, fireDir, 1.5f, _range1, _skillProjectileSpeed);
        }
    }

    public override void Skill2() { }

    public override void FeverSkill() {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _feverRange, _monsterLayer);
        int length = possibleTargets.Length;

        if (length > 0) {
            // 가장 가까운 적 찾기
            Collider2D target = GetClosestTarget(possibleTargets);

            if (target != null) {
                MonsterBase targetMonster = target.GetComponent<MonsterBase>();
                Vector3 initialPosition = targetMonster.transform.position; // 몬스터의 초기 위치 저장
                float multiplier = 3;

                if (CheckHpIsLessThanHalf()) {
                    multiplier += 0.25f;
                }

                // Effect0 생성 및 Fade In/Out 효과
                StartCoroutine(SpawnAndFadeEffect0(initialPosition));

                // 보스가 아닌 몬스터만 끌어당기기
                if (!targetMonster.CheckBoss()) {
                    // 몬스터를 초기 위치로 끌어당김
                    StartCoroutine(PullMonsterTowardsInitialPosition(targetMonster, initialPosition, 1f)); // 1초 동안 끌어당김
                }

                // Effect0가 유지되는 동안 시간별로 대미지 분할 적용
                StartCoroutine(ApplySplitDamage(targetMonster, multiplier, 2)); // 2초 동안 대미지 분할 적용

                // 0.7초 후 Effect1 생성 및 대미지 적용
                StartCoroutine(SpawnEffect1WithDelay(targetMonster, multiplier));
            }
        }
    }

    #endregion // public funcs





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
        if (CheckHpIsLessThanHalf()) multiplier += 0.25f;
        projectileScript.SetMultiplier(multiplier);
        float lastTime = range / speed;
        projectileScript.SetLastTime(lastTime);
        projectile.SetActive(true);
    }

    private bool CheckHpIsLessThanHalf() {
        float hpPercentage = (float)PlayerDataManager.Instance.Status._hp / (float)PlayerDataManager.Instance.Status._maxHp;
        return hpPercentage <= 0.5f;
    }

    // 가장 가까운 몬스터를 찾는 함수
    private Collider2D GetClosestTarget(Collider2D[] possibleTargets) {
        Collider2D closestTarget = possibleTargets[0];
        double shortestDistance = CalculateManhattanDist(_playerScript.transform.position, closestTarget.transform.position);

        int length = possibleTargets.Length;
        for (int i = 0; i < length; i++) {
            double distance = CalculateManhattanDist(_playerScript.transform.position, possibleTargets[i].transform.position);
            if (distance < shortestDistance) {
                shortestDistance = distance;
                closestTarget = possibleTargets[i];
            }
        }

        return closestTarget;
    }

    // Effect0를 생성하고, Fade In/Out 후 삭제하는 코루틴
    private IEnumerator SpawnAndFadeEffect0(Vector3 initialPosition) {
        GameObject effect0 = Instantiate(_feverSkillEffect0, initialPosition, Quaternion.identity);

        // 페이드 인/아웃 효과 실행
        yield return StartCoroutine(FadeInOut(effect0));

        // 이펙트 제거
        Destroy(effect0);
    }

    // Effect1을 0.7초 후에 생성하고 대미지를 입히는 코루틴
    private IEnumerator SpawnEffect1WithDelay(MonsterBase target, float damageMultiplier) {
        yield return new WaitForSeconds(0.7f);

        GameObject effect1 = Instantiate(_feverSkillEffect1, target.transform.position, Quaternion.identity);

        _playerScript.NormalMagicalAttack(target, damageMultiplier);

        Destroy(effect1, 1.5f);
    }

    // 몬스터를 초기 위치로 끌어당기는 코루틴
    private IEnumerator PullMonsterTowardsInitialPosition(MonsterBase target, Vector3 initialPosition, float duration) {
        Vector3 startPos = target.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            target.transform.position = Vector3.Lerp(startPos, initialPosition, elapsedTime / duration);
            yield return null;
        }
    }

    // 대미지를 분할하여 적용하는 코루틴
    private IEnumerator ApplySplitDamage(MonsterBase target, float totalDamage, float duration) {
        float damagePerInterval = totalDamage / (duration / 0.5f); // 0.5초마다 대미지 적용
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            elapsedTime += 0.5f;
            _playerScript.NormalMagicalAttack(target, damagePerInterval);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FadeInOut(GameObject effect) {
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();

        // 페이드 인
        for (float t = 0; t < 1; t += Time.deltaTime / 1f) {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(0, 1, t);
            spriteRenderer.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // 페이드 아웃
        for (float t = 0; t < 1; t += Time.deltaTime / 1f) {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(1, 0, t);
            spriteRenderer.color = color;
            yield return null;
        }
    }
    #endregion // private funcs
}