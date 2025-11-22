using UnityEngine;
using UnityEngine.UI;

public class LevelTextFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f; // thời gian fade in/out
    public float displayTime = 5f;  // thời gian hiển thị

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(FadeRoutine());
    }

    private System.Collections.IEnumerator FadeRoutine()
    {
        // Fade In
        yield return Fade(0f, 1f, fadeDuration);

        // Chờ 5 giây hiển thị
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        yield return Fade(1f, 0f, fadeDuration);

        // Ẩn chữ
        gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator Fade(float start, float end, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, t / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = end;
    }
}
