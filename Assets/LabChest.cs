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
        return _isOpened; // ���ڰ� ���� ��� ��ȣ�ۿ� ����
    }

    public override string GetInteractText() {
        return "���� (G)";
    }

    #endregion

    #region private funcs

    protected virtual void ChestOpen() {
        // InteractManager���� ��ȣ�ۿ� ���� ���� üũ
        if (!_isOpened && Player2_InteractManager.Instance.CheckInteractable(this)) {
            if (Input.GetKey(KeyCode.G)) // 'G' Ű�� ������ ��
            {
                Player2_InteractManager.Instance.InteractCoolTime().Forget(); // ��Ÿ�� ó��
                if (_animator != null) {
                    _animator.SetTrigger("Open"); // ���� �ִϸ��̼� ����
                }
                DropItems(); // ������ ���
                _isOpened = true; // ���� ���� ���·� ����
                // �ִϸ��̼��� �ߴ��ϰ� openImage ǥ��
                if (_animator != null) {
                    _animator.enabled = false; // �ִϸ����� �ߴ�
                }
                PlaySound(_openSound); // ���� �Ҹ� ���
                if (_openImage != null) {
                    _openImage.gameObject.SetActive(true); // _openImage Ȱ��ȭ
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
