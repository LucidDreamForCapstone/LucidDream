using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfSword : WeaponBase {

    #region serialized field

    [SerializeField] float _basicRange;
    [SerializeField] float _basicAngle;
    [SerializeField] float _range1;
    [SerializeField] float _stunTime1;
    [SerializeField] float _range2;
    [SerializeField] float _angle2;
    [SerializeField] protected AudioClip wSound;
    [SerializeField] protected AudioClip eSound;
    //[SerializeField] float _feverRange;
    #endregion // serialized field





    #region private variable


    #endregion // private variable






    #region property

    #endregion // property variable







    #region mono funcs

    new private void Start() {
        base.Start();
    }

    #endregion // mono funcs




    #region protected funcs

    protected override void BasicAttackAnimation() {
        _playerScript.AttackNow(_basicDelay).Forget();
        _playerScript.ArmTrigger("Sword");
        PlaySound(nomalattackSound);
    }

    protected override void Skill1Animation() {
        _playerScript.AttackNow(_delay1).Forget();
        _playerScript.ArmTrigger("Buff");
        PlaySound(wSound);
        Skill1();
    }

    protected override void Skill2Animation() {

        _playerScript.AttackNow(_delay2).Forget();
        PlaySound(eSound);
        Skill2();
    }

    protected override void FeverSkillAnimation() {

        _playerScript.AttackNow(_feverDelay).Forget();
        FeverSkill();
    }

    #endregion



    #region public funcs

    public override void BasicAttack() {
        Vector2 toMonster, lookAt = _playerScript.MoveDir;
        BasicSkillEffect(lookAt, 0.6f).Forget();
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _basicRange, _monsterLayer);
        List<Collider2D> possibleTargetList = possibleTargets.ToList();
        List<Collider2D> tempTargetList = new List<Collider2D>();
        int i, length = possibleTargetList.Count;
        float angle;
        if (length > 0) {
            Collider2D tempTarget;
            for (i = 0; i < length; i++) {//부채꼴 영역에 있는 적 선별
                tempTarget = possibleTargetList[i];
                toMonster = tempTarget.transform.position - _playerScript.transform.position;
                angle = Vector2.Angle(lookAt, toMonster);
                if (angle < _basicAngle * 0.5f) {
                    tempTargetList.Add(tempTarget);
                }
            }

            length = tempTargetList.Count;
            if (length > 0) {
                Collider2D target = tempTargetList[0];
                double dist;
                double shortDist = CalculateManhattanDist(_playerScript.transform.position, target.transform.position);
                for (i = 1; i < length; i++) {
                    dist = CalculateManhattanDist(tempTargetList[i].transform.position, _playerScript.transform.position);
                    if (shortDist > dist) {
                        shortDist = dist;
                        target = tempTargetList[i];
                    }
                }
                _playerScript.PhysicalAttack(target.GetComponent<MonsterBase>(), 1.3f);
            }
        }
    }

    public override void Skill1() {
        Skill1Task().Forget();
    }

    public override void Skill2() {
        Vector2 toMonster, lookAt = _playerScript.MoveDir;

        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _range2, _monsterLayer);
        List<Collider2D> possibleTargetList = possibleTargets.ToList();
        List<Collider2D> tempTargetList = new List<Collider2D>();
        List<MonsterBase> targetList = new List<MonsterBase>();

        Collider2D tempTarget;
        int i, index, cnt = 0, length = possibleTargetList.Count;
        float angle;
        if (length > 0) {
            for (i = 0; i < length; i++) {//부채꼴 영역에 있는 적 선별
                tempTarget = possibleTargetList[i];
                toMonster = tempTarget.transform.position - _playerScript.transform.position;
                angle = Vector2.Angle(lookAt, toMonster);
                if (angle < _angle2 * 0.5f) {
                    tempTargetList.Add(tempTarget);
                }
            }

            while (tempTargetList.Count > 0 && cnt < 4) { //부채꼴 영역 안에서 가까이 있는 적 최대 4명 선별
                length = tempTargetList.Count;
                Collider2D target = tempTargetList[0];
                double dist;
                double shortDist = CalculateManhattanDist(_playerScript.transform.position, target.transform.position);
                index = 0;
                for (i = 1; i < length; i++) {
                    dist = CalculateManhattanDist(tempTargetList[i].transform.position, _playerScript.transform.position);
                    if (shortDist > dist) {
                        shortDist = dist;
                        target = tempTargetList[i];
                        index = i;
                    }
                }
                targetList.Add(target.GetComponent<MonsterBase>());
                tempTargetList.RemoveAt(index);
                cnt++;
            }

            length = targetList.Count;
            for (i = 0; i < length; i++)//최종선별 타겟에게 데미지
                _playerScript.PhysicalAttack(targetList[i], 1.3f);
        }

        SkillEffect2(lookAt).Forget();
    }

    public override void FeverSkill() {
        // 주변 몬스터 탐지
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _feverRange, _monsterLayer);
        int i, length = possibleTargets.Length;

        if (length > 0) {
            Collider2D target = possibleTargets[0];
            // 가장 가까운 몬스터 탐색
            double shortDist = CalculateManhattanDist(_playerScript.transform.position, target.transform.position);
            for (i = 1; i < length; i++) {
                double dist = CalculateManhattanDist(possibleTargets[i].transform.position, _playerScript.transform.position);
                if (shortDist > dist) {
                    shortDist = dist;
                    target = possibleTargets[i];
                }
            }
            // 대상에게 피지컬 공격
            _playerScript.PhysicalAttack(target.GetComponent<MonsterBase>(), 4.3f);

            // 대상 위치에 이펙트 생성
            Vector3 effectPosition = target.transform.position; // 대상의 위치
            GameObject feverSkillEffect = Instantiate(_feverSkillEffect0, effectPosition, Quaternion.identity);

            // 이펙트가 일정 시간 후 사라지도록 설정 (예: 1초 후)
            Destroy(feverSkillEffect, 0.8f);
        }
    }

    #endregion //public funcs




    #region private funcs

    private async UniTaskVoid Skill1Task() {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _range1, _monsterLayer);
        List<Collider2D> possibleTargetList = possibleTargets.ToList();
        List<MonsterBase> targetList = new List<MonsterBase>();
        int i, length, cnt = 0, index;
        while (possibleTargetList.Count > 0 && cnt < 4) {
            length = possibleTargetList.Count;
            Collider2D target = possibleTargetList[0];
            double dist;
            double shortDist = CalculateManhattanDist(_playerScript.transform.position, target.transform.position);
            index = 0;
            for (i = 1; i < length; i++) {
                dist = CalculateManhattanDist(possibleTargetList[i].transform.position, _playerScript.transform.position);
                if (shortDist > dist) {
                    shortDist = dist;
                    target = possibleTargetList[i];
                    index = i;
                }
            }
            targetList.Add(target.GetComponent<MonsterBase>());
            possibleTargetList.RemoveAt(index);
            cnt++;
        }
        SkillEffect1(0.4f).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(0.23f));
        length = targetList.Count;
        for (i = 0; i < length; i++)
            targetList[i].Stun(_stunTime1).Forget();
    }

    private async UniTaskVoid BasicSkillEffect(Vector2 lookAt, float offset) {
        float skillAngle = Vector2.SignedAngle(_playerScript.transform.right, lookAt);
        GameObject basicSkillEffect = ObjectPool.Instance.GetObject(_basicSkillEffect);
        basicSkillEffect.transform.SetParent(_playerScript.transform);
        basicSkillEffect.transform.right = lookAt;
        if (-1 < lookAt.x && lookAt.x <= 0) {
            if (lookAt.x < 0 && lookAt.y > 0)
                basicSkillEffect.transform.eulerAngles = new Vector3(0, 0, 45);
            else if (lookAt.x < 0 && lookAt.y < 0)
                basicSkillEffect.transform.eulerAngles = new Vector3(0, 0, -45);

            basicSkillEffect.transform.eulerAngles += new Vector3(0, 180, 0);
        }
        basicSkillEffect.transform.position = _playerScript.transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * skillAngle), Mathf.Sin(Mathf.Deg2Rad * skillAngle)) * offset;
        basicSkillEffect.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.25f), ignoreTimeScale: true);
        basicSkillEffect.transform.localRotation = Quaternion.identity;
        basicSkillEffect.transform.SetParent(null);
        ObjectPool.Instance.ReturnObject(basicSkillEffect);
    }

    private async UniTaskVoid SkillEffect1(float offsetY) {
        GameObject skillEffect = Instantiate(_skillEffect1, _playerScript.transform);
        SpriteRenderer sr = skillEffect.GetComponent<SpriteRenderer>();
        if (_playerScript.MoveDir.x > 0)
            sr.flipX = true;
        skillEffect.transform.position = _playerScript.transform.position + Vector3.up * offsetY;
        Debug.Log(skillEffect);
        await UniTask.Delay(TimeSpan.FromSeconds(1.2), ignoreTimeScale: true);
        Destroy(skillEffect);
    }

    private async UniTaskVoid SkillEffect2(Vector2 lookAt) {
        GameObject skillEffect = Instantiate(_skillEffect2);
        skillEffect.transform.right = lookAt;
        skillEffect.transform.position = _playerScript.transform.position;
        Debug.Log(skillEffect);
        await UniTask.Delay(TimeSpan.FromSeconds(0.6f), ignoreTimeScale: true);
        Destroy(skillEffect);
    }

    #endregion
}
