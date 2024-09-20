using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour {
    private static TimeScaleManager _instance;
    private float _timeScaleCache; // no cache = -1

    public static TimeScaleManager Instance { get { return _instance; } }
    public static bool IsTimeFlow { get; private set; }

    private void Awake() {
        _instance = this;
        _timeScaleCache = -1;
        IsTimeFlow = true;
    }


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

    public void TimeRestore() {
        if (_timeScaleCache != -1) {
            Time.timeScale = _timeScaleCache;
            _timeScaleCache = -1;
        }
        else
            Time.timeScale = 1;

        if (!IsTimeFlow)
            IsTimeFlow = true;
    }

    public async UniTask TimeSlowLerp(float timeScale, float lerpTime) {
        await DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScale, lerpTime);
    }

    public async UniTask TimeRestoreLerp(float lerpTime) {
        await DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, lerpTime);
    }
}
