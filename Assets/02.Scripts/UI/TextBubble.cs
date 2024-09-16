using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBubble : MonoBehaviour {
    AudioSource _audioSource;
    [SerializeField] AudioClip _textSound;
    [SerializeField] TextMeshProUGUI _textUI;
    [SerializeField] List<TextBubbleData> _wordList;

    #region mono funcs
    private void Start() {
        _textUI.text = "";
        _audioSource = GetComponent<AudioSource>();
        PrintSentence().Forget();
    }

    #endregion



    #region private funcs

    private async UniTaskVoid PrintSentence() {
        _audioSource.Play();
        int j, length = _wordList.Count;
        for (int i = 0; i < length; i++) {
            _audioSource.UnPause();
            TextBubbleData data = _wordList[i];
            for (j = 0; j < data._word.Length - 1; j++) {
                _textUI.text += data._word[j];
                await UniTask.Delay(System.TimeSpan.FromSeconds(data._charWaitTime));
            }
            _textUI.text += data._word[j];
            _audioSource.Pause();
            await UniTask.Delay(System.TimeSpan.FromSeconds(data._wordWaitTime));
        }
    }

    #endregion
}
