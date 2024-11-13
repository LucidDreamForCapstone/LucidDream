using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineTrigger : MonoBehaviour
{
    private void Start() {
        if (playableDirector != null) {
            playableDirector.Pause(); // 시작 시 Timeline 일시정지
        }
    }
    [SerializeField] private PlayableDirector playableDirector; // 연결된 Timeline
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other) {
        if (hasTriggered) return; // 이미 실행되었으면 무시
        if (other.CompareTag("Player")) {
            hasTriggered = true;
            playableDirector.Resume();
        }
    }
}
