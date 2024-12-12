using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour {

    #region private variable

    private static PlayerWeaponManager _instance;

    #endregion //private variable






    #region serialized field

    [SerializeField] WeaponBase _equippedWeapon;
    [SerializeField] Essence _equippedEssence;
    [SerializeField] private InGameUIController _inGameUIController;
    [SerializeField] string _warningMessage;
    [SerializeField] Color _cooldownWarningTextColor;

    #endregion //serialized field 





    #region private variable

    private Player _playerScript;

    #endregion //private variable





    #region property

    public static PlayerWeaponManager Instance { get { return _instance; } }

    #endregion //property





    #region mono funcs

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Update() {
        ActivateSkill();
    }
    #endregion


    #region public funcs

    public void WeaponChanged(WeaponBase changedWeapon) {
        if (!AreAllCooldownsComplete()) {
            SystemMessageManager.Instance.PushSystemMessage(_warningMessage, _cooldownWarningTextColor);
            return;
        }
        DropEquippedWeapon();
        //새 무기 장착.
        _inGameUIController.SetFever(PlayerDataManager.Instance.Status._feverGauge, PlayerDataManager.Instance.Status._maxFeverGauge);
        _equippedWeapon = changedWeapon;
        changedWeapon.transform.SetParent(transform);
        changedWeapon.InitializeWeaponPos();
        changedWeapon.tag = "Untagged";
        changedWeapon.SetEquipState(true);
        changedWeapon.SetWeaponSortingOrder(6);
        _equippedWeapon.SetWeaponSortingLayer("Player");
        _inGameUIController.UpdateSkillUI(_equippedWeapon);
        _inGameUIController.SetSkillCooldowns(
        changedWeapon.GetBasicCooldown(),
        changedWeapon.GetSkill1Cooldown(),
        changedWeapon.GetSkill2Cooldown()
        );
        if (changedWeapon.CheckUseCharging())
            _playerScript.ActivateChargeGauge();
        else
            _playerScript.DeActivateChargeGauge();

        PlayerDataManager.Instance.SetWeaponBonus(changedWeapon.GetWeaponBasicData());
    }

    public void DropEquippedWeapon() {

        if (_equippedWeapon == null) return;

        if (_equippedWeapon.GetComponent<NoWeapon>())
            NoWeaponEscape();

        _equippedWeapon.transform.SetParent(null);
        _equippedWeapon.transform.rotation = Quaternion.identity;
        _equippedWeapon.SetWeaponColor(Color.white);
        _equippedWeapon.tag = "PooledItem";
        _equippedWeapon.Drop();
        _equippedWeapon.SetEquipState(false);
        _equippedWeapon.SetWeaponSortingOrder(4);
        _equippedWeapon.SetWeaponSortingLayer("Item");
        // 스킬 UI 비활성화 (무기를 드롭하면 스킬을 숨기도록 할 수 있음)
        if (_inGameUIController != null) {
            _inGameUIController.ClearSkillUI();  // 스킬 UI를 비활성화하는 함수 추가
        }
    }

    public void EssenceChanged(Essence changedEssence) {
        DropEquippedEssence();
        //새 스톤 장착.
        _equippedEssence = changedEssence;
        changedEssence.transform.SetParent(transform);
        changedEssence.tag = "Untagged";
        changedEssence.gameObject.SetActive(false);
        changedEssence.SetEquipState(true);
        _inGameUIController.UpdateEssenceUI();
    }

    public void DropEquippedEssence() {
        _equippedEssence.transform.SetParent(null);
        _equippedEssence.transform.rotation = Quaternion.identity;
        _equippedEssence.tag = "PooledItem";
        _equippedEssence.gameObject.SetActive(true);
        _equippedEssence.Drop();
        _equippedEssence.SetEquipState(false);
    }


    public void BasicAttack() { //Animation Event에 연동하는 용도
        _equippedWeapon.BasicAttack();
    }

    public void Skill1() { //Animation Event에 연동하는 용도
        _equippedWeapon.Skill1();
    }

    public void Skill2() { //Animation Event에 연동하는 용도
        _equippedWeapon.Skill2();
    }

    public void FeverSkill() { //Animation Event에 연동하는 용도
        _equippedWeapon.FeverSkill();
    }

    public WeaponBase GetEquippedWeapon() { return _equippedWeapon; }

    public Essence GetEquippedEssence() { return _equippedEssence; }

    #endregion //public funcs




    #region private funcs   

    private void ActivateSkill() {
        if (!_playerScript.CheckStun() && !_playerScript.CheckPause() && !_playerScript.CheckAttacking() && !_playerScript.CheckDead() && _playerScript.CheckEnabled() && _playerScript.CheckAttackable()) {
            if (Input.GetKeyDown(KeyCode.Q))
                _equippedWeapon.ActivateBasicAttack().Forget();
            else if (Input.GetKeyDown(KeyCode.W))
                _equippedWeapon.ActivateSkill1().Forget();
            else if (Input.GetKeyDown(KeyCode.E))
                _equippedWeapon.ActivateSkill2().Forget();
            else if (Input.GetKeyDown(KeyCode.R))
                _equippedWeapon.ActivateFeverSkill();
        }
    }

    private bool AreAllCooldownsComplete() {
        for (int i = 0; i < 4; i++) {
            if (_inGameUIController.GetRemainingCooldown(i) > 0) {
                return false;
            }
        }
        return true;
    }

    private void NoWeaponEscape() {
        _playerScript.ArmTrigger("GetWeapon");

    }
    #endregion //private funcs
}
