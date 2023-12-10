using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Assign the corresponding scene name in the Inspector
    public string skateparkSceneName = "skatepark";

    // Delay before input is enabled (in seconds)
    private float activationDelay = 4.2f;
    private bool inputEnabled = false;

    private void Start()
    {
        // Start the activation delay coroutine
        StartCoroutine(EnableInputAfterDelay());
    }

    private IEnumerator EnableInputAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        inputEnabled = true;
    }

    private void Update()
    {
        // Check if input is enabled
        if (!inputEnabled)
            return;

        // Check keyboard input
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Load the "skatepark" scene when Enter is pressed
            SceneManager.LoadScene(skateparkSceneName);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit the application when ESC is pressed
            Application.Quit();
        }

        // Check Xbox controller input
        if (Input.GetButtonDown("Submit")) // "A" button on Xbox controller (also "X" for PS4 controllers)
        {
            SceneManager.LoadScene(skateparkSceneName);
        }

        if (Input.GetButtonDown("Cancel")) // "B" button on Xbox controller (also "O" for PS4 controllers)
        {
            Application.Quit();
        }
    }
}
