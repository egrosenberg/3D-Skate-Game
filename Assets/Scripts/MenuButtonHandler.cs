using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{
    // Attach this script to each menu button in Unity's UI system

    public void OnStartButtonClicked()
    {
        // Handle the start button click
        SceneManager.LoadScene("ingame");
        // Replace "skatepark" with the actual name of your scene file
    }

    public void OnExitButtonClicked()
    {
        // Handle the exit button click
        Debug.Log("Exiting the game");
        Application.Quit();
        // Note: Application.Quit() might not work in the Unity Editor. It's best tested in a built executable.
    }
}
