using UnityEngine;

public class Essence : ItemBase {
    [SerializeField] WeaponEssence _type;
    [SerializeField] CardRank _rank;
    [SerializeField] float _chance;
    [SerializeField] float _lastTime;
    [SerializeField] float _ratio;

    [Header("\nPoison")]
    [SerializeField] int _tickDamage;
    [SerializeField] float _tickTime;
    [SerializeField] int _tickCount;

    [SerializeField] bool _isEquipped;
    [SerializeField] GameObject _textObj;
    Sprite _mainSprite;

    private void Start() {
        _textObj.SetActive(false);
        _mainSprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Equip();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            _textObj.SetActive(false);
        }
    }

    public EssenceData GetEssenceData() {
        return new EssenceData(_type, _rank, _chance, _lastTime, _ratio, _tickDamage, _tickTime, _tickCount);
    }

    public bool CheckEquipped() {
        return _isEquipped;
    }

    public void SetEquipState(bool state) {
        _isEquipped = state;
    }

    public string GetEssenceTitle() {
        string name = "";
        switch (_type) {
            case WeaponEssence.Stun:
                name = "������ ����";
                break;
            case WeaponEssence.Ice:
                name = "Ȥ���� ����";
                break;
            case WeaponEssence.Poison:
                name = "���� ����";
                break;
            case WeaponEssence.AttFear:
                name = "������ ����";
                break;
            case WeaponEssence.DefFear:
                name = "������ ����";
                break;
            case WeaponEssence.BloodSuck:
                name = "������ ����";
                break;
        }
        return $"[{_rank.ToString()}]\n{name}";
    }
    public string GetEssenceDescription() {
        string desc = "";
        switch (_type) {
            case WeaponEssence.None:
                desc = "���� ���� ������ �����ϴ�";
                break;
            case WeaponEssence.Stun:
                desc = $"���� ��\n<color=yellow>{_chance}%</color>�� Ȯ����\n���� <color=yellow>{_lastTime}��</color> ����\n������ŵ�ϴ�.";
                break;
            case WeaponEssence.Ice:
                desc = $"���� ��\n<color=yellow>{_chance}%</color>�� Ȯ����\n�������\n���� �̵��ӵ���\n<color=yellow>{_lastTime}��</color> ����\n<color=yellow>{_ratio}%</color>���ҽ�ŵ�ϴ�.\n[��ø ����]";
                break;
            case WeaponEssence.Poison:
                desc = $"���� ��\n<color=yellow>{_chance}%</color>�� Ȯ����\n���� �ߵ�����\n<color=yellow>{_tickTime}��</color> ����\n<color=yellow>{_tickDamage}</color>�� �ߵ� ���ظ�\n<color=yellow>{_tickCount}ȸ</color> �����ϴ�.\n[��ø ����]";
                break;
            case WeaponEssence.AttFear:
                desc = $"���� ��\n<color=yellow>{_chance}%</color>�� Ȯ����\n�̿� ������ �Ͽ�\n���� ���ݷ���\n<color=yellow>{_lastTime}��</color> ����\n<color=yellow>{_ratio}</color>%���ҽ�ŵ�ϴ�.\n[��ø ����]";
                break;
            case WeaponEssence.DefFear:
                desc = $"���� ��\n<color=yellow>{_chance}%</color>�� Ȯ����\n������ �����Ͽ�\n���� ������\n<color=yellow>{_lastTime}��</color> ����\n<color=yellow>{_ratio}</color>%���ҽ�ŵ�ϴ�.\n[��ø ����]";
                break;
            case WeaponEssence.BloodSuck:
                desc = $"���� ��\n<color=yellow>{_chance}%</color>�� Ȯ����\n�����Ͽ�\n�ִ� ü����\n<color=yellow>{_ratio}%</color>��ŭ\nȸ���մϴ�.";
                break;
        }
        return desc;
    }
    public Sprite GetMainEssenceSprite() {
        return _mainSprite;
    }

    public override bool IsInteractBlock() {
        if (_isEquipped) return true;
        else return false;
    }

    private void Equip() {
        if (!_isEquipped && _isGround && InteractManager.Instance.CheckInteractable(this)) {
            _textObj.SetActive(true);
            if (Input.GetKey(KeyCode.G)) {
                InteractManager.Instance.InteractCoolTime().Forget();
                PlayerWeaponManager.Instance.EssenceChanged(this);
            }
        }
    }
}
