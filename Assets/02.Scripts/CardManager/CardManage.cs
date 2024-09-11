using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardManage : MonoBehaviour {
    #region serialized field

    private static CardManage _instance;

    #endregion // serialized field





    #region properties

    public static CardManage Instance { get { return _instance; } }

    #endregion // properties





    #region private variable

    private List<Card> _normalCards = new List<Card>();
    private List<Card> _rareCards = new List<Card>();
    private List<Card> _uniqueCards = new List<Card>();
    private List<Card> _legendaryCards = new List<Card>();
    private Card _mysticCard;
    private const float _NORMAL_RATE = 0.49f;
    private const float _RARE_RATE = 0.84f;
    private const float _UNIQUE_RATE = 0.98f;
    private const float _LEGENDARY_RATE = 0.9996f;

    #endregion // private variable





    #region mono funcs

    private void Awake() {
        GenerateAllCards();
    }

    void Start() {
        _instance = this;
    }

    #endregion // mono funcs





    #region public funcs

    public List<Card> DrawCards() {
        int i;
        float randomN;
        List<Card> cards = new List<Card>();
        for (i = 0; i < 3; i++) {
            //Random.InitState((int)DateTime.Now.Ticks);
            randomN = Random.Range(0, 1.00f);
            if (0 <= randomN && randomN < _NORMAL_RATE)
                cards.Add(DrawCard(_normalCards));
            else if (_NORMAL_RATE <= randomN && randomN < _RARE_RATE)
                cards.Add(DrawCard(_rareCards));
            else if (_RARE_RATE <= randomN && randomN < _UNIQUE_RATE)
                cards.Add(DrawCard(_uniqueCards));
            else if (_UNIQUE_RATE <= randomN && randomN < _LEGENDARY_RATE)
                cards.Add(DrawCard(_legendaryCards));
            else
                cards.Add(_mysticCard);
        }
        return cards;
    }

    public void ApplyCard(Card selectedCard) {
        int beforeAdPlusRate = PlayerDataManager.Instance.Status._adPlusRate;
        int beforeApPlusRate = PlayerDataManager.Instance.Status._apPlusRate;
        int beforeCritChance = PlayerDataManager.Instance.Status._critChance;
        int beforeCritDamage = PlayerDataManager.Instance.Status._critDamage;
        float beforeMoveSpeed = PlayerDataManager.Instance.Status._moveSpeed;
        int beforeDef = PlayerDataManager.Instance.Status._def;
        int beforeHp = PlayerDataManager.Instance.Status._hp;
        int beforeMaxHp = PlayerDataManager.Instance.Status._maxHp;

        if (selectedCard._adRate != 0)
            PlayerDataManager.Instance.SetAdPlusRate(beforeAdPlusRate + selectedCard._adRate);
        if (selectedCard._apRate != 0)
            PlayerDataManager.Instance.SetApPlusRate(beforeApPlusRate + selectedCard._apRate);
        if (selectedCard._critChance != 0)
            PlayerDataManager.Instance.SetCritChance(beforeCritChance + selectedCard._critChance);
        if (selectedCard._critDamage != 0)
            PlayerDataManager.Instance.SetCritDamage(beforeCritDamage + selectedCard._critDamage);
        if (selectedCard._moveSpeed != 0)
            PlayerDataManager.Instance.SetMoveSpeed(beforeMoveSpeed * (1 + selectedCard._moveSpeed * 0.01f));
        if (selectedCard._def != 0)
            PlayerDataManager.Instance.SetDef(beforeDef + selectedCard._def);
        if (selectedCard._hp != 0) {
            PlayerDataManager.Instance.SetMaxHP(beforeMaxHp + selectedCard._hp);
            PlayerDataManager.Instance.SetHP(beforeHp + selectedCard._hp);
        }
    }

    #endregion //public funcs





    #region private funcs   

    private void GenerateAllCards() {
        _normalCards.Add(new Card(CardRank.Normal, 10, 0, 0, 0, 0, 0, 0));
        _normalCards.Add(new Card(CardRank.Normal, 0, 30, 0, 0, 0, 0, 0));
        _normalCards.Add(new Card(CardRank.Normal, 0, 0, 5, 0, 0, 0, 0));
        _normalCards.Add(new Card(CardRank.Normal, 0, 0, 0, 20, 0, 0, 0));
        _normalCards.Add(new Card(CardRank.Normal, 0, 0, 0, 0, 5, 0, 0));
        _normalCards.Add(new Card(CardRank.Normal, 0, 0, 0, 0, 0, 5, 0));
        _normalCards.Add(new Card(CardRank.Normal, 0, 0, 0, 0, 0, 0, 100));

        _rareCards.Add(new Card(CardRank.Rare, 20, 0, 0, 0, 0, 0, 0));
        _rareCards.Add(new Card(CardRank.Rare, 0, 50, 0, 0, 0, 0, 0));
        _rareCards.Add(new Card(CardRank.Rare, 0, 0, 7, 0, 0, 0, 0));
        _rareCards.Add(new Card(CardRank.Rare, 0, 0, 0, 40, 0, 0, 0));
        _rareCards.Add(new Card(CardRank.Rare, 0, 0, 0, 0, 10, 0, 0));
        _rareCards.Add(new Card(CardRank.Rare, 0, 0, 0, 0, 0, 10, 0));
        _rareCards.Add(new Card(CardRank.Rare, 0, 0, 0, 0, 0, 0, 200));

        _uniqueCards.Add(new Card(CardRank.Unique, 30, 0, 0, 0, 0, 0, 0));
        _uniqueCards.Add(new Card(CardRank.Unique, 0, 70, 0, 0, 0, 0, 0));
        _uniqueCards.Add(new Card(CardRank.Unique, 0, 0, 10, 0, 0, 0, 0));
        _uniqueCards.Add(new Card(CardRank.Unique, 0, 0, 0, 60, 0, 0, 0));
        _uniqueCards.Add(new Card(CardRank.Unique, 0, 0, 0, 0, 15, 0, 0));
        _uniqueCards.Add(new Card(CardRank.Unique, 0, 0, 0, 0, 0, 15, 0));
        _uniqueCards.Add(new Card(CardRank.Unique, 0, 0, 0, 0, 0, 0, 300));

        _legendaryCards.Add(new Card(CardRank.Legendary, 50, 0, 0, 0, 0, 0, 0));
        _legendaryCards.Add(new Card(CardRank.Legendary, 0, 150, 0, 0, 0, 0, 0));
        _legendaryCards.Add(new Card(CardRank.Legendary, 0, 0, 15, 0, 0, 0, 0));
        _legendaryCards.Add(new Card(CardRank.Legendary, 0, 0, 0, 80, 0, 0, 0));
        _legendaryCards.Add(new Card(CardRank.Legendary, 0, 0, 0, 0, 30, 0, 0));
        _legendaryCards.Add(new Card(CardRank.Legendary, 0, 0, 0, 0, 0, 30, 0));
        _legendaryCards.Add(new Card(CardRank.Legendary, 0, 0, 0, 0, 0, 0, 500));

        _mysticCard = new Card(CardRank.Mystic, 100, 250, 30, 60, 0, 0, 0);
    }

    private Card DrawCard(List<Card> cardList) {
        int randomN = Random.Range(0, 7);
        return cardList[randomN];
    }

    #endregion //private funcs
}
