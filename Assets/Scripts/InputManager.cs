using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject keyboardCanvas;
    public GameObject ps4Canvas;
    public GameObject xboxCanvas;

    void Update()
    {
        // Check if keyboard is being used
        if (Input.GetJoystickNames().Length == 0)
        {
            ShowCanvas(keyboardCanvas);
        }
        else
        {
            // Check if a controller is being used
            string controllerName = Input.GetJoystickNames()[0].ToLower();

            if (controllerName.Contains("ps") || controllerName.Contains("sony"))
            {
                // PS4 controller is being used
                ShowCanvas(ps4Canvas);
            }
            else if (controllerName.Contains("xbox"))
            {
                // Xbox controller is being used
                ShowCanvas(xboxCanvas);
            }
        }
    }

    void ShowCanvas(GameObject canvas)
    {
        // Deactivate all canvases
        keyboardCanvas.SetActive(false);
        ps4Canvas.SetActive(false);
        xboxCanvas.SetActive(false);

        // Activate the specified canvas
        canvas.SetActive(true);
    }
}

