using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase {

    #region serialized field

    public List<GameObject> _cardBGImages;   // 배경 이미지를 담은 리스트
    public List<Sprite> _cardImages;    // 카드 이미지를 담은 리스트
    public Image _cardImage;           // 실제 카드 이미지
    public TMP_Text _cardDescription;        // 카드 설명

    #endregion // serialized field

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
    }

    // 카드에 대한 Fade In 효과
    private void FadeInCard() {
        // 배경 이미지 Fade In
        Image bgImage = _cardBGImages[(int)_card._cardRank].GetComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0);  // 시작 시 완전히 투명
        bgImage.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5초에 걸쳐 Fade In

        // 카드 이미지 Fade In
        _cardImage.color = new Color(1, 1, 1, 0);  // 시작 시 완전히 투명
        _cardImage.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5초에 걸쳐 Fade In

        // 카드 설명 텍스트 Fade In
        _cardDescription.alpha = 0; // 시작 시 완전히 투명
        _cardDescription.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5초에 걸쳐 Fade In
    }

    // 카드에 대한 Fade Out 효과
    private void FadeOutCard() {
        Image bgImage = _cardBGImages[(int)_card._cardRank].GetComponent<Image>();
        bgImage.DOFade(0f, 0.7f).SetUpdate(true);  // 0.7초에 걸쳐 Fade Out

        _cardImage.DOFade(0f, 0.7f).SetUpdate(true);  // 0.7초에 걸쳐 Fade Out

        // 카드 설명 텍스트 Fade Out
        _cardDescription.DOFade(0f, 0.7f).SetUpdate(true);  // 1.5초에 걸쳐 Fade Out
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

    // 마우스가 카드 위로 올라왔을 때
    public void OnPointerEnter() {
        SoundManager.Instance.PlaySFX(_touchSound.name,true);
        ScaleUpCard();
    }

    // 마우스가 카드에서 벗어났을 때
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

