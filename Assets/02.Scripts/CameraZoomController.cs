using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class CameraZoomController : MonoBehaviour
{
    private PixelPerfectCamera pixelPerfectCamera;

    private void Start() {
        // Pixel Perfect Camera ������Ʈ ã��
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        if (pixelPerfectCamera == null) {
            Debug.LogError("Pixel Perfect Camera ������Ʈ�� ã�� �� �����ϴ�.");
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
