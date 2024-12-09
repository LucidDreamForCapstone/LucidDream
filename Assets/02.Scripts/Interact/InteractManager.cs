using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour {


    #region serialize field

    [SerializeField] float _searchRadius;

    #endregion //serialize field



    #region private variable

    private static InteractManager _instance;
    private bool _interactReady = true;
    private float _interactCoolTime = 0.5f;
    private Player _playerScript;
    private Interactable _interactTarget;
    private WeaponText currentWeaponItem;
    #endregion //private variable





    #region property

    public static InteractManager Instance { get { return _instance; } }

    #endregion //property





    #region mono funcs

    private void Awake() {
        _instance = this;
    }

    private void Update() {
        _playerScript = GetComponent<Player>();
        UpdateTargetInteractable();
        InteractUI();
    }

    #endregion //mono funcs





    #region public funcs
    public async UniTaskVoid InteractCoolTime() {
        if (_interactReady) {
            _interactReady = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_interactCoolTime), ignoreTimeScale: true);
            _interactReady = true;
        }
    }

    public bool CheckInteractable(Interactable checkTarget) {
        if (_interactReady && _interactTarget.Equals(checkTarget))
            return true;
        else
            return false;
    }

    #endregion //public funcs





    #region private funcs

    private void UpdateTargetInteractable() {
        var playerPos = _playerScript.transform.position;
        var interactables = Physics2D.OverlapCircleAll(playerPos, _searchRadius);
        List<GameObject> possibleTargets = new List<GameObject>();
        int i, length = interactables.Length;
        for (i = 0; i < length; i++) {
            var interactableTarget = interactables[i].GetComponent<Interactable>();
            if (interactableTarget != null && !interactableTarget.IsInteractBlock())
                possibleTargets.Add(interactables[i].gameObject);
        }
        length = possibleTargets.Count;

        if (length != 0) {
            GameObject target = possibleTargets[0];
            double dist = CalculateManhattanDist(playerPos, target.transform.position);
            double shortestDist = dist;
            for (i = 1; i < length; i++) {
                dist = CalculateManhattanDist(playerPos, possibleTargets[i].transform.position);
                if (dist < shortestDist) {
                    shortestDist = dist;
                    target = possibleTargets[i];
                }
            }
            _interactTarget = target.GetComponent<Interactable>();


            var weaponText = target.GetComponent<WeaponText>();
            if (weaponText != null && weaponText != currentWeaponItem) {
                if (currentWeaponItem != null) {
                    currentWeaponItem.HideText();
                }
                weaponText.ShowTextDelay(0.5f);
                currentWeaponItem = weaponText;
            }
        }
        else {
            _interactTarget = null;
            if (currentWeaponItem != null) {
                currentWeaponItem.HideText();
                currentWeaponItem = null;
            }
        }
    }



    private void InteractUI() {
        if (_interactTarget != null) {
            //Debug.Log(_interactTarget.GetInteractText());

        }
    }


    private double CalculateManhattanDist(Vector2 a, Vector2 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion //private funcs
}
