using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour {
    #region type def

    private enum TextType {
        Level,
        HPSlider,
        Coin,
        Dream,
        PlayerStatus
    }

    #endregion // type def





    #region serialized field

    [SerializeField] private List<TMP_Text> _texts;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _feverSlider;

    [SerializeField] private CardSelectUIController _cardUIController;

    [SerializeField] private GameObject playerStatusUI;
    [SerializeField] private GameObject _showGuard;

    [SerializeField] private Image[] skillImages = new Image[4];  // 4���� ��ų ���� �̹���
    [SerializeField] private Image[] standSkillInven = new Image[4];   // 4���� �븻 ��ų �κ� �̹���.
    [SerializeField] private Image[] standSkillInform = new Image[4];   // 4���� �븻 ��ų ���� �̹���.
    [SerializeField] private TMP_Text[] skillTexts = new TMP_Text[4]; // 4���� ��ų ���� �ؽ�Ʈ

    [SerializeField] private Image feverFillImage; // fever ������ Fill Image

    [SerializeField] private Image[] skillCooldownImages = new Image[4];
    [SerializeField] private Image[] skillCooldownUnderlay = new Image[4];
    [SerializeField] private TMP_Text notificationText; // "���� ���� ���� �����ϴ�."�� ǥ���� UI �ؽ�Ʈ

    [SerializeField] private Sprite lockedSkillImage;

    [SerializeField] Image _essenceImage;
    [SerializeField] GameObject _essenceDescriptionUI;
    [SerializeField] TextMeshProUGUI _essenceDescriptionTM;
    [SerializeField] TextMeshProUGUI _essenceTitleTM;
    [SerializeField] List<Color> _essenceTextColorList;


    #endregion // serialized field




    #region private variables

    private int _levelUpCount = 0;
    //private bool[] skillLocks = new bool[4];  // �� ��ų ��� ���¸� ����
    private float[] skillCooldownTimes = new float[4];  // ��ų�� ��ٿ� �ð�
    private float[] skillMaxCooldowns = new float[4];   // ��ų�� �ִ� ��Ÿ�� ����.
    private Coroutine[] skillCoroutines = new Coroutine[4];
    private float[] remainingCooldownTimes = new float[4];
    private Coroutine fadeCoroutine;
    private InventoryManager inventoryManager;
    #endregion // private variables





    #region mono funcs

    public void Start() {
        PlayerDataManager.Instance?.InitializeIngameUI(this, () => {
            //SetHP(PlayerDataManager.Instance.Status._hp, PlayerDataManager.Instance.Status._maxHp);
            SetCoin(0);
            SetLevel(1);
            SetFever(0, 100); // �ǹ������� �ϴ� 0/100���� �����
        });
        InitializeCardUI();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void Update() {

        // Tab Ű�� ������ ���� �÷��̾� ���� UI�� ���̰� ��
        if (Input.GetKey(KeyCode.Tab)) {
            // Tab Ű�� ������ ���� �� �÷��̾� ������ �����ϰ� UI�� Ȱ��ȭ
            playerStatusUI.SetActive(true);
            SetPlayerStatus(
                PlayerDataManager.Instance.GetFinalAd(),               // ���� AD
                PlayerDataManager.Instance.GetFinalAp(),               // ���� AP
                PlayerDataManager.Instance.Status._def,                // ���� (���� ���)
                PlayerDataManager.Instance.GetFinalMoveSpeed(),        // ���� �̵� �ӵ�
                PlayerDataManager.Instance.GetFinalCritChance(),       // ���� ġ��Ÿ Ȯ��
                PlayerDataManager.Instance.GetFinalCritDamage()        // ���� ġ��Ÿ ������
            );
        }
        else if (Input.GetKeyUp(KeyCode.Tab)) {
            // Tab Ű�� ���� UI�� ����
            playerStatusUI.SetActive(false);
        }
    }

    #endregion // mono funcs




    #region public funcs

    public void SetHP(int currHP, int maxHP) {
        _texts[(int)TextType.HPSlider].text = string.Format("{0}/{1}", currHP, maxHP);
        _hpSlider.value = (float)currHP / (float)maxHP;
    }

    public void SetCoin(int coin) {
        _texts[(int)TextType.Coin].text = string.Format("{0}", coin);
    }

    public void SetLevel(int level) {
        _texts[(int)TextType.Level].text = string.Format("{0}", level);
    }

    public void SetFever(int currFever, int maxFever) {
        _feverSlider.value = (float)currFever / (float)maxFever;
        feverFillImage.fillAmount = _feverSlider.value;  // Fill Amount ������Ʈ
    }

    public void SetDream(int dream) {
        _texts[(int)TextType.Dream].text = string.Format("{0}", dream);
    }

    public void SetPlayerStatus(int finalAd, int finalAp, int finalDef, float finalSpeed, int finalCriticalChance, int finalCriticalDamage) {
        string statusText = string.Format("AD: {0}\nAP: {1}\nDEF: {2}\nSd: {3}\nCC: {4}%\nCD: {5}%",
            finalAd, finalAp, finalDef, finalSpeed, finalCriticalChance, finalCriticalDamage);

        _texts[(int)TextType.PlayerStatus].text = statusText;
    }

    public void ShowCardSelect(List<Card> cards, int levelUpCount) { // ī�� ���� UI ȣ�� �Լ�
        _levelUpCount = levelUpCount;
        _cardUIController.SetShow(cards);
        --_levelUpCount;
    }
    public void ResetFeverFill() {
        feverFillImage.fillAmount = 0;  // Fill Amount�� 0���� ����
    }
    public void ClearSkillUI() {
        for (int i = 0; i < skillImages.Length; i++) {
            skillImages[i].sprite = null;
            standSkillInven[i].sprite = null;
            skillTexts[i].text = "";
        }
    }
    public void UpdateSkillUI(WeaponBase newWeapon) {
        Sprite[] newSkillIcons = newWeapon.GetSkillIcons();
        Sprite[] newSkillInven = newWeapon.GetSkillInvens();
        Sprite[] newSkillInform = newWeapon.GetSkillInformPanel();
        Sprite[] newUnderlay = newWeapon.GetSkillUnderlay();

        string[] newSkillDescriptions = newWeapon.GetSkillDescriptions();
        bool[] skillLocks = newWeapon.GetSkillLocks();

        for (int i = 0; i < skillImages.Length; i++) {
            SkillTooltip skillTooltip = skillImages[i].GetComponent<SkillTooltip>();
            standSkillInven[i].sprite = newSkillInven[i];

            if (skillLocks[i]) {
                skillTooltip.SetSkillLocked(skillLocks[i]);
                skillImages[i].sprite = lockedSkillImage;
            }
            else {
                skillCooldownUnderlay[i].sprite = newUnderlay[i];
                // �� ��ų �����ܰ� ������ ������Ʈ
                skillTooltip.SetSkillLocked(skillLocks[i]);
                skillImages[i].sprite = newSkillIcons[i];
                standSkillInform[i].sprite = newSkillInform[i];
                skillTexts[i].text = newSkillDescriptions[i]; // �ؽ�Ʈ ������Ʈ
                skillTooltip = skillImages[i].GetComponent<SkillTooltip>();
                skillTooltip.SetSkillDescription(newSkillDescriptions[i]);
            }
        }
    }

    public void UseSkill(int skillIndex) {
        if (skillIndex >= 0 && skillIndex < skillCooldownTimes.Length) {
            StartCoroutine(SkillCooldownRoutine(skillIndex, skillCooldownTimes[skillIndex]));
        }
    }
    public void SetSkillCooldowns(float basicCooldown, float skill1Cooldown, float skill2Cooldown) {
        skillCooldownTimes[0] = basicCooldown;
        skillCooldownTimes[1] = skill1Cooldown;
        skillCooldownTimes[2] = skill2Cooldown;
    }

    public float GetRemainingCooldownForSkill(int skillIndex) {
        if (skillIndex >= 0 && skillIndex < 4) {
            return remainingCooldownTimes[skillIndex];
        }
        return 0f;
    }
    #endregion

    public float GetRemainingCooldown(int skillIndex) {
        // skillIndex�� ��ų�� �ε���. ��: 0 = �⺻ ����, 1 = ��ų 1, 2 = ��ų 2, 3 = �ǹ� ��ų ��.

        // ��ȿ�� �ε������� üũ
        if (skillIndex >= 0 && skillIndex < remainingCooldownTimes.Length) {
            // �ش� ��ų�� ���� ��Ÿ���� ��ȯ
            return remainingCooldownTimes[skillIndex];
        }

        // ��ȿ���� ���� �ε����� ��� 0�� ��ȯ
        return 0f;
    }

    public void ShowGuard() {
        if (InventoryManager.Instance.HasItem(ItemType.Guard)) {
            _showGuard.SetActive(true);
        }
    }

    public void offGuard() {
        _showGuard.SetActive(false);
    }
    #region private funcs



    private void InitializeCardUI() {
        _cardUIController.Initialize(OnClick_Card);
    }


    private void OnClick_Card(Card card) {
        CardManage.Instance.ApplyCard(card);

        if (1 <= _levelUpCount) {
            ShowCardSelect(CardManage.Instance.DrawCards(), _levelUpCount);
        }
        else {
            TimeScaleManager.Instance.TimeRestore();
            _cardUIController.SetHide();
        }
    }

    public void UpdateEssenceUI() {
        Essence essence = PlayerWeaponManager.Instance.GetEquippedEssence();
        _essenceImage.sprite = essence.GetMainEssenceSprite();
        _essenceTitleTM.text = essence.GetEssenceTitle();
        _essenceTitleTM.color = _essenceTextColorList[(int)essence.GetEssenceData().rank];
        _essenceDescriptionTM.text = essence.GetEssenceDescription();
    }


    // ��ų ��Ÿ�� UI ������Ʈ ��ƾ
    private IEnumerator SkillCooldownRoutine(int skillIndex, float remainingCooldown) {
        float elapsedTime = 0f;  // ��� �ð� �ʱ�ȭ

        // �ʱ� ��ų �������� fillAmount�� 0���� �����Ͽ� ����ִ� ���·� ����
        skillCooldownImages[skillIndex].fillAmount = 0f;

        while (elapsedTime < remainingCooldown) {
            elapsedTime += Time.deltaTime;  // ��� �ð� ����
            float fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / remainingCooldown);  // ���������� 1�� ����
            skillCooldownImages[skillIndex].fillAmount = fillAmount;  // ��ų �������� fillAmount�� ������Ʈ

            // ���� ��Ÿ�� ���
            remainingCooldownTimes[skillIndex] = remainingCooldown - elapsedTime;

            yield return null;  // �� ������ ���
        }

        // ��Ÿ���� ������ ���� ��Ÿ���� 0���� ����
        remainingCooldownTimes[skillIndex] = 0f;
        skillCooldownImages[skillIndex].fillAmount = 1f;
    }

    #endregion // private funcs
}
