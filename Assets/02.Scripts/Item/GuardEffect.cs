using System.Collections;
using UnityEngine;

public class GuardEffect : MonoBehaviour
{
    public float fadeOutDuration = 1.0f; // 페이드 아웃 시간

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // 이펙트가 2초 동안 유지된 후 페이드 아웃 시작
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 페이드 아웃 완료 후 오브젝트 제거
        Destroy(gameObject);
    }
}

