using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase {

    #region serialized field

    public int _cardIndex;
    public List<GameObject> _cardBGImages;   // ��� �̹����� ���� ����Ʈ
    public List<Sprite> _cardImages;    // ī�� �̹����� ���� ����Ʈ
    public Image _cardImage;           // ���� ī�� �̹���
    public TMP_Text _cardDescription;        // ī�� ����

    #endregion // serialized field




    #region property

    public static int BeforePointedCardIndex { get; set; }

    #endregion



    #region private variables
    [SerializeField] AudioClip _touchSound;
    [SerializeField] AudioClip _clickedSound;
    private Card _card;
    private System.Action<Card> _callback;

    #endregion // private variables

    #region public funcs

    public void Initialize(System.Action<Card> callback) {
        this._callback = callback;
    }

    public void SetShow(Card card) {
        this._card = card;
        SetBGImage();
        SetCardImage();
        SetDesc();

        // Card init
        InitCard();

        // Show the card with Fade In effect
        FadeInCard();
        base.SetShow();
    }

    public void OnClick_Card() {
        SoundManager.Instance.PlaySFX(_clickedSound.name, true);
        Debug.Log("Card clicked!");
        if (_callback != null) {
            _callback(_card);
        }

        // Fade Out the card when clicked
        //FadeOutCard();
    }

    #endregion // public funcs

    #region private funcs

    private void SetBGImage() {
        for (int i = 0, count = _cardBGImages.Count; i < count; ++i) {
            _cardBGImages[i].SetActive((int)_card._cardRank == i);
        }
    }

    private void SetCardImage() {
        CardProperty cardProperty = _card._cardProperty;
        _cardImage.sprite = _cardImages[(int)cardProperty];
    }

    private void SetDesc() {
        _cardDescription.text = _card._description;
        if (_card._cardRank == CardRank.Mystic) {
            _cardDescription.color = Color.white;
        }
        else {
            _cardDescription.color = Color.black;
        }
    }

    // ī�忡 ���� Fade In ȿ��
    private void FadeInCard() {
        // ��� �̹��� Fade In
        Image bgImage = _cardBGImages[(int)_card._cardRank].GetComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0);  // ���� �� ������ ����
        bgImage.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5�ʿ� ���� Fade In

        // ī�� �̹��� Fade In
        _cardImage.color = new Color(1, 1, 1, 0);  // ���� �� ������ ����
        _cardImage.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5�ʿ� ���� Fade In

        // ī�� ���� �ؽ�Ʈ Fade In
        _cardDescription.alpha = 0; // ���� �� ������ ����
        _cardDescription.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5�ʿ� ���� Fade In
    }

    // ī�忡 ���� Fade Out ȿ��
    private void FadeOutCard() {
        Image bgImage = _cardBGImages[(int)_card._cardRank].GetComponent<Image>();
        bgImage.DOFade(0f, 0.7f).SetUpdate(true);  // 0.7�ʿ� ���� Fade Out

        _cardImage.DOFade(0f, 0.7f).SetUpdate(true);  // 0.7�ʿ� ���� Fade Out

        // ī�� ���� �ؽ�Ʈ Fade Out
        _cardDescription.DOFade(0f, 0.7f).SetUpdate(true);  // 1.5�ʿ� ���� Fade Out
    }

    private void AddEventTriggers() {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        // PointerEnter 
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { OnPointerEnter(); });
        trigger.triggers.Add(entryEnter);

        // PointerExit 
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnPointerExit(); });
        trigger.triggers.Add(entryExit);
    }

    // ���콺�� ī�� ���� �ö���� ��
    public void OnPointerEnter() {
        BeforePointedCardIndex = _cardIndex;
        SoundManager.Instance.PlaySFX(_touchSound.name, true);
        ScaleUpCard();
    }

    // ���콺�� ī�忡�� ����� ��
    public void OnPointerExit() {
        ScaleDownCard();
    }

    private void ScaleUpCard() {
        transform.DOScale(1.2f, 0.3f).SetUpdate(true);
        _cardImage?.DOFade(1f, 0.3f).SetUpdate(true);
        _cardDescription?.DOFade(1f, 0.3f).SetUpdate(true);
    }

    private void ScaleDownCard() {
        transform.DOScale(1f, 0.3f).SetUpdate(true);
        _cardImage?.DOFade(1f, 0.3f).SetUpdate(true);
        _cardDescription?.DOFade(1f, 0.3f).SetUpdate(true);
    }

    private void InitCard() {
        transform.localScale = Vector3.one; //set normal
    }
    #endregion // private funcs
}

