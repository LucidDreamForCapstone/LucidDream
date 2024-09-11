using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : UIBase {
    #region serialized field

    [SerializeField] private List<GameObject> _cardBGImages;
    [SerializeField] private Image _cardImage;
    [SerializeField] private TMP_Text _cardDescription;

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

        base.SetShow();
    }

    public void OnClick_Card() {
        Debug.Log("Card clicked!");
        if (null != _callback) {
            _callback(_card);
        }
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

    #endregion // private funcs
}
