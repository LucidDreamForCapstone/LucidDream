using UnityEngine;

public class Essence : ItemBase {
    [SerializeField] WeaponEssence _attribute;
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

    private void Start() {
        _textObj.SetActive(false);
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
        return new EssenceData(_attribute, _rank, _chance, _lastTime, _ratio, _tickDamage, _tickTime, _tickCount);
    }

    public bool CheckEquipped() {
        return _isEquipped;
    }

    public void SetEquipState(bool state) {
        _isEquipped = state;
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
