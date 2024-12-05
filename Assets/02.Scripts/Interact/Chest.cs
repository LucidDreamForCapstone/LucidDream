using UnityEngine;

public class Chest : DropableBase {

    #region serialize field

    [SerializeField] ChestRank _chestRank;
    [SerializeField] protected AudioClip _openSound;
    #endregion //serialize field



    #region private variable

    protected bool _isOpened;
    protected Animator _animator;

    #endregion //private variable



    #region mono funcs

    protected void Start() {
        _isOpened = false;
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player"))
            ChestOpen();
    }

    #endregion //mono funcs




    #region public funcs

    public override bool IsInteractBlock() {
        if (_isOpened) return true;
        else return false;
    }

    public override string GetInteractText() {
        return "¿­±â (G)";
    }

    #endregion


    #region private funcs

    protected virtual void ChestOpen() {
        if (!_isOpened && InteractManager.Instance.CheckInteractable(this)) {
            if (Input.GetKey(KeyCode.G)) {
                InteractManager.Instance.InteractCoolTime().Forget();
                _animator.SetTrigger("Open");
                DropItems();
                _isOpened = true;
                PlaySound(_openSound);
            }
        }
    }
    protected void PlaySound(AudioClip clip) {
        if (clip != null) {
            SoundManager.Instance.PlaySFX(clip.name);
        }
    }
    #endregion //private funcs

}
