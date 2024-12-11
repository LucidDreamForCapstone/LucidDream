using System.Collections.Generic;
using UnityEngine;

public class TurretPuzzleManager : PuzzleBase {
    [SerializeField] DoorControl _doorControl;
    [SerializeField] List<LabTurret> _turretList;
    private int _leftTurretCount;


    private void Start() {
        _leftTurretCount = _turretList.Count;
    }

    private void Update() {
        cleared = IsGameClear();
        _doorControl.DoorOpenOnGameClear(cleared);
    }

    public void DecreaseTurretCount() {
        _leftTurretCount--;
    }

    public bool IsGameClear() {
        if (_leftTurretCount == 0) {
            return true;
        }
        return false;
    }
}
