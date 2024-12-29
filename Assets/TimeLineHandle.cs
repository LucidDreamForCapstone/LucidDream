using Cysharp.Threading.Tasks;
using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineHandle : MonoBehaviour
{
    [SerializeField] GungeonGameManager gungeonGameManager;
    [SerializeField] TimeLineManager timeLineManager;
    private void Start() {
        gungeonGameManager = GungeonGameManager.Instance;
    }
    void Update()
    {
        if (gungeonGameManager.Stage == 3) {
            CameraManager.Instance.SetFogOfWar(false);
            timeLineManager.PlayTimeline().Forget();
        }
    }
}
