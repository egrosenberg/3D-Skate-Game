using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject background;
    public GameObject masterVolumeSlider;
    public GameObject continueButton;
    public GameObject exitButton;
    public GameObject[] highlights;

    public TextMeshProUGUI optionsText;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI exitText;

    private int currentHighlightIndex = 0;
    private bool canChangeHighlight = true;
    private bool canChangeSlider = true;

    private void Start()
    {
        optionsMenu.SetActive(false);
        background.SetActive(false);

        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].SetActive(false);
        }
        highlights[currentHighlightIndex].SetActive(true);

        // Set the default value of the master volume slider to the current volume level
        masterVolumeSlider.GetComponent<Slider>().value = AudioListener.volume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            optionsMenu.SetActive(!optionsMenu.activeSelf);
            background.SetActive(optionsMenu.activeSelf);

            if (optionsMenu.activeSelf)
            {
                currentHighlightIndex = 0;
                UpdateHighlights();
                HandleSliderControls();
            }
        }

        if (optionsMenu.activeSelf)
        {
            HandleHighlightMovement();
            HandleSliderControls();
            HandleButtonPress();
        }
    }

    private void HandleHighlightMovement()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput > 0 && currentHighlightIndex > 0 && canChangeHighlight)
        {
            currentHighlightIndex--;
            canChangeHighlight = false;
            Invoke("EnableHighlightChange", 0.2f); // Allow highlight change after 0.2 seconds
        }
        else if (verticalInput < 0 && currentHighlightIndex < highlights.Length - 1 && canChangeHighlight)
        {
            currentHighlightIndex++;
            canChangeHighlight = false;
            Invoke("EnableHighlightChange", 0.2f); // Allow highlight change after 0.2 seconds
        }

        UpdateHighlights();
    }

    private void EnableHighlightChange()
    {
        canChangeHighlight = true;
    }

    private void UpdateHighlights()
    {
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].SetActive(i == currentHighlightIndex);
        }
    }

    private void HandleSliderControls()
    {
        if (!canChangeSlider) return;

        Slider currentSlider = null;

        switch (currentHighlightIndex)
        {
            case 0:
                currentSlider = masterVolumeSlider.GetComponent<Slider>();
                break;
        }

        if (currentSlider != null)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput != 0)
            {
                canChangeSlider = false;
                Invoke("EnableSliderChange", 0.1f); // Allow slider change after 0.1 seconds

                float currentValue = currentSlider.value;
                float newValue = Mathf.Clamp01(currentValue + horizontalInput * 0.1f); // Adjust sensitivity as needed
                currentSlider.value = newValue;

                // Update audio volume based on the master volume slider
                AudioListener.volume = masterVolumeSlider.GetComponent<Slider>().value;
            }
        }
    }

    private void EnableSliderChange()
    {
        canChangeSlider = true;
    }

    private void HandleButtonPress()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (currentHighlightIndex == highlights.Length - 2) // Continue button
            {
                optionsMenu.SetActive(false);
            }
            else if (currentHighlightIndex == highlights.Length - 1) // Exit button
            {
                SceneManager.LoadScene("Menu");
                Debug.Log("Exiting to Menu...");
            }
        }
    }
}
