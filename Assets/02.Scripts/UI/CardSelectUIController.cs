using System.Collections.Generic;
using UnityEngine;

public class CardSelectUIController : UIBase {
    #region serialized field

    [SerializeField] private List<CardUI> _cards;
    [SerializeField] private AudioClip _cardUISound;
    [SerializeField] GameObject _pauseUI;

    #endregion // serialized field



    #region private variables

    private System.Action<Card> _cardCallback;

    #endregion // private variables




    #region mono funcs
    private void Update() {
        SelectCardWithKey();
    }

    #endregion



    #region public funcs

    public void Initialize(System.Action<Card> cardClickCallback) {
        _cardCallback = cardClickCallback;
    }

    public void SetShow(List<Card> cards) {
        CardUI.BeforePointedCardIndex = -1;
        for (int i = 0, count = cards.Count; i < count; ++i) {
            if (null != _cards[i]) {
                _cards[i].Initialize(_cardCallback);
                _cards[i].SetShow(cards[i]);
            }
        }
        base.SetShow();
        SoundManager.Instance.PlaySFX(_cardUISound.name, true);
    }

    #endregion // public funcs





    #region private funcs

    private void SelectCardWithKey() {
        if (!_pauseUI.activeSelf) {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                CardInputCheck(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                CardInputCheck(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                CardInputCheck(2);
        }
    }

    private void CardInputCheck(int cardIndex) {
        int beforeIndex = CardUI.BeforePointedCardIndex;
        if (beforeIndex == cardIndex) {
            _cards[cardIndex].OnClick_Card();
        }
        else {
            _cards[cardIndex].OnPointerEnter();
            if (beforeIndex >= 0)
                _cards[beforeIndex].OnPointerExit();
            CardUI.BeforePointedCardIndex = cardIndex;
        }
    }

    #endregion // private funcs
}