using System.Collections.Generic;
using UnityEngine;

public class TurretPuzzleManager : PuzzleBase
{
    [SerializeField] List<LabTurret> _turretList;
    private int _leftTurretCount;


    private void Start()
    {
        _leftTurretCount = _turretList.Count;
    }

    private void Update()
    {
        cleared = IsGameClear();
        DoorController.DoorOpenOnGameClear(cleared);
        //_doorControl.IsInteractingToPortal = isInteractingToPortal;
        if (isInteractingToPortal)
            DoorController.IsInteractedOnce = true;
    }

    public void DecreaseTurretCount()
    {
        _leftTurretCount--;
    }

    public bool IsGameClear()
    {
        if (_leftTurretCount == 0)
        {
            return true;
        }
        return false;
    }
}
