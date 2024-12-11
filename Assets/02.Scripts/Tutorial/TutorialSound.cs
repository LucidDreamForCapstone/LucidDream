using UnityEngine;

public class TutorialSound : MonoBehaviour {
    [SerializeField] AudioClip _tutorialBGM;

    private void Start() {
        SoundManager.Instance.PlayBGM(_tutorialBGM.name);
    }
}
