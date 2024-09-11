using System;

[Serializable]
public class PlayerStatus {
    #region public variables

    public string _nickName;
    public int _playerLevel;
    public int _maxLevel;
    public int _hp;
    public int _maxHp;
    public int _exp;
    //public int _maxExp;
    public float _moveSpeed; //5~10
    public int _ad;
    public int _ap;
    public int _adPlusRate;//카드 효과 + 일시적인 버프 효과
    public int _apPlusRate;//카드 효과 + 일시적인 버프 효과
    public int _def;
    public int _critChance;
    public int _critDamage;
    public int _weaponAdBonus;//무기에 의한 공격력 수치 증가 (비율 아니고 깡공격력)
    public int _weaponApBonus;//무기에 의한 마력 수치 증가 (비율 아니고 깡마력)
    public int _weaponMoveSpeedBonus;
    public int _weaponCritChanceBonus;
    public int _weaponCritDamageBonus;
    public int _feverGauge;
    public int _maxFeverGauge;
    public int _coin;
    public int _dream;
    #endregion





    #region public funcs

    public PlayerStatus() {
        _nickName = "테스트";
        _playerLevel = Common.baseLevel;
        _maxLevel = Common.maxLevel;
        _hp = Common.baseHp;
        _exp = 0;
        _moveSpeed = Common.baseMoveSpeed;
        _maxHp = Common.baseHp;
        //_maxExp = MaxEXP(_playerLevel);
        _ad = Common.baseAd;
        _ap = Common.baseAp;
        _adPlusRate = 0;
        _apPlusRate = 0;
        _def = Common.baseDef;
        _critChance = Common.baseCritChance;
        _critDamage = Common.baseCritDamage;
        _weaponAdBonus = 0;
        _weaponApBonus = 0;
        _weaponMoveSpeedBonus = 5;
        _weaponCritChanceBonus = 0;
        _weaponCritDamageBonus = 0;
        _feverGauge = 0;
        _maxFeverGauge = Common.baseFeverMaxGauge;
        _coin = 0;
        _dream = 0;
    }

    public int GetMaxExp() {
        if (0 < _playerLevel && _playerLevel < 9) //3 5 7 9 11 13 15 17
        {
            return _playerLevel * 2 + 1;
        }
        else if (9 <= _playerLevel && _playerLevel < 20) //20 22 24 26 28 30 32 34 36 38 40
        {
            return _playerLevel * 2 + 2;
        }
        else if (20 <= _playerLevel && _playerLevel < 29) //45 50 55 60 65 70 75 80 85
        {
            return _playerLevel * 5 - 55;
        }
        else if (_playerLevel == 29) {
            return 100;
        }
        else {
            return -1;
        }
    }

    #endregion // public funcs




    #region private funcs
    /*
    private int MaxEXP(int level) {
        return level * 10;
    }
    */
    #endregion // private funcs
}