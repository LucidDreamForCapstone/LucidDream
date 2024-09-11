using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MainUIController : UIBase
{
    #region serializable field

    [SerializeField] private OptionUIController _optionUIController;

    #endregion //serializable field



    #region public funcs

    public void OnClick_GameStart() {
        GameSceneManager.Instance.LoadStageScene(1);
    }

    public void OnClick_Option() {
        _optionUIController.SetShow();
    }

    #endregion // public funcs
}
