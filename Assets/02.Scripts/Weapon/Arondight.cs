using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Arondight : WeaponBase {
    #region serialized field

    [SerializeField] float _basicRange;
    [SerializeField] float _basicAngle;
    [SerializeField] float _lastTime1;
    [SerializeField] float _defenseUpRate;
    [SerializeField] protected AudioClip wSound;
    [SerializeField] protected AudioClip feverSound;
    [SerializeField] protected AudioClip feverSound2;
    private PixelPerfectCamera pixelPerfectCamera;
    //[SerializeField] float _feverRange;

    #endregion // serialized field





    #region private variable


    #endregion // private variable






    #region property

    #endregion // property variable







    #region mono funcs

    new private void Start() {
        base.Start();
        pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
    }

    #endregion // mono funcs





    #region protected funcs

    protected override void BasicAttackAnimation() {
        _playerScript.AttackNow(_basicDelay).Forget();
        _playerScript.ArmTrigger("Sword");
        //BasicAttack();
        PlaySound(_normalAttackSound);
    }

    protected override void Skill1Animation() {
        _playerScript.AttackNow(_delay1).Forget();
        _playerScript.ArmTrigger("Buff");
        PlaySound(wSound);
        Skill1();
        // ī�޶� �� �� �� �� �ƿ�

    }



    protected override void Skill2Animation() {
        //_playerScript.AttackNow(_delay2).Forget();
    }

    protected override void FeverSkillAnimation() {
        _playerScript.AttackNow(_feverDelay).Forget();
        PlaySound(feverSound);
        DelayedSoundEffect(feverSound2, 0.7f).Forget();
        FeverSkill();
    }

    #endregion





    #region public funcs

    public override void BasicAttack() {
        Vector2 toMonster, lookAt = _playerScript.MoveDir;
        BasicSkillEffect(lookAt, 0.3f).Forget();
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _basicRange, _monsterLayer);
        List<Collider2D> possibleTargetList = possibleTargets.ToList();
        List<Collider2D> tempTargetList = new List<Collider2D>();
        List<MonsterBase> targetList = new List<MonsterBase>();

        Collider2D tempTarget;
        int i, index, cnt = 0, length = possibleTargetList.Count;
        float angle;
        if (length > 0) {
            for (i = 0; i < length; i++) {//��ä�� ������ �ִ� �� ����
                tempTarget = possibleTargetList[i];
                toMonster = tempTarget.transform.position - _playerScript.transform.position;
                angle = Vector2.Angle(lookAt, toMonster);
                if (angle < _basicAngle * 0.5f) {
                    tempTargetList.Add(tempTarget);
                }
            }

            while (tempTargetList.Count > 0 && cnt < 2) { //��ä�� ���� �ȿ��� ������ �ִ� �� �ִ� 2�� ����
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
            PassiveBonusActivate().Forget();
            for (i = 0; i < length; i++)//�������� Ÿ�ٿ��� ������
                _playerScript.PhysicalAttack(targetList[i], 1);
        }
    }

    public override void Skill1() {
        // �÷��̾� ��ġ���� Y������ 0.7��ŭ ������ ����Ʈ ����
        Vector3 effect0Position = _playerScript.transform.position + new Vector3(0, 0.7f, 0);
        GameObject playerEffect = Instantiate(_skillEffect1, effect0Position, Quaternion.identity);
        // �÷��̾� ������Ʈ�� �ڽ����� ����
        playerEffect.transform.SetParent(_playerScript.transform);
        Destroy(playerEffect, 1f);
        DefUp().Forget();
        // Pixel Perfect Camera �� ��/�� �ƿ� ����
        if (pixelPerfectCamera != null) {
            StartCoroutine(ZoomInAndOutStepCoroutine(20f));
        }
    }
    public override void Skill2() {
    }

    public override void FeverSkill() {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(_playerScript.transform.position, _feverRange, _monsterLayer);
        List<Collider2D> possibleTargetList = possibleTargets.ToList();
        List<MonsterBase> targetList = new List<MonsterBase>();
        int i, length, cnt = 0, index;

        // ���� ����� 4���� ���͸� ã�� ����
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

        // Passive Bonus ����
        PassiveBonusActivate().Forget();

        // �� Ÿ�ٿ��� Fever1 ��ų�� Fever2 ��ų�� ó��
        for (i = 0; i < targetList.Count; i++) {
            MonsterBase target = targetList[i];

            // Fever1 ��ų ���� (��� �����ϰ� ������ ����� ����)
            _playerScript.PhysicalAttack(target, 1); // ���� �����
            FeverEffect(target).Forget();
            DelayedFeverEffect(target).Forget();
        }
    }


    #endregion //public funcs





    #region private funcs  
    private IEnumerator ZoomInAndOutStepCoroutine(float targetPPU) {
        if (pixelPerfectCamera != null) {
            // ���� PPU ���� ����
            float originalPPU = pixelPerfectCamera.assetsPPU;

            // �� �� (2�� ����)
            while (pixelPerfectCamera.assetsPPU < targetPPU) {
                pixelPerfectCamera.assetsPPU += 1;
                if (pixelPerfectCamera.assetsPPU > targetPPU) {
                    pixelPerfectCamera.assetsPPU = (int)targetPPU; // �ִ밪 �ʰ� ����
                }
                yield return new WaitForSeconds(0.03f); // ���� �ӵ� ����
            }

            // ��� �ð� (�ִ밪 ����)
            yield return new WaitForSeconds(0.5f);

            // �� �ƿ� 
            while (pixelPerfectCamera.assetsPPU > originalPPU) {
                pixelPerfectCamera.assetsPPU -= 1;
                if (pixelPerfectCamera.assetsPPU < originalPPU) {
                    pixelPerfectCamera.assetsPPU = (int)originalPPU; // ���� �� �ʰ� ����
                }
                yield return new WaitForSeconds(0.03f); // ���� �ӵ� ����
            }
        }
    }
        private async UniTaskVoid DefUp() {
        int currentDef = PlayerDataManager.Instance.Status._def;
        int defPlus = (int)(currentDef * _defenseUpRate * 0.01f);
        PlayerDataManager.Instance.SetDef(currentDef + defPlus);
        await UniTask.Delay(TimeSpan.FromSeconds(_lastTime1), ignoreTimeScale: true);
        currentDef = PlayerDataManager.Instance.Status._def;
        PlayerDataManager.Instance.SetDef(currentDef - defPlus);
    }

    private async UniTaskVoid PassiveBonusActivate() {
        int currentMaxHp = PlayerDataManager.Instance.Status._maxHp;
        int currentAd = PlayerDataManager.Instance.Status._ad;
        int adPlus = (int)(currentMaxHp * 0.2f);
        PlayerDataManager.Instance.SetAd(currentAd + adPlus);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        currentAd = PlayerDataManager.Instance.Status._ad;
        PlayerDataManager.Instance.SetAd(currentAd - adPlus);
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

    // Fever1 ��ų ����Ʈ ����
    private async UniTaskVoid FeverEffect(MonsterBase target) {
        // Ÿ�� ��ġ�� Fever1 ��ų ����Ʈ ����
        GameObject feverEffect1 = Instantiate(_feverSkillEffect0, target.transform.position, Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: true);
        Destroy(feverEffect1); // 1�� �� ���� (���ϴ� �ð��� �°� ���� ����)
    }

    // Fever2 ��ų ����Ʈ ����
    private async UniTaskVoid DelayedFeverEffect(MonsterBase target) {
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f), ignoreTimeScale: true);
        // Ÿ�� ��ġ�� Fever2 ��ų ����Ʈ ����
        GameObject feverEffect2 = Instantiate(_feverSkillEffect1, target.transform.position, Quaternion.identity);
        // ������ ����� ���� (��ü ������� ������ ����)
        _playerScript.PhysicalAttack(target, 1); // ������ ���� �����
        await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true);
        // ���� �ð� �� Fever2 ��ų ����Ʈ ����
        Destroy(feverEffect2); // 1�� �� ���� (���ϴ� �ð��� �°� ���� ����)
    }

    private async UniTaskVoid DelayedSoundEffect(AudioClip clip, float delay) {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: true);
        PlaySound(clip);
    }
    #endregion //private funcs
}
