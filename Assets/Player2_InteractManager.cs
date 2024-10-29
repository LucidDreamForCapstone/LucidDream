using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player2_InteractManager : MonoBehaviour {
    #region serialize field

    [SerializeField] private float _searchRadius; // ��ȣ�ۿ� Ž�� �ݰ�

    #endregion // serialize field

    #region private variable

    private static Player2_InteractManager _instance; // �̱��� �ν��Ͻ�
    private bool _interactReady = true; // ��ȣ�ۿ� �غ� ����
    private float _interactCoolTime = 0.5f; // ��ȣ�ۿ� ��Ÿ��
    private Interactable _interactTarget; // ���� ��ȣ�ۿ� ���
    private Player_2 _playerScript2; // Player_2 ��ũ��Ʈ ����

    #endregion // private variable

    #region property

    public static Player2_InteractManager Instance {
        get {
            if (_instance == null) {
                Debug.LogError("Player2_InteractManager is null!");
            }
            return _instance;
        }
    }

    #endregion // property

    #region mono funcs

    private void Awake() {
        // �̱��� �ν��Ͻ��� ����
        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject); // �̹� �����ϸ� �� ��ü�� �ı�
        }
    }

    private void Update() {
        // Player_2 ������Ʈ�� �����ɴϴ�.
        _playerScript2 = GetComponent<Player_2>();

        // ��ȿ�� �÷��̾� ��ũ��Ʈ�� ������ ��� ��ȣ�ۿ� ����� ������Ʈ�մϴ�.
        if (_playerScript2 != null) {
            UpdateTargetInteractable(_playerScript2.transform.position);
            InteractUI();
        }
    }

    #endregion // mono funcs

    #region public funcs

    public async UniTaskVoid InteractCoolTime() {
        if (_interactReady) {
            _interactReady = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_interactCoolTime), ignoreTimeScale: true);
            _interactReady = true;
        }
    }

    public bool CheckInteractable(Interactable checkTarget) {
        if (_interactTarget == null) {
            Debug.LogError("No interact target set!");
            return false; // null�� ��� false ��ȯ
        }

        return _interactReady && _interactTarget.Equals(checkTarget);
    }

    #endregion // public funcs

    #region private funcs

    private void UpdateTargetInteractable(Vector2 playerPos) {
        var interactables = Physics2D.OverlapCircleAll(playerPos, _searchRadius);
        List<GameObject> possibleTargets = new List<GameObject>();

        foreach (var interactable in interactables) {
            var interactableTarget = interactable.GetComponent<Interactable>();
            if (interactableTarget != null && !interactableTarget.IsInteractBlock()) {
                possibleTargets.Add(interactable.gameObject);
            }
        }

        if (possibleTargets.Count > 0) {
            _interactTarget = possibleTargets[0].GetComponent<Interactable>();
        }
        else {
            _interactTarget = null;
        }
    }





    private void InteractUI() {
        if (_interactTarget != null) {
            // ��ȣ�ۿ� UI ���� ó�� (��: �ؽ�Ʈ ǥ�� ��)
            Debug.Log($"Interact with: {_interactTarget.GetInteractText()}");
        }
    }

    #endregion // private funcs
}
