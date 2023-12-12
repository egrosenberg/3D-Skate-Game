using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI latestScoreText;
    public TextMeshProUGUI highScoreText;

    public AudioSource scoreIncreaseSound; // Add this line
    public AudioClip scoreIncreaseClip; // Add this line

    private int latestScore = 0;
    private int highScore = 0;

    // Duration of the digit-changing animation
    public float animationDuration = 0.05f;

    private void Start()
    {
        // Load the high score from the player's preferences
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreText();
    }

    private void Update()
    {
        // Call HandleInput in the Update method to check for keyboard input
        HandleInput();
    }

    private void UpdateScoreText()
    {
        latestScoreText.text = latestScore.ToString();
        highScoreText.text = highScore.ToString();
    }

    public void IncreaseScore(int amount)
    {
        StartCoroutine(DigitChangingAnimation(amount));
    }

    private IEnumerator DigitChangingAnimation(int amount)
    {
        int targetScore = latestScore + amount;
        float timer = 0f;

        while (timer < animationDuration)
        {
            float progress = timer / animationDuration;
            int animatedScore = Mathf.RoundToInt(Mathf.Lerp(latestScore, targetScore, progress));
            latestScoreText.text = animatedScore.ToString();

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the final score is set correctly
        latestScore = targetScore;

        // Update high score after the animation completes
        UpdateScoreText();

        // Play the score increase sound
        if (scoreIncreaseSound != null && scoreIncreaseClip != null)
        {
            scoreIncreaseSound.PlayOneShot(scoreIncreaseClip);
        }

        // Update high score if the latest score surpasses the current high score
        if (latestScore > highScore)
        {
            highScore = latestScore;
            PlayerPrefs.SetInt("HighScore", highScore); // Save the high score to player preferences
        }
    }

    // Test method to increase the score for testing purposes
    public void TestIncreaseScore()
    {
        IncreaseScore(10); // Modify the score amount as needed
    }

    // Test method to reset the current score to 0 for testing purposes
    public void TestResetScore()
    {
        latestScore = 0;
        UpdateScoreText();
    }

    private void HandleInput()
    {
        // Check for keyboard input
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            TestIncreaseScore();
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            TestResetScore();
        }
    }

    public void PlaySound()
    {
        // Play the score increase sound
        if (scoreIncreaseSound != null && scoreIncreaseClip != null)
        {
            scoreIncreaseSound.PlayOneShot(scoreIncreaseClip);
        }
    }
}
