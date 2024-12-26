using System;

[Serializable]
public class Card {
    #region public variable
    public CardRank _cardRank;
    public CardProperty _cardProperty;
    public int _adRate; //���ݷ� ���� ���� %
    public int _apRate; //���� ���� ���� %
    public int _critChance; //ũ��Ƽ�� Ȯ�� ���� ��ġ (������)
    public int _critDamage; //ũ��Ƽ�� ������ ���� ��ġ (������)
    public int _moveSpeed; //�̵��ӵ� ���� ���� %
    public int _def; //���� ���� ��ġ (������)
    public int _hp; //ü�� ���� ��ġ (������)
    public string _description = ""; //ī�� ����
    #endregion // public variable





    #region public funcs

    public Card(CardRank cardRank, int adRate, int apRate, int critChance, int critDamage, int moveSpeed, int def, int hp) {
        int cnt = 0;
        _cardRank = cardRank;
        _adRate = adRate;
        if (_adRate != 0) {
            AppendDescription($"���ݷ� + {_adRate}%");
            _cardProperty = CardProperty.Ad;
            cnt++;
        }

        _apRate = apRate;
        if (_apRate != 0) {
            AppendDescription($"���� + {_apRate}%");
            _cardProperty = CardProperty.Ap;
            cnt++;
        }

        _critChance = critChance;
        if (_critChance != 0) {
            AppendDescription($"ġ��Ÿ Ȯ�� + {_critChance}%");
            _cardProperty = CardProperty.CritChance;
            cnt++;
        }

        _critDamage = critDamage;
        if (_critDamage != 0) {
            AppendDescription($"ġ��Ÿ ���� + {_critDamage}%");
            _cardProperty = CardProperty.CritDamage;
            cnt++;
        }

        _moveSpeed = moveSpeed;
        if (_moveSpeed != 0) {
            AppendDescription($"�̵� �ӵ� + {_moveSpeed}%");
            _cardProperty = CardProperty.Speed;
        }

        _def = def;
        if (_def != 0) {
            AppendDescription($"���� + {_def}");
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
