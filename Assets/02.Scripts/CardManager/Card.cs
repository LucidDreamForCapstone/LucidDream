using System;

[Serializable]
public class Card {
    #region public variable
    public CardRank _cardRank;
    public CardProperty _cardProperty;
    public int _adRate; //공격력 증가 비율 %
    public int _apRate; //마력 증가 비율 %
    public int _critChance; //크리티컬 확률 증가 수치 (합적용)
    public int _critDamage; //크리티컬 데미지 증가 수치 (합적용)
    public int _moveSpeed; //이동속도 증가 비율 %
    public int _def; //방어력 증가 수치 (합적용)
    public int _hp; //체력 증가 수치 (합적용)
    public string _description = ""; //카드 설명
    #endregion // public variable





    #region public funcs

    public Card(CardRank cardRank, int adRate, int apRate, int critChance, int critDamage, int moveSpeed, int def, int hp) {
        int cnt = 0;
        _cardRank = cardRank;
        _adRate = adRate;
        if (_adRate != 0) {
            AppendDescription($"공격력 + {_adRate}%");
            _cardProperty = CardProperty.Ad;
            cnt++;
        }

        _apRate = apRate;
        if (_apRate != 0) {
            AppendDescription($"마력 + {_apRate}%");
            _cardProperty = CardProperty.Ap;
            cnt++;
        }

        _critChance = critChance;
        if (_critChance != 0) {
            AppendDescription($"치명타 확률 + {_critChance}%");
            _cardProperty = CardProperty.CritChance;
            cnt++;
        }

        _critDamage = critDamage;
        if (_critDamage != 0) {
            AppendDescription($"치명타 피해 + {_critDamage}%");
            _cardProperty = CardProperty.CritDamage;
            cnt++;
        }

        _moveSpeed = moveSpeed;
        if (_moveSpeed != 0) {
            AppendDescription($"이동 속도 + {_moveSpeed}%");
            _cardProperty = CardProperty.Speed;
        }

        _def = def;
        if (_def != 0) {
            AppendDescription($"방어력 + {_def}");
            _cardProperty = CardProperty.Def;
        }

        _hp = hp;
        if (_hp != 0) {
            AppendDescription($"HP + {_hp}");
            _cardProperty = CardProperty.Hp;
        }

        if (cnt == 4)
            _cardProperty = CardProperty.Mystic;
    }


    #endregion //public funcs





    #region private funcs

    private void AppendDescription(string addString) {
        if (_description.Equals(""))
            _description = addString;
        else
            _description += "\n" + addString;
    }

    #endregion //private funcs
}
