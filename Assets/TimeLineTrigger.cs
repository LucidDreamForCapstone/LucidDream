using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineTrigger : MonoBehaviour
{
    private void Start() {
        if (playableDirector != null) {
            playableDirector.Pause(); // ���� �� Timeline �Ͻ�����
        }
    }
    [SerializeField] private PlayableDirector playableDirector; // ����� Timeline
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other) {
        if (hasTriggered) return; // �̹� ����Ǿ����� ����
        if (other.CompareTag("Player")) {
            hasTriggered = true;
            playableDirector.Resume();
        }
    }
}
