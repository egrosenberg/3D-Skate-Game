using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public AudioClip bgm1;
    public AudioClip bgm2;

    private AudioSource audioSource;

    private void Start()
    {
        // Initialize the AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false; // We'll handle looping manually
        audioSource.volume = 0.1f; // Set initial volume
        PlayInitialBGM();
    }

    private void PlayInitialBGM()
    {
        // Set the initial BGM clip and start playing
        audioSource.clip = bgm1;
        audioSource.Play();

        // Invoke a method to transition to the looping BGM after the first clip finishes
        Invoke("SwitchToLoopingBGM", bgm1.length);
    }

    private void SwitchToLoopingBGM()
    {
        // Set the looping BGM clip and enable looping
        audioSource.clip = bgm2;
        audioSource.loop = true;

        // Start playing the looping BGM
        audioSource.Play();
    }
}
