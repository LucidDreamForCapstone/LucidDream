using System;
using UnityEngine;

[Serializable]
public class SystemMessageData {
    public string _message;
    public Color _color;
    public bool _withSound;
    public float _lastTime;
    public float _fadeTime;


    public SystemMessageData(string message, Color color, bool withSound, float lastTime, float fadeTime) {
        _message = message;
        _color = color;
        _withSound = withSound;
        _lastTime = lastTime;
        _fadeTime = fadeTime;
    }
}
