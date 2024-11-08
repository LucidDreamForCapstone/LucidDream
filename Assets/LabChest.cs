using UnityEngine;
using UnityEngine.UI;

public class LabChest : DropableBase {
    #region serialize field

    [SerializeField] ChestRank _chestRank;
    [SerializeField] protected AudioClip _openSound;
    [SerializeField] private GameObject _openImage;
    #endregion //serialize field

    #region private variable

    protected bool _isOpened;
    protected Animator _animator;

    #endregion //private variable

    #region mono funcs

    protected void Start() {
        _isOpened = false;
        _animator = GetComponent<Animator>();
        if (_openImage != null) {
            _openImage.gameObject.SetActive(false);
        }
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
                if (_animator != null) {
                    _animator.SetTrigger("Open"); // 열기 애니메이션 실행
                }
                DropItems(); // 아이템 드롭
                _isOpened = true; // 상자 열림 상태로 변경
                // 애니메이션을 중단하고 openImage 표시
                if (_animator != null) {
                    _animator.enabled = false; // 애니메이터 중단
                }
                PlaySound(_openSound); // 열기 소리 재생
                if (_openImage != null) {
                    _openImage.gameObject.SetActive(true); // _openImage 활성화
                }

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
