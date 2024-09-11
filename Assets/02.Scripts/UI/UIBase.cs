using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIBase : MonoBehaviour
{

    #region serialize fields

    [SerializeField] protected Text[] _texts;

    #endregion // serialize fields





    #region public funcs

    public virtual void UpdateTextsUI() { }

    public virtual void SetShow() { gameObject.SetActive(true); }

    public virtual void RefreshUI() { /* DO NOTHING */ }

    public virtual void SetHide() { gameObject.SetActive(false); }

    public virtual void OnClick_Close() { SetHide(); }

    #endregion // public funcs





    #region private funcs

    protected void SetText(int textTypeIndex, string content) {
        if (_texts.Length <= textTypeIndex) {
            Debug.LogErrorFormat("[ {0} ] : SetText : Out of texts array..!!\t[ {1}\t/ texts Length : {2}\t/ text index : {3} ]\n",
                this.GetType().Name, textTypeIndex, _texts.Length, textTypeIndex);
            return;
        }
        else if (null == _texts[textTypeIndex]) {
            Debug.LogErrorFormat("[ {0} ] : SetText : Invalid Text..!!\t[ {1} ]\n", textTypeIndex, content);
            return;
        }

        _texts[textTypeIndex].text = content;
        _texts[textTypeIndex].gameObject.SetActive(0 < content.Length);
    }

    #endregion // private funcs
}
