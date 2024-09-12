using Cysharp.Threading.Tasks;
using System;
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

    [SerializeField] private Image[] skillImages = new Image[4];  // 4개의 스킬 슬롯 이미지
    [SerializeField] private Image[] standSkillInven = new Image[4];   // 4개의 노말 스킬 인벤 이미지.
    [SerializeField] private Image[] standSkillInform = new Image[4];   // 4개의 노말 스킬 설명 이미지.
    [SerializeField] private TMP_Text[] skillTexts = new TMP_Text[4]; // 4개의 스킬 설명 텍스트

    [SerializeField] private Image feverFillImage; // fever 게이지 Fill Image

    [SerializeField] private Image[] skillCooldownImages = new Image[4];
    [SerializeField] private Image[] skillCooldownUnderlay = new Image[4];
    [SerializeField] private TMP_Text notificationText; // "범위 내에 적이 없습니다."를 표시할 UI 텍스트

    [SerializeField] private Sprite lockedSkillImage;


    #endregion // serialized field




    #region private variables

    private int _levelUpCount = 0;
    //private bool[] skillLocks = new bool[4];  // 각 스킬 잠김 상태를 저장
    private float[] skillCooldownTimes = new float[4];  // 스킬별 쿨다운 시간
    private float[] skillMaxCooldowns = new float[4];   // 스킬별 최대 쿨타임 저장.
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
            SetFever(0, 100); // 피버게이지 일단 0/100으로 만들기
        });
        InitializeCardUI();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void Update() {

        // Tab 키가 눌리는 동안 플레이어 스탯 UI를 보이게 함
        if (Input.GetKey(KeyCode.Tab)) {
            // Tab 키를 누르고 있을 때 플레이어 스탯을 갱신하고 UI를 활성화
            playerStatusUI.SetActive(true);
            SetPlayerStatus(
                PlayerDataManager.Instance.GetFinalAd(),               // 최종 AD
                PlayerDataManager.Instance.GetFinalAp(),               // 최종 AP
                PlayerDataManager.Instance.Status._def,                // 방어력 (직접 사용)
                PlayerDataManager.Instance.GetFinalMoveSpeed(),        // 최종 이동 속도
                PlayerDataManager.Instance.GetFinalCritChance(),       // 최종 치명타 확률
                PlayerDataManager.Instance.GetFinalCritDamage()        // 최종 치명타 데미지
            );
        }
        else if (Input.GetKeyUp(KeyCode.Tab)) {
            // Tab 키를 떼면 UI를 숨김
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
        feverFillImage.fillAmount = _feverSlider.value;  // Fill Amount 업데이트
    }

    public void SetDream(int dream) {
        _texts[(int)TextType.Dream].text = string.Format("{0}", dream);
    }

    public void SetPlayerStatus(int finalAd, int finalAp, int finalDef, float finalSpeed, int finalCriticalChance, int finalCriticalDamage) {
        string statusText = string.Format("AD: {0}\nAP: {1}\nDEF: {2}\nSd: {3}\nCC: {4}%\nCD: {5}%",
            finalAd, finalAp, finalDef, finalSpeed, finalCriticalChance, finalCriticalDamage);

        _texts[(int)TextType.PlayerStatus].text = statusText;
    }

    public void ShowCardSelect(List<Card> cards, int levelUpCount) { // 카드 선택 UI 호출 함수
        _levelUpCount = levelUpCount;
        _cardUIController.SetShow(cards);
        --_levelUpCount;
    }
    public void ResetFeverFill() {
        feverFillImage.fillAmount = 0;  // Fill Amount를 0으로 설정
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
                // 각 스킬 아이콘과 설명을 업데이트
                skillTooltip.SetSkillLocked(skillLocks[i]);
                skillImages[i].sprite = newSkillIcons[i];
                standSkillInform[i].sprite = newSkillInform[i];
                skillTexts[i].text = newSkillDescriptions[i]; // 텍스트 업데이트
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
        // skillIndex는 스킬의 인덱스. 예: 0 = 기본 공격, 1 = 스킬 1, 2 = 스킬 2, 3 = 피버 스킬 등.

        // 유효한 인덱스인지 체크
        if (skillIndex >= 0 && skillIndex < remainingCooldownTimes.Length) {
            // 해당 스킬의 남은 쿨타임을 반환
            return remainingCooldownTimes[skillIndex];
        }

        // 유효하지 않은 인덱스일 경우 0을 반환
        return 0f;
    }
    // 문구 표시 메서드
    public void ShowNotification(string message, float duration) {
        if (fadeCoroutine != null) {
            StopCoroutine(fadeCoroutine); // 이전 코루틴이 있으면 멈춤
        }

        fadeCoroutine = StartCoroutine(ShowNotificationRoutine(message, duration));
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


    private async void OnClick_Card(Card card) {
        Debug.Log("카드 적용한다");
        CardManage.Instance.ApplyCard(card);

        if (1 <= _levelUpCount) {
            ShowCardSelect(CardManage.Instance.DrawCards(), _levelUpCount);
        }
        else {
            //await WaitTime(1f);
            Time.timeScale = 1f;
            _cardUIController.SetHide();
        }
    }


    // 스킬 쿨타임 UI 업데이트 루틴
    private IEnumerator SkillCooldownRoutine(int skillIndex, float remainingCooldown) {
        float elapsedTime = 0f;  // 경과 시간 초기화

        // 초기 스킬 아이콘의 fillAmount를 0으로 설정하여 비어있는 상태로 시작
        skillCooldownImages[skillIndex].fillAmount = 0f;

        while (elapsedTime < remainingCooldown) {
            elapsedTime += Time.deltaTime;  // 경과 시간 증가
            float fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / remainingCooldown);  // 점진적으로 1로 증가
            skillCooldownImages[skillIndex].fillAmount = fillAmount;  // 스킬 아이콘의 fillAmount를 업데이트

            // 남은 쿨타임 계산
            remainingCooldownTimes[skillIndex] = remainingCooldown - elapsedTime;

            yield return null;  // 한 프레임 대기
        }

        // 쿨타임이 끝나면 남은 쿨타임을 0으로 설정
        remainingCooldownTimes[skillIndex] = 0f;
        skillCooldownImages[skillIndex].fillAmount = 1f;
    }


    private IEnumerator ShowNotificationRoutine(string message, float displayDuration) {
        // 텍스트 설정 및 초기 알파값 설정 (투명)
        notificationText.text = message;
        Color color = notificationText.color;
        color.a = 0f;
        notificationText.color = color;

        // Fade in (0 -> 1)
        float fadeInDuration = 0.2f; // fade in 시간
        for (float t = 0; t < fadeInDuration; t += Time.deltaTime) {
            color.a = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            notificationText.color = color;
            yield return null;
        }

        // 알파값을 1로 고정
        color.a = 1f;
        notificationText.color = color;

        // 지정된 시간 동안 텍스트 표시
        yield return new WaitForSeconds(displayDuration);

        // Fade out (1 -> 0)
        float fadeOutDuration = 0.2f; // fade out 시간
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime) {
            color.a = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            notificationText.color = color;
            yield return null;
        }

        // 알파값을 0으로 고정 (텍스트 숨김)
        color.a = 0f;
        notificationText.color = color;

        fadeCoroutine = null; // 코루틴 종료 후 null로 설정
    }

    private async UniTask WaitTime(float waitTime) {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime), ignoreTimeScale: true);
    }

    #endregion // private funcs
}
