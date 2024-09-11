using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StateEffectManager : MonoBehaviour {
    private static StateEffectManager _instance;

    [SerializeField] List<GameObject> _stateObjs;


    #region properties

    public static StateEffectManager Instance { get { return _instance; } }

    #endregion // properties




    #region mono funcs
    private void Awake() {
        _instance = this;
    }

    #endregion




    #region public funcs

    public async UniTaskVoid SummonEffect(Transform caster, StateType stateType, float offsetY, float lastTime) {
        GameObject stateEffect = Instantiate(_stateObjs[(int)stateType], caster);
        stateEffect.transform.localPosition = Vector3.up * offsetY;
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        Destroy(stateEffect);
    }

    #endregion
}
