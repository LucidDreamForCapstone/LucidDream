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
    [SerializeField] private CanvasGroup cooldownTextCanvasGroup; // �ؽ�Ʈ�� �߰��� CanvasGroup

    #endregion //serialized field 





    #region private variable

    private Player _playerScript;
    private float warningDisplayDuration = 1.0f;
    private float fadeDuration = 0.5f; // Fade ȿ�� �ð�
    private bool isTextShowing = false; // �ؽ�Ʈ�� ǥ�� ������ Ȯ���ϴ� �÷���
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
        cooldownTextCanvasGroup.alpha = 0; // CanvasGroup ���İ��� 0���� ����
    }

    private void Update() {
        ActivateSkill();
    }
    private IEnumerator ShowCooldownWarning() {
        isTextShowing = true; // �ؽ�Ʈ�� ǥ�� ������ ���
        cooldownWarningText.gameObject.SetActive(true);
        // Fade In
        yield return StartCoroutine(FadeText(0, 1, fadeDuration));
        yield return new WaitForSeconds(warningDisplayDuration);
        // Fade Out
        yield return StartCoroutine(FadeText(1, 0, fadeDuration));
        cooldownWarningText.gameObject.SetActive(false);
        isTextShowing = false; // �ؽ�Ʈ�� ����� �� �ٽ� ǥ�� ����
    }

    // �ؽ�Ʈ�� ���İ��� ��ȭ��Ű�� Fade �ڷ�ƾ
    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration) {
        float elapsedTime = 0f;

        // �ؽ�Ʈ�� ���İ��� startAlpha���� endAlpha�� ��ȭ
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
        //�� ���� ����.
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
        // ��ų UI ��Ȱ��ȭ (���⸦ ����ϸ� ��ų�� ���⵵�� �� �� ����)
        if (_inGameUIController != null) {
            _inGameUIController.ClearSkillUI();  // ��ų UI�� ��Ȱ��ȭ�ϴ� �Լ� �߰�
        }
    }

    public void BasicAttack() { //Animation Event�� �����ϴ� �뵵
        _equippedWeapon.BasicAttack();
    }

    public void Skill1() { //Animation Event�� �����ϴ� �뵵
        _equippedWeapon.Skill1();
    }

    public void Skill2() { //Animation Event�� �����ϴ� �뵵
        _equippedWeapon.Skill2();
    }

    public void FeverSkill() { //Animation Event�� �����ϴ� �뵵
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
