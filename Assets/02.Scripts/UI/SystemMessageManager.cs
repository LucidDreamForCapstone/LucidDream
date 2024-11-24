using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemMessageManager : MonoBehaviour {
    private static SystemMessageManager _instance;
    private bool _isPrinting;
    private Queue<SystemMessageData> _q;
    [SerializeField] TextMeshProUGUI _systemMessageTM;
    [SerializeField] AudioClip _warningSound;

    private void Awake() {
        _instance = this;
        _systemMessageTM.color = new Color(0, 0, 0, 0);
        _q = new Queue<SystemMessageData>();
        _isPrinting = false;
    }

    private void Update() {
        PrintSystemMessage().Forget();
    }

    public static SystemMessageManager Instance { get { return _instance; } }

    public void PushSystemMessage(string message, Color color, bool withSound = true, float lastTime = 1.0f, float fadeTime = 0.6f) {
        _q.Enqueue(new SystemMessageData(message, color, withSound, lastTime, fadeTime));
    }

    private async UniTaskVoid PrintSystemMessage() {
        if (_q.Count > 0 && !_isPrinting) {
            _isPrinting = true;
            SystemMessageData messageData = _q.Dequeue();
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
}
