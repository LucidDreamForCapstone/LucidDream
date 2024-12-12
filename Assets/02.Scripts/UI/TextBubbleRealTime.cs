using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBubbleRealTime : MonoBehaviour {
    AudioSource _audioSource;
    [SerializeField] AudioClip _textSound;
    [SerializeField] TextMeshProUGUI _textUI;

    private bool _isPrinting = false;

    private void Start() {
        _textUI.text = "";
        gameObject.SetActive(false);
    }

    private async UniTask PrintSentence(string sentence, float interval = 0.1f, float afterDelay = 2) {
        gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX(_textSound.name);
        for (int i = 0; i < sentence.Length; i++) {
            _textUI.text += sentence[i];
            await UniTask.Delay(System.TimeSpan.FromSeconds(interval), ignoreTimeScale: true);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(afterDelay), ignoreTimeScale: true);
        _textUI.text = "";
        gameObject.SetActive(false);
    }


    public async void PrintSentenceSequence(List<string> sentenceList, float interval = 0.1f, float afterDelay = 2) {
        if (!_isPrinting) {
            _isPrinting = true;
            for (int i = 0; i < sentenceList.Count; i++) {
                await PrintSentence(sentenceList[i], interval, afterDelay);
            }
            _isPrinting = false;
        }
        else {
            Debug.Log("Text Bubble is Skipped.");
        }
    }
}
