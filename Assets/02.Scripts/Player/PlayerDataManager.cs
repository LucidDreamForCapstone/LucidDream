using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour {
    #region private variable

    private static PlayerDataManager _instance;

    #endregion // private variable





    #region properties

    public static PlayerDataManager Instance { get { return _instance; } }
    public int Stage { get { return _stage; } }
    public PlayerStatus Status { get { return _status; } }
    public Player Player { get { return _playerScript; } }

    #endregion // properties





    #region mono funcs

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }

    private void Update() {
        _player ??= GameObject.Find("Player");
        if (_player != null)
            _playerScript ??= _player.GetComponent<Player>();
    }

    #endregion // mono funcs





    #region private variables

    private int _stage;
    private GameObject _player;
    private Player _playerScript;
    [SerializeField] private PlayerStatus _status = new();
    private InGameUIController _ingameUIController = null;

    #endregion // private variables





    #region public funcs

    public void ResetGame() {
        _status = new PlayerStatus();
        SetPlayer().Forget();
    }

    private async UniTaskVoid SetPlayer() {
        await UniTask.Delay(TimeSpan.FromSeconds(8));
        _player ??= GameObject.Find("Player");
    }

    public void InitializeIngameUI(InGameUIController inGameUIController, System.Action callback) {
        _ingameUIController = inGameUIController;
        callback();
    }

    public void SetHP(int currHP) {
        _status._hp = currHP;
        _ingameUIController.SetHP(currHP, _status._maxHp);
    }

    public void HealAbs(int healAmount) {
        int currentHp = _status._hp;
        int afterHp = currentHp + healAmount;
        int maxHp = _status._maxHp;
        StateEffectManager.Instance.SummonEffect(_player.transform, StateType.FastHeal, 0, 1).Forget();
        if (afterHp > maxHp) {
            afterHp = maxHp;
            FloatingDamageManager.Instance.ShowDamage(_player, maxHp - currentHp, true, false, true);
        }
        else {
            FloatingDamageManager.Instance.ShowDamage(_player, healAmount, true, false, true);
        }
        SetHP(afterHp);
    }

    public void HealPercent(int percent) {
        int currentHp = _status._hp;
        int currentMaxHp = _status._maxHp;
        int healAmount = (int)(currentMaxHp * percent * 0.01f);
        int afterHp = currentHp + healAmount;
        StateEffectManager.Instance.SummonEffect(_player.transform, StateType.FastHeal, 0, 1).Forget();
        if (afterHp > currentMaxHp) {
            afterHp = currentMaxHp;
            FloatingDamageManager.Instance.ShowDamage(_player, currentMaxHp - currentHp, true, false, true);
        }
        else {
            FloatingDamageManager.Instance.ShowDamage(_player, healAmount, true, false, true);
        }
        SetHP(afterHp);
    }

    public void HealByMaxPercent(float percent) {
        int maxHP = _status._maxHp;
        int healAmount = Mathf.CeilToInt(maxHP * percent * 0.01f); // 최대 체력의 percent %를 계산
        int currentHp = _status._hp;
        int afterHp = currentHp + healAmount;

        if (afterHp > maxHP) {
            afterHp = maxHP;
            FloatingDamageManager.Instance.ShowDamage(_player, maxHP - currentHp, true, false, true);
        }
        else {
            FloatingDamageManager.Instance.ShowDamage(_player, healAmount, true, false, true);
        }

        SetHP(afterHp);
    }

    public void SetMaxHP(int maxHP) {
        _status._maxHp = maxHP;
    }

    public void SetDef(int def) {
        _status._def = def;
    }

    public void SetAd(int ad) {
        _status._ad = ad;
    }

    public void SetAp(int ap) {
        _status._ap = ap;
    }

    public void SetAdPlusRate(int adPlusRate) {
        _status._adPlusRate = adPlusRate;
    }

    public void SetApPlusRate(int apPlusRate) {
        _status._apPlusRate = apPlusRate;
    }

    public void SetCritChance(int critChance) {
        _status._critChance = critChance;
    }

    public void SetCritDamage(int critDamage) {
        _status._critDamage = critDamage;
    }

    public void SetMoveSpeed(float moveSpeed) {
        if (moveSpeed > Common.maxMoveSpeed)
            moveSpeed = Common.maxMoveSpeed;
        _status._moveSpeed = moveSpeed;
    }

    public void SetWeaponBonus(WeaponBasicData weaponBasicData) {
        _status._weaponAdBonus = weaponBasicData._weaponAd;
        _status._weaponApBonus = weaponBasicData._weaponAp;
        _status._weaponMoveSpeedBonus = weaponBasicData._weaponMoveSpeed;
        _status._weaponCritChanceBonus = weaponBasicData._weaponCritChance;
        _status._weaponCritDamageBonus = weaponBasicData._weaponCritDamage;
    }

    public void SetCoin(int coin) {
        _status._coin = coin;
        _ingameUIController.SetCoin(coin);
    }

    public void SetDream(int dream) {
        _status._dream = dream;
        _ingameUIController.SetDream(dream);
    }

    public void SetExp(int exp) {
        _status._exp = exp;
    }

    public void SetLevel(int level) {
        _status._playerLevel = level;
        _ingameUIController.SetLevel(level);
    }

    public void SetFeverGauge(int feverGauge) {
        if (feverGauge > _status._maxFeverGauge)
            feverGauge = _status._maxFeverGauge;
        _status._feverGauge = feverGauge;
        _ingameUIController.SetFever(feverGauge, _status._maxFeverGauge);
        if (_status._feverGauge == _status._maxFeverGauge) {
            // InGameUIController의 Fill Amount를 1로 설정
            _ingameUIController.SetFever(_status._feverGauge, _status._maxFeverGauge);
        }
    }

    public void SetMaxFeverGauge() {
        int level = _status._playerLevel;
        if (0 < level && level < 10)
            _status._maxFeverGauge = Common.baseFeverMaxGauge;
        else if (10 <= level && level < 20)
            _status._maxFeverGauge = Common.secondFeverMaxGauge;
        else if (20 <= level && level < 30)
            _status._maxFeverGauge = Common.thirdFeverMaxGauge;
        else
            Debug.Log("임시");

        if (_status._maxFeverGauge < _status._feverGauge)
            SetFeverGauge(_status._maxFeverGauge);
    }

    public bool IsFeverReady() {
        if (_status._feverGauge == _status._maxFeverGauge)
            return true;
        else
            return false;
    }

    public void ShowCardSelectUI(int levelUpCount) {
        if (0 < levelUpCount) {
            _ingameUIController.ShowCardSelect(CardManage.Instance.DrawCards(), levelUpCount);
        }
    }

    public int GetFinalAd() {
        int basicAd = _status._ad;
        int weaponAd = _status._weaponAdBonus;
        int adPlusRate = _status._adPlusRate;

        float finalAd = (basicAd + weaponAd) * (1 + adPlusRate * 0.01f);
        return (int)finalAd;
    }

    public int GetFinalAp() {
        int basicAp = _status._ap;
        int weaponAp = _status._weaponApBonus;
        int apPlusRate = _status._apPlusRate;

        float finalAp = (basicAp + weaponAp) * (1 + apPlusRate * 0.01f);
        return (int)finalAp;
    }

    public float GetFinalMoveSpeed() {
        float basicMoveSpeed = _status._moveSpeed;
        int weaponMoveSpeed = _status._weaponMoveSpeedBonus;

        float finalMoveSpeed = basicMoveSpeed * (1 + weaponMoveSpeed * 0.01f);

        if (finalMoveSpeed > Common.maxMoveSpeed)
            finalMoveSpeed = Common.maxMoveSpeed;

        // 소숫점 둘째 자리까지 반올림
        finalMoveSpeed = Mathf.Round(finalMoveSpeed * 100f) / 100f;

        return finalMoveSpeed;
    }


    public int GetFinalCritChance() {
        int basicCritChance = _status._critChance;
        int weaponCritChance = _status._weaponCritChanceBonus;

        int finalCritChance = basicCritChance + weaponCritChance;

        return finalCritChance;
    }

    public int GetFinalCritDamage() {
        int basicCritDamage = _status._critDamage;
        int weaponCritDamage = _status._weaponCritDamageBonus;

        int finalCritDamage = basicCritDamage + weaponCritDamage;

        return finalCritDamage;
    }

    public bool BuyItem(int price) {
        if (price > _status._coin) {
            return false;
        }
        _status._coin -= price;
        _ingameUIController.SetCoin(_status._coin); // UI에 코인 갱신
        return true;
    }

    /*public async UniTaskVoid UseFeverSkill(int feverDuration = 0) {
        // 피버 스킬 사용: 게이지를 초기화하고 UI를 동기화
        SetFeverGauge(0);

        // 스킬 지속 시간 동안 대기
        await UniTask.Delay(TimeSpan.FromSeconds(feverDuration));

        Debug.Log("Fever Skill Ended");
    }

    public void AddFeverGauge(int value) {
        // 몬스터 처치 시 피버 게이지 추가
        int newFeverGauge = _status._feverGauge + value;

        // 최대 게이지 초과 방지
        if (newFeverGauge > _status._maxFeverGauge) {
            newFeverGauge = _status._maxFeverGauge;
        }

        SetFeverGauge(newFeverGauge); // UI 동기화
    }

    public void SetFeverGauge(int feverGauge) {
        // 피버 게이지와 UI 동기화
        if (feverGauge > _status._maxFeverGauge)
            feverGauge = _status._maxFeverGauge;

        _status._feverGauge = feverGauge;

        // UI 업데이트
        _ingameUIController.SetFever(feverGauge, _status._maxFeverGauge);
    } */


    #endregion // public funcs





    #region privete funcs



    #endregion private funcs


    #region test code
    /*
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShowCardSelectUI(1);
        }
    }
    */
    #endregion // test code
}