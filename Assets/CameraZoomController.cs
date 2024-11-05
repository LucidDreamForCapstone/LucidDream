using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class CameraZoomController : MonoBehaviour
{
    private PixelPerfectCamera pixelPerfectCamera;

    private void Start() {
        // Pixel Perfect Camera 컴포넌트 찾기
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        if (pixelPerfectCamera == null) {
            Debug.LogError("Pixel Perfect Camera 컴포넌트를 찾을 수 없습니다.");
            return;
        }
    }

    public void ZoomIn(float newPixelsPerUnit, float duration) {
        StartCoroutine(AdjustPixelsPerUnit(newPixelsPerUnit, duration));
    }

    public void ZoomOut(float originalPixelsPerUnit, float duration) {
        StartCoroutine(AdjustPixelsPerUnit(originalPixelsPerUnit, duration));
    }

    private IEnumerator AdjustPixelsPerUnit(float targetValue, float duration) {
        float startValue = pixelPerfectCamera.assetsPPU;
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            pixelPerfectCamera.assetsPPU = (int)Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pixelPerfectCamera.assetsPPU = (int)targetValue;
    }
}
