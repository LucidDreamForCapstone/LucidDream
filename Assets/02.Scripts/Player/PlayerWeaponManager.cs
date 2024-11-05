using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour {

    #region private variable

    private static PlayerWeaponManager _instance;

    #endregion //private variable






    #region serialized field

    [SerializeField] WeaponBase _equippedWeapon;
    //[SerializeField] Vector2 _weaponPos;

    [SerializeField] private InGameUIController _inGameUIController;
    [SerializeField] private TMP_Text cooldownWarningText;
    [SerializeField] private CanvasGroup cooldownTextCanvasGroup; // 텍스트에 추가한 CanvasGroup

    #endregion //serialized field 





    #region private variable

    private Player _playerScript;
    private float warningDisplayDuration = 1.0f;
    private float fadeDuration = 0.5f; // Fade 효과 시간
    private bool isTextShowing = false; // 텍스트가 표시 중인지 확인하는 플래그
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
        cooldownWarningText.gameObject.SetActive(false);
        cooldownTextCanvasGroup.alpha = 0; // CanvasGroup 알파값을 0으로 설정
    }

    private void Update() {
        ActivateSkill();
    }
    private IEnumerator ShowCooldownWarning() {
        isTextShowing = true; // 텍스트가 표시 중임을 기록
        cooldownWarningText.gameObject.SetActive(true);
        // Fade In
        yield return StartCoroutine(FadeText(0, 1, fadeDuration));
        yield return new WaitForSeconds(warningDisplayDuration);
        // Fade Out
        yield return StartCoroutine(FadeText(1, 0, fadeDuration));
        cooldownWarningText.gameObject.SetActive(false);
        isTextShowing = false; // 텍스트가 사라진 후 다시 표시 가능
    }

    // 텍스트의 알파값을 변화시키는 Fade 코루틴
    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration) {
        float elapsedTime = 0f;

        // 텍스트의 알파값을 startAlpha에서 endAlpha로 변화
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            cooldownTextCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        cooldownTextCanvasGroup.alpha = endAlpha;
    }
    #endregion // mono funcs






    #region public funcs

    public void WeaponChanged(WeaponBase changedWeapon) {
        if (!AreAllCooldownsComplete()) {
            if (!isTextShowing) {
                StartCoroutine(ShowCooldownWarning());
            }
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

    #endregion //public funcs




    #region private funcs   

    private void ActivateSkill() {
        if (!_playerScript.CheckStun() && !_playerScript.CheckPause() && !_playerScript.CheckAttacking() && !_playerScript.CheckDead() && _playerScript.CheckEnabled()) {
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
