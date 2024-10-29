using UnityEngine;

public class LabChest : DropableBase {
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

    protected void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player"))
            ChestOpen();
    }

    #endregion //mono funcs

    #region public funcs

    public override bool IsInteractBlock() {
        return _isOpened; // 상자가 열린 경우 상호작용 차단
    }

    public override string GetInteractText() {
        return "열기 (G)";
    }

    #endregion

    #region private funcs

    protected virtual void ChestOpen() {
        // InteractManager에서 상호작용 가능 여부 체크
        if (!_isOpened && Player2_InteractManager.Instance.CheckInteractable(this)) {
            if (Input.GetKey(KeyCode.G)) // 'G' 키를 눌렀을 때
            {
                Player2_InteractManager.Instance.InteractCoolTime().Forget(); // 쿨타임 처리
                _animator.SetTrigger("Open"); // 애니메이션 실행
                DropItems(); // 아이템 드롭
                _isOpened = true; // 상자 열림 상태로 변경
                PlaySound(_openSound); // 열기 소리 재생
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
