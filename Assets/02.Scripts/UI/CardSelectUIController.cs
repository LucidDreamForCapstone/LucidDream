using System.Collections.Generic;
using UnityEngine;

public class CardSelectUIController : UIBase {
    #region serialized field

    [SerializeField] private List<CardUI> _cards;

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
        for (int i = 0, count = cards.Count; i < count; ++i) {
            if (null != _cards[i]) {
                _cards[i].Initialize(_cardCallback);
                _cards[i].SetShow(cards[i]);
            }
        }

        base.SetShow();
    }

    #endregion // public funcs





    #region private funcs

    private void SelectCardWithKey() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _cards[0].OnClick_Card();
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            _cards[1].OnClick_Card();
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            _cards[2].OnClick_Card();

        Debug.Log("test");
    }

    #endregion // private funcs
}
