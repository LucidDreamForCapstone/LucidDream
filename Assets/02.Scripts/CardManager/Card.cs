using System;
using System.Diagnostics;

[Serializable]
public class Card
{
    #region public variable
    public CardRank _cardRank;
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
        _cardRank = cardRank;
        _adRate = adRate;
        if (_adRate != 0)
            AppendDescription($"���ݷ� + {_adRate}%");
        _apRate = apRate;
        if (_apRate != 0)
            AppendDescription($"���� + {_apRate}%");
        _critChance = critChance;
        if (_critChance != 0)
            AppendDescription($"ġ��Ÿ Ȯ�� + {_critChance}%");
        _critDamage = critDamage;
        if (_critDamage != 0)
            AppendDescription($"ġ��Ÿ ���� + {_critDamage}%");
        _moveSpeed = moveSpeed;
        if (_moveSpeed != 0)
            AppendDescription($"�̵� �ӵ� + {_moveSpeed}%");
        _def = def;
        if (_def != 0)
            AppendDescription($"���� + {_def}");
        _hp = hp;
        if (_hp != 0)
            AppendDescription($"HP + {_hp}");
    }


    #endregion //public funcs





    #region private funcs

    private void AppendDescription(string addString) { // ���࿡ UI�� �־��µ� ���� �ٹٲ� �̽��� ���ϴٸ� ���� �Լ��� ������ �ʿ䰡 ����
        if (_description.Equals(""))
            _description = addString;
        else
            _description += ", " + addString;
    }

    #endregion //private funcs
}
