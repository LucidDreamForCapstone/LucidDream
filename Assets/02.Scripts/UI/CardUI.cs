using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase {

    #region serialized field

    public List<GameObject> _cardBGImages;   // ��� �̹����� ���� ����Ʈ
    public Image _cardImage;               // ī�� �̹����� ���� �迭
    public TMP_Text _cardDescription;        // ī�� ������ ���� �ؽ�Ʈ

    #endregion // serialized field

    #region private variables

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
        SetDesc();

        // Card init
        InitCard();

        // Show the card with Fade In effect
        FadeInCard();
        base.SetShow();
    }

    public void OnClick_Card() {
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

    private void SetDesc() {
        _cardDescription.text = _card._description;
    }

    // ī�忡 ���� Fade In ȿ��
    private void FadeInCard() {
        // ��� �̹��� Fade In
        CanvasGroup bgCanvasGroup = _cardBGImages[(int)_card._cardRank].GetComponent<CanvasGroup>();
        if (bgCanvasGroup != null) {
            bgCanvasGroup.alpha = 0f;  // ���� �� ������ ����
            bgCanvasGroup.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5�ʿ� ���� Fade In
        }


        // ī�� �̹��� Fade In

        _cardImage.color = new Color(1, 1, 1, 0);  // ���� �� ������ ����
        _cardImage.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5�ʿ� ���� Fade In


        // ī�� ���� �ؽ�Ʈ Fade In
        _cardDescription.alpha = 0;
        // ���� �� ������ ����
        _cardDescription.DOFade(1f, 1.5f).SetUpdate(true);  // 1.5�ʿ� ���� Fade In
    }

    // ī�忡 ���� Fade Out ȿ��
    private void FadeOutCard() {
        // ��� �̹��� Fade Out
        CanvasGroup bgCanvasGroup = _cardBGImages[(int)_card._cardRank].GetComponent<CanvasGroup>();
        if (bgCanvasGroup != null) {
            bgCanvasGroup.DOFade(0f, 0.7f).SetUpdate(true);  // 1.5�ʿ� ���� Fade Out
        }



        _cardImage.DOFade(0f, 0.7f).SetUpdate(true);  // 1.5�ʿ� ���� Fade Out


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

