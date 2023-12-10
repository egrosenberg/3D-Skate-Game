using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeMenu : MonoBehaviour
{
    public Image[] targetImages;
    public TextMeshProUGUI[] targetTexts;
    public float delayBeforeFade = 4.2f;
    public float fadeDuration = 2f;

    private void Start()
    {
        StartCoroutine(StartFade());
    }

    private IEnumerator StartFade()
    {
        // Wait for the delay before starting the fade
        yield return new WaitForSeconds(delayBeforeFade);

        // Start the fade-in process for each target image
        foreach (Image targetImage in targetImages)
        {
            if (targetImage != null)
            {
                StartCoroutine(FadeImage(targetImage, 0f, 1f, fadeDuration));
            }
        }

        // Start the fade-in process for each target TextMeshPro
        foreach (TextMeshProUGUI targetText in targetTexts)
        {
            if (targetText != null)
            {
                StartCoroutine(FadeText(targetText, 0f, 1f, fadeDuration));
            }
        }
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = image.color;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}
