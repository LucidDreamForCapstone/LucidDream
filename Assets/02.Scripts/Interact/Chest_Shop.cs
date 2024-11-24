using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


public class Chest_Shop : Chest {
    #region serialize field
    [SerializeField] private string _message;
    [SerializeField] private int _requiredCoins = 100;
    [SerializeField] protected AudioClip effectSound;
    #endregion //serialize field





    #region private field
    private InGameUIController inGameUIController;
    private bool _isPrinting;


    #endregion //serialize field




    #region mono funcs
    new private void Start() {
        base.Start();
        inGameUIController = FindObjectOfType<InGameUIController>();
        _isPrinting = false;
    }
    #endregion //mono




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
                    NoCoinMessage().Forget();
                }
            }
        }
    }

    #endregion //protected func



    #region private funcs

    private async UniTaskVoid NoCoinMessage() {
        if (!_isPrinting) {
            _isPrinting = true;
            SystemMessageManager.Instance.PushSystemMessage(_message, Color.red);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _isPrinting = false;
        }
    }

    #endregion
}
