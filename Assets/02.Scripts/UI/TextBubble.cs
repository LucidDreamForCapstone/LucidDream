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



