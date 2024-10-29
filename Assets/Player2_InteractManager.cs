using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player2_InteractManager : MonoBehaviour {
    #region serialize field

    [SerializeField] private float _searchRadius; // 상호작용 탐색 반경

    #endregion // serialize field

    #region private variable

    private static Player2_InteractManager _instance; // 싱글톤 인스턴스
    private bool _interactReady = true; // 상호작용 준비 상태
    private float _interactCoolTime = 0.5f; // 상호작용 쿨타임
    private Interactable _interactTarget; // 현재 상호작용 대상
    private Player_2 _playerScript2; // Player_2 스크립트 참조

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
        // 싱글톤 인스턴스를 설정
        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject); // 이미 존재하면 이 객체를 파괴
        }
    }

    private void Update() {
        // Player_2 컴포넌트를 가져옵니다.
        _playerScript2 = GetComponent<Player_2>();

        // 유효한 플레이어 스크립트가 존재할 경우 상호작용 대상을 업데이트합니다.
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
            return false; // null인 경우 false 반환
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
            // 상호작용 UI 관련 처리 (예: 텍스트 표시 등)
            Debug.Log($"Interact with: {_interactTarget.GetInteractText()}");
        }
    }

    #endregion // private funcs
}
