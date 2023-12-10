using UnityEngine;
using TMPro;

public class TextMeshProFall : MonoBehaviour
{
    public TextMeshProUGUI textMeshProText;
    public GameObject[] images;

    public float fallSpeed = 5f;

    private bool isFalling = false;
    private Vector3[] initialPositions;

    private void Start()
    {
        // Disable text and images initially
        textMeshProText.enabled = false;
        foreach (GameObject image in images)
        {
            image.SetActive(false);
        }

        // Store initial positions
        StoreInitialPositions();

        // Invoke falling after 5 seconds
        Invoke("StartFalling", 2f);
    }

    private void Update()
    {
        if (isFalling)
        {
            // Move text and images down
            float step = fallSpeed * Time.deltaTime;
            textMeshProText.transform.Translate(Vector3.down * step);
            for (int i = 0; i < images.Length; i++)
            {
                images[i].transform.Translate(Vector3.down * step);
            }

            // Check if they have reached their final position
            if (textMeshProText.transform.position.y <= initialPositions[0].y)
            {
                isFalling = false;
                // Ensure they are exactly at the target position
                textMeshProText.transform.position = initialPositions[0];
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].transform.position = initialPositions[i + 1];
                }
            }
        }
    }

    private void StartFalling()
    {
        // Enable text and images
        textMeshProText.enabled = true;
        foreach (GameObject image in images)
        {
            image.SetActive(true);
        }

        // Set falling to true to start the animation
        isFalling = true;
    }

    private void StoreInitialPositions()
    {
        // Store initial positions of text and images
        initialPositions = new Vector3[images.Length + 1];
        initialPositions[0] = textMeshProText.transform.position;

        for (int i = 0; i < images.Length; i++)
        {
            initialPositions[i + 1] = images[i].transform.position;
            images[i].transform.position = new Vector3(images[i].transform.position.x, Screen.height + initialPositions[i + 1].y, images[i].transform.position.z);
        }
    }
}
