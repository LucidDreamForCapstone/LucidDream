using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


public class Chest_Shop : Chest {
    #region serialize field
    [SerializeField] private int _requiredCoins = 100;
    [SerializeField] protected AudioClip effectSound;
    #endregion //serialize field





    #region private field
    private InGameUIController inGameUIController;


    #endregion //serialize field





    #region protected funcs

    protected override void ChestOpen() {
        if (!_isOpened && InteractManager.Instance.CheckInteractable(this)) {
            if (Input.GetKey(KeyCode.G)) {
                // 100 ������ ������ �� �ִ��� Ȯ��
                if (PlayerDataManager.Instance.BuyItem(_requiredCoins)) {
                    InteractManager.Instance.InteractCoolTime().Forget();
                    _animator.SetTrigger("Open");
                    PlaySound(_openSound);
                    PlaySound(effectSound);
                    DropItems();
                    _isOpened = true;
                }
                else {
                    // ������ ������ �� �˸� �޽��� ǥ��
                    inGameUIController.ShowNotification("������ �����մϴ�.", 2f);
                }
            }
        }
    }

    #endregion //protected funcs


    #region mono funcs
    new private void Start() {
        base.Start();
        inGameUIController = FindObjectOfType<InGameUIController>();
    }


    #endregion //mono
}
