using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SystemMessageManager : MonoBehaviour {
    private static SystemMessageManager _instance;
    private bool _isPrinting;
    private List<SystemMessageData> _messageList;
    [SerializeField] TextMeshProUGUI _systemMessageTM;
    [SerializeField] AudioClip _warningSound;

    [SerializeField] List<RectTransform> _dialogBoxList;
    [SerializeField] List<TextMeshProUGUI> _dialogNameTMList;
    [SerializeField] List<TextMeshProUGUI> _dialogMessageTMList;
    [SerializeField] AudioClip _dialogStartSound;
    [SerializeField] AudioClip _dialogEndSound;

    bool _isReady;
    bool[] _isDialogBoxReady = { true, true };

    private void Awake() {
        _instance = this;
        _systemMessageTM.color = new Color(0, 0, 0, 0);
        _messageList = new List<SystemMessageData>();
        _isPrinting = false;
        _isReady = true;
        _dialogBoxList.ForEach(box => box.gameObject.SetActive(false));
    }

    private void Update() {
        PrintSystemMessage().Forget();
    }

    public static SystemMessageManager Instance { get { return _instance; } }


    public async UniTaskVoid ShowDialogBox(string name, List<string> messages, float lastTime = 3.0f) {
        int index = GetDialogBox();
        _dialogNameTMList[index].text = name;
        _dialogBoxList[index].DOMoveX(_dialogBoxList[index].rect.x + 860, 2).SetUpdate(true).ToUniTask().Forget();
        for (int i = 0; i < messages.Count; i++) {
            SoundManager.Instance.PlaySFX(_dialogStartSound.name, true);
            await DynamicDialogText(index, messages[i]);
            await UniTask.Delay(TimeSpan.FromSeconds(lastTime), ignoreTimeScale: true);
        }
        if (_isDialogBoxReady[0] || _isDialogBoxReady[1])
            SoundManager.Instance.PlaySFX(_dialogEndSound.name, true);
        await _dialogBoxList[index].DOMoveX(_dialogBoxList[index].rect.x - 860, 2).SetUpdate(true);
        _isDialogBoxReady[index] = true;
    }


    public void PushSystemMessage(string message, Color color, bool withSound = true, float lastTime = 1.0f, float fadeTime = 0.6f) {
        if (_isReady && (_messageList.Count == 0 || (_messageList.Count > 0 && _messageList.Last()._message != message))) {
            _messageList.Add(new SystemMessageData(message, color, withSound, lastTime, fadeTime));
            PushWait().Forget();
        }
    }

    private async UniTaskVoid PrintSystemMessage() {
        if (_messageList.Count > 0 && !_isPrinting) {
            _isPrinting = true;
            SystemMessageData messageData = _messageList.First();
            _messageList.RemoveAt(0);
            if (messageData._withSound)
                SoundManager.Instance.PlaySFX(_warningSound.name, true);
            Color color = messageData._color;
            float lastTime = messageData._lastTime;
            float fadeTime = messageData._fadeTime;
            _systemMessageTM.text = messageData._message;
            _systemMessageTM.color = new Color(color.r, color.g, color.b, 0);
            await _systemMessageTM.DOFade(1, fadeTime).SetUpdate(true);
            await UniTask.Delay(TimeSpan.FromSeconds(lastTime), ignoreTimeScale: true);
            await _systemMessageTM.DOFade(0, fadeTime).SetUpdate(true);
            _systemMessageTM.text = "";
            _isPrinting = false;
        }
    }

    private async UniTask DynamicDialogText(int index, string message, float delay = 0.05f) {
        string[] words = message.Split(' ');
        _dialogMessageTMList[index].text = "";
        for (int i = 0; i < words.Length; i++) {
            _dialogMessageTMList[index].text += words[i] + " ";
            await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: true);
        }
    }

    private int GetDialogBox() {
        if (_isDialogBoxReady[0] || !_isDialogBoxReady[0] && !_isDialogBoxReady[1]) {
            _isDialogBoxReady[0] = false;
            _dialogBoxList[0].gameObject.SetActive(true);
            _dialogBoxList[1].gameObject.SetActive(false);
            return 0;
        }
        else {
            _isDialogBoxReady[1] = false;
            _dialogBoxList[0].gameObject.SetActive(false);
            _dialogBoxList[1].gameObject.SetActive(true);
            return 1;
        }
    }

    private async UniTaskVoid PushWait() {
        _isReady = false;
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        _isReady = true;
    }
}
