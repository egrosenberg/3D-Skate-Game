using System.Collections;
using TMPro;
using UnityEngine;

public class LogoFall : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public GameObject imageObject1;
    public GameObject imageObject2;

    public float startDelay = 2f;
    public float fallDuration = 2f;
    public float textY = 280f;
    public float image1Y = -79f;
    public float image2Y = -30f;

    private void Start()
    {
        // Delay the animation start
        Invoke("StartAnimation", startDelay);
    }

    private void StartAnimation()
    {
        // Animate TextMeshPro falling
        RectTransform textTransform = textMeshPro.rectTransform;
        Vector3 textStartPosition = textTransform.localPosition;
        Vector3 textEndPosition = new Vector3(textStartPosition.x, textY, textStartPosition.z);
        StartCoroutine(MoveObject(textTransform, textStartPosition, textEndPosition, fallDuration, () => Debug.Log("Text Animation Complete")));

        // Animate Image1 falling
        RectTransform imageTransform1 = imageObject1.GetComponent<RectTransform>();
        Vector3 image1StartPosition = imageTransform1.localPosition;
        Vector3 image1EndPosition = new Vector3(image1StartPosition.x, image1Y, image1StartPosition.z);
        StartCoroutine(MoveObject(imageTransform1, image1StartPosition, image1EndPosition, fallDuration, () => Debug.Log("Image1 Animation Complete")));

        // Animate Image2 falling
        RectTransform imageTransform2 = imageObject2.GetComponent<RectTransform>();
        Vector3 image2StartPosition = imageTransform2.localPosition;
        Vector3 image2EndPosition = new Vector3(image2StartPosition.x, image2Y, image2StartPosition.z);
        StartCoroutine(MoveObject(imageTransform2, image2StartPosition, image2EndPosition, fallDuration, () => Debug.Log("Image2 Animation Complete")));
    }

    private IEnumerator MoveObject(RectTransform transform, Vector3 startPos, Vector3 endPos, float duration, System.Action onComplete)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}
