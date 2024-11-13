/*using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBubble : MonoBehaviour {
    AudioSource _audioSource;
    [SerializeField] AudioClip _textSound;
    [SerializeField] TextMeshProUGUI _textUI;
    [SerializeField] List<TextBubbleData> _wordList;
    [SerializeField] int _fontSize;
    [SerializeField] Color _fontColor;

    #region mono funcs
    private void Start() {
        _textUI.text = "";
        _textUI.fontSize = _fontSize;
        _textUI.color = _fontColor;
        _audioSource = GetComponent<AudioSource>();
    }

    #endregion

    public void TriggerPrintSentence() {
        PrintSentence().Forget(); //UniTaskVoid 함수는 유니티에서 인식을 못함.
    }


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
} */

/*
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] AudioClip _textSound;
    [SerializeField] TextMeshProUGUI _textUI;
    [SerializeField] List<TextBubbleData> _wordList;
    [SerializeField] int _fontSize;
    [SerializeField] Color _fontColor;

    #region mono funcs
    private void Start() {
        _textUI.text = "";
        _textUI.fontSize = _fontSize;
        _textUI.color = _fontColor;
        _audioSource = GetComponent<AudioSource>();
    }

    #endregion

    public void TriggerPrintSentence(int index) {
        if (index >= 0 && index < _wordList.Count) {
            PrintSentence(_wordList[index]).Forget();
        }
    }

    #region private funcs

    private async UniTaskVoid PrintSentence(TextBubbleData data) {
        _audioSource.Play();
        _textUI.text = ""; // 이전 텍스트 초기화
        for (int j = 0; j < data._word.Length - 1; j++) {
            _textUI.text += data._word[j];
            await UniTask.Delay(System.TimeSpan.FromSeconds(data._charWaitTime));
        }
        _textUI.text += data._word[^1];
        _audioSource.Pause();
        await UniTask.Delay(System.TimeSpan.FromSeconds(data._wordWaitTime));
    }

    #endregion
} */




using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TextBubble : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] AudioClip _textSound; // 글자마다 출력되는 소리
    [SerializeField] TextMeshProUGUI _textUI; // UI 텍스트
    [SerializeField] List<SentenceData> _sentenceList; // 문장 데이터
    [SerializeField] int _fontSize; // 텍스트 크기
    [SerializeField] Color _fontColor; // 텍스트 색상

    private bool _isPrinting = false; // 중복 실행 방지 플래그

    private void Start() {
        _textUI.text = "";
        _textUI.fontSize = _fontSize;
        _textUI.color = _fontColor;
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Signal Emitter에서 호출 가능한 메서드 (문장 인덱스를 기반으로 실행)
    /// </summary>
    public void TriggerPrintSentenceByIndex(int index) {
        TriggerPrintSentenceAsync(index).Forget(); // 비동기 함수 호출
    }

    /// <summary>
    /// 특정 문장을 출력합니다.
    /// </summary>
    private async UniTask TriggerPrintSentenceAsync(int sentenceIndex) {
        if (_isPrinting) return; // 이미 출력 중이면 무시
        if (sentenceIndex < 0 || sentenceIndex >= _sentenceList.Count) return; // 잘못된 인덱스 방지

        _isPrinting = true;
        // 이전 문장 제거
        _textUI.text = "";

        SentenceData sentence = _sentenceList[sentenceIndex]; // 문장 가져오기
        foreach (var word in sentence.Words) {
            await PrintWord(word); // 단어 출력
        }

        _isPrinting = false;
    }

    /// <summary>
    /// 단어를 출력합니다.
    /// </summary>
    private async UniTask PrintWord(TextBubbleData data) {
        for (int i = 0; i < data._word.Length; i++) {
            _textUI.text += data._word[i]; // 글자 추가

            if (_textSound != null) {
                _audioSource.PlayOneShot(_textSound); // 소리 출력
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(data._charWaitTime)); // 글자 대기 시간
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(data._wordWaitTime)); // 단어 대기 시간
        _textUI.text += " "; // 단어 간 공백 추가
    }
}



