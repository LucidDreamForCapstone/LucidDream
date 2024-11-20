using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour {
    private static TimeScaleManager _instance;
    private float _timeScaleCache;
    private List<Animator> _effectList = new List<Animator>();

    public static TimeScaleManager Instance { get { return _instance; } }

    private void Awake() {
        _instance = this;
        _timeScaleCache = 1;
    }

    /*
    /// <param name="timeScale">must be [0, 1]</param>
    public void TimeSlow(float timeScale) {
        if (timeScale == 0) {
            IsTimeFlow = false;
        }
        if (Time.timeScale > timeScale) {
            _timeScaleCache = Time.timeScale;
            Time.timeScale = timeScale;
        }
    }
    */

    public void TimeStop() {
        if (Time.timeScale > 0) {
            _timeScaleCache = Time.timeScale;
            SoundManager.Instance.PauseSFX();
            PauseEffects();
            Time.timeScale = 0;
        }
    }

    public void TimeRestore() {
        Time.timeScale = _timeScaleCache;
        UnPauseEffects();
        SoundManager.Instance.UnPauseSFX();
    }



    public async UniTask TimeSlowLerp(float timeScale, float lerpTime) {
        await DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScale, lerpTime);
    }

    public async UniTask TimeRestoreLerp(float lerpTime) {
        await DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, lerpTime);
    }

    public void AddAnimator(GameObject effectObj) {
        _effectList.Add(effectObj.GetComponent<Animator>());
    }

    public void AddAnimator(Animator animator) {
        _effectList.Add(animator);
    }

    public void RemoveAnimator(GameObject effectObj) {
        _effectList.Remove(effectObj.GetComponent<Animator>());
    }

    public void RemoveAnimator(Animator animator) {
        _effectList.Remove(animator);
    }

    private void PauseEffects() {
        int length = _effectList.Count;
        for (int i = 0; i < length; i++) {
            _effectList[i].enabled = false;
        }
    }

    private void UnPauseEffects() {
        int length = _effectList.Count;
        for (int i = 0; i < length; i++) {
            _effectList[i].enabled = true;
        }
    }
}
