using System.Collections;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeDuration = 1f; // 淡出时间（秒）

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Invoke("StartFade", 7f); // 4 秒后开始淡出
        }
    }

    private void StartFade()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Color originalColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r * alpha, originalColor.g * alpha, originalColor.b * alpha, alpha);
            yield return null;
        }

        Destroy(gameObject); // 完全变黑后销毁物体
    }
}
