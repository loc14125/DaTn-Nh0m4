using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public MonoBehaviour movementScript; // script điều khiển nhân vật
    public float delay = 2f;
    public float fadeDuration = 1f;

    void Start()
    {
        // Tàng hình player
        Color c = spriteRenderer.color;
        c.a = 0f;
        spriteRenderer.color = c;

        // Tắt chức năng di chuyển
        movementScript.enabled = false;

        // Sau 2 giây bắt đầu hiện lên
        Invoke(nameof(FadeIn), delay);
    }

    void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    System.Collections.IEnumerator FadeInRoutine()
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;

            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;

            yield return null;
        }

        // Khi fade-in xong → bật lại di chuyển
        movementScript.enabled = true;
    }
}
