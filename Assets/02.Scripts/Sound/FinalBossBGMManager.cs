using UnityEngine;

public class FinalBossBGMManager : MonoBehaviour {
    [SerializeField] AudioClip _bossBGM;
    private void Start() {
        SoundManager.Instance.PlayBGM(_bossBGM.name);
    }
}
