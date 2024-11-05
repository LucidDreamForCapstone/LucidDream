using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WoodenSword : WeaponBase {

    #region serialized field

    [SerializeField] float _basicRange;
    [SerializeField] float _basicAngle;

    #endregion // serialized field



    

    #region private variable


    #endregion // private variable






    #region property

    #endregion // property variable




    #region protected funcs

    protected override void BasicAttackAnimation() {
        _playerScript.AttackNow(_basicDelay).Forget();
        _playerScript.ArmTrigger("Sword");
        PlaySound(_normalAttackSound);
    }

    protected override void Skill1Animation() {
        //_playerScript.AttackNow(_delay1).Forget();
    }

    protected override void Skill2Animation() {
        //_playerScript.AttackNow(_delay2).Forget();
    }

    protected override void FeverSkillAnimation() {
        //_playerScript.AttackNow(_feverDelay).Forget();
    }

    #endregion





    #region public funcs

    public override void BasicAttack() {
        Vector2 toMonster, lookAt = _playerScript.MoveDir;
        BasicSkillEffect(lookAt, 0.2f).Forget();
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _basicRange, _monsterLayer);
        List<Collider2D> possibleTargetList = possibleTargets.ToList();
        List<Collider2D> tempTargetList = new List<Collider2D>();
        int i, length = possibleTargetList.Count;
        float angle;
        if (length > 0) {
            Collider2D tempTarget;
            for (i = 0; i < length; i++) {//��ä�� ������ �ִ� �� ����
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
                _playerScript.PhysicalAttack(target.GetComponent<MonsterBase>(), 1);
            }
        }
    }

    public override void Skill1() {
    }
    public override void Skill2() {
    }
    public override void FeverSkill() {

    }
    #endregion //public funcs
    




    #region private funcs  

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

    #endregion //private funcs
}
