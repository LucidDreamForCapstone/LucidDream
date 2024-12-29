using Cinemachine;
using Cysharp.Threading.Tasks;
using Edgar.Unity;
using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraManager : MonoBehaviour {
    private static CameraManager _instance;
    private Camera _camera;
    private PixelPerfectCamera _pixelPerfectCamera;
    private FogOfWarGrid2D _fow2d;
    [SerializeField] CinemachineVirtualCamera _virtualCamera;

    public static CameraManager Instance { get { return _instance; } }

    #region monofuncs
    private void Awake() {
        _instance = this;
        _camera = GetComponent<Camera>();
        _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        _fow2d = GetComponent<FogOfWarGrid2D>();
    }

    #endregion



    #region public funcs

    public async UniTask CameraFocus(Transform targetTransform, int offset, float focusTime) {
        Transform originTarget = _virtualCamera.Follow;
        Player player = originTarget.GetComponent<Player>();
        player.PlayerPause();
        player.PhantomForceCancel();
        player.CustomInvincible(focusTime + 1).Forget();
        _virtualCamera.Follow = targetTransform;
        _pixelPerfectCamera.assetsPPU -= offset;
        await UniTask.WaitUntil(() => Vector2.Distance(transform.position, targetTransform.position) < 0.1f);
        await UniTask.Delay(TimeSpan.FromSeconds(focusTime), ignoreTimeScale: true);
        _pixelPerfectCamera.assetsPPU += offset;
        player.PlayerUnPause();
        _virtualCamera.Follow = originTarget;
    }

    public void SetFogOfWar(bool state) {
        _fow2d.enabled = state;
    }

    public void SetFogOfwarTrue() {
        _fow2d.enabled = true;
    }

    #endregion
}
