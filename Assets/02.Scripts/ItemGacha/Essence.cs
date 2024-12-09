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
                name = "정지의 정수";
                break;
            case WeaponEssence.Ice:
                name = "혹한의 정수";
                break;
            case WeaponEssence.Poison:
                name = "독의 정수";
                break;
            case WeaponEssence.AttFear:
                name = "공포의 정수";
                break;
            case WeaponEssence.DefFear:
                name = "관통의 정수";
                break;
            case WeaponEssence.BloodSuck:
                name = "흡혈의 정수";
                break;
        }
        return $"[{_rank.ToString()}]\n{name}";
    }
    public string GetEssenceDescription() {
        string desc = "";
        switch (_type) {
            case WeaponEssence.None:
                desc = "장착 중인 정수가 없습니다";
                break;
            case WeaponEssence.Stun:
                desc = $"공격 시\n<color=yellow>{_chance}%</color>의 확률로\n적을 <color=yellow>{_lastTime}초</color> 동안\n기절시킵니다.";
                break;
            case WeaponEssence.Ice:
                desc = $"공격 시\n<color=yellow>{_chance}%</color>의 확률로\n빙결시켜\n적의 이동속도를\n<color=yellow>{_lastTime}초</color> 동안\n<color=yellow>{_ratio}%</color>감소시킵니다.\n[중첩 가능]";
                break;
            case WeaponEssence.Poison:
                desc = $"공격 시\n<color=yellow>{_chance}%</color>의 확률로\n독에 중독시켜\n<color=yellow>{_tickTime}초</color> 마다\n<color=yellow>{_tickDamage}</color>의 중독 피해를\n<color=yellow>{_tickCount}회</color> 입힙니다.\n[중첩 가능]";
                break;
            case WeaponEssence.AttFear:
                desc = $"공격 시\n<color=yellow>{_chance}%</color>의 확률로\n겁에 질리게 하여\n적의 공격력을\n<color=yellow>{_lastTime}초</color> 동안\n<color=yellow>{_ratio}</color>%감소시킵니다.\n[중첩 가능]";
                break;
            case WeaponEssence.DefFear:
                desc = $"공격 시\n<color=yellow>{_chance}%</color>의 확률로\n공격이 관통하여\n적의 방어력을\n<color=yellow>{_lastTime}초</color> 동안\n<color=yellow>{_ratio}</color>%감소시킵니다.\n[중첩 가능]";
                break;
            case WeaponEssence.BloodSuck:
                desc = $"공격 시\n<color=yellow>{_chance}%</color>의 확률로\n흡혈하여\n최대 체력의\n<color=yellow>{_ratio}%</color>만큼\n회복합니다.";
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
