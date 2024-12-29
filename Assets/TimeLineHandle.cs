using Cysharp.Threading.Tasks;
using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineHandle : MonoBehaviour
{
    [SerializeField] GungeonGameManager gungeonGameManager;
    [SerializeField] TimeLineManager timeLineManager;
    private bool playOnce = false;
    private void Start() {
        gungeonGameManager = GungeonGameManager.Instance;
    }
    void Update()
    {
        if (gungeonGameManager.Stage == 3 && !playOnce) {
            playOnce = true;    
            CameraManager.Instance.SetFogOfWar(false);
            timeLineManager.ResumeTimeline();
        }
    }
}
    