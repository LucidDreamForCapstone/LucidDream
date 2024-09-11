using System.Collections;
using UnityEngine;

public class GuardEffect : MonoBehaviour
{
    public float fadeOutDuration = 1.0f; // ���̵� �ƿ� �ð�

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // ����Ʈ�� 2�� ���� ������ �� ���̵� �ƿ� ����
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

        // ���̵� �ƿ� �Ϸ� �� ������Ʈ ����
        Destroy(gameObject);
    }
}

