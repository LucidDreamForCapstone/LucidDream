using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StateEffectManager : MonoBehaviour {
    private static StateEffectManager _instance;

    [SerializeField] List<GameObject> _stateObjs;
    [SerializeField] List<AudioClip> _stateSounds;
    [SerializeField] Material _coldMat;


    #region properties

    public static StateEffectManager Instance { get { return _instance; } }

    #endregion // properties




    #region mono funcs
    private void Awake() {
        _instance = this;
    }

    #endregion




    #region public funcs

    public async UniTaskVoid SummonEffect(Transform caster, StateType stateType, float offsetY, float lastTime, float scale = 1) {
        GameObject stateEffect = Instantiate(_stateObjs[(int)stateType], caster);
        stateEffect.transform.localPosition = Vector3.up * offsetY;
        stateEffect.transform.localScale = new Vector3(scale, scale, 1);
        if (_stateSounds[(int)stateType] != null)
            SoundManager.Instance.PlaySFX(_stateSounds[(int)stateType].name);
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        Destroy(stateEffect);
    }

    public Material GetColdMat() {
        return _coldMat;
    }

    #endregion
}
