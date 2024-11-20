using UnityEngine;

public class FloatingDamageManager : MonoBehaviour {

    #region private variable

    private static FloatingDamageManager _instance;

    #endregion // private variable





    #region serialized field

    [SerializeField] private GameObject _text;

    #endregion // serialized field





    #region properties

    public static FloatingDamageManager Instance { get { return _instance; } }

    #endregion // properties





    #region mono funcs

    void Start() {
        _instance = this;
    }

    #endregion // mono funcs





    #region public funcs

    public void ShowDamage(GameObject vr, int damage, bool isMine, bool isCrit, bool isHeal) {
        //GameObject txtObject = ObjectPool.Instance.GetObject(_text);
        GameObject txtObject = Instantiate(_text);
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(vr.transform.position);
        txtObject.transform.localPosition = uiPosition;
        txtObject.transform.SetParent(GameObject.Find("Player1Canvas").transform);
        txtObject.transform.SetAsFirstSibling();
        txtObject.SetActive(true);
        txtObject.GetComponent<FloatingText>().ShowDamage(damage, isMine, isCrit, isHeal);
    }

    #endregion // public funcs
}
