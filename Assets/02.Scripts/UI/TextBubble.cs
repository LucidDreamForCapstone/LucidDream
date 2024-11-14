/*using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TextBubble : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] AudioClip _textSound; // ���ڸ��� ��µǴ� �Ҹ�
    [SerializeField] TextMeshProUGUI _textUI; // UI �ؽ�Ʈ
    [SerializeField] List<SentenceData> _sentenceList; // ���� ������
    [SerializeField] int _fontSize; // �ؽ�Ʈ ũ��
    [SerializeField] Color _fontColor; // �ؽ�Ʈ ����

    private bool _isPrinting = false; // �ߺ� ���� ���� �÷���

    private void Start() {
        _textUI.text = "";
        _textUI.fontSize = _fontSize;
        _textUI.color = _fontColor;
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Signal Emitter���� ȣ�� ������ �޼��� (���� �ε����� ������� ����)
    /// </summary>
    public void TriggerPrintSentenceByIndex(int index) {
        Debug.Log($"TriggerPrintSentenceByIndex called with index: {index}");
        TriggerPrintSentenceAsync(index).Forget(); // �񵿱� �Լ� ȣ��
    }

    public void TriggerPrintSentence() {
        PrintSentence().Forget(); //UniTaskVoid �Լ��� ����Ƽ���� �ν��� ����.
    }

        // �ε��� ��ȿ�� �˻�
        if (sentenceIndex < 0 || sentenceIndex >= _sentenceList.Count) {
            Debug.LogError($"Invalid sentenceIndex: {sentenceIndex}. It must be between 0 and {_sentenceList.Count - 1}.");
            _isPrinting = false; // �ߺ� ���� ���� ����
            return;
        }

        _isPrinting = true;
        _textUI.text = ""; // ���� ���� ����

        SentenceData sentence = _sentenceList[sentenceIndex]; // ���� ��������
        foreach (var word in sentence.Words) {
            await PrintWord(word); // �ܾ� ���
        }

        _isPrinting = false;
    }


    /// <summary>
    /// �ܾ ����մϴ�.
    /// </summary>
    private async UniTask PrintWord(TextBubbleData data) {
        for (int i = 0; i < data._word.Length; i++) {
            _textUI.text += data._word[i]; // ���� �߰�

            if (_textSound != null) {
                _audioSource.PlayOneShot(_textSound); // �Ҹ� ���
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(data._charWaitTime)); // ���� ��� �ð�
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(data._wordWaitTime)); // �ܾ� ��� �ð�
        _textUI.text += " "; // �ܾ� �� ���� �߰�
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
        _textUI.text = ""; // ���� �ؽ�Ʈ �ʱ�ȭ
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
    [SerializeField] AudioClip _textSound; // ���ڸ��� ��µǴ� �Ҹ�
    [SerializeField] TextMeshProUGUI _textUI; // UI �ؽ�Ʈ
    [SerializeField] List<SentenceData> _sentenceList; // ���� ������
    [SerializeField] int _fontSize; // �ؽ�Ʈ ũ��
    [SerializeField] Color _fontColor; // �ؽ�Ʈ ����

    private bool _isPrinting = false; // �ߺ� ���� ���� �÷���

    private void Start() {
        _textUI.text = "";
        _textUI.fontSize = _fontSize;
        _textUI.color = _fontColor;
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Signal Emitter���� ȣ�� ������ �޼��� (���� �ε����� ������� ����)
    /// </summary>
    public void TriggerPrintSentenceByIndex(int index) {
        TriggerPrintSentenceAsync(index).Forget(); // �񵿱� �Լ� ȣ��
    }

    /// <summary>
    /// Ư�� ������ ����մϴ�.
    /// </summary>
    private async UniTask TriggerPrintSentenceAsync(int sentenceIndex) {
        if (_isPrinting) return; // �̹� ��� ���̸� ����
        if (sentenceIndex < 0 || sentenceIndex >= _sentenceList.Count) return; // �߸��� �ε��� ����

        _isPrinting = true;
        // ���� ���� ����
        _textUI.text = "";

        SentenceData sentence = _sentenceList[sentenceIndex]; // ���� ��������
        foreach (var word in sentence.Words) {
            await PrintWord(word); // �ܾ� ���
        }

        _isPrinting = false;
    }

    /// <summary>
    /// �ܾ ����մϴ�.
    /// </summary>
    private async UniTask PrintWord(TextBubbleData data) {
        for (int i = 0; i < data._word.Length; i++) {
            _textUI.text += data._word[i]; // ���� �߰�

            if (_textSound != null) {
                _audioSource.PlayOneShot(_textSound); // �Ҹ� ���
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(data._charWaitTime)); // ���� ��� �ð�
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(data._wordWaitTime)); // �ܾ� ��� �ð�
        _textUI.text += " "; // �ܾ� �� ���� �߰�
    }
}



