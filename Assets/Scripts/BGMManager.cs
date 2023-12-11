using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip bgm1;
    public AudioClip bgm2;

    // Set manual default volumes for each AudioClip
    public float defaultVolumeBGM1 = 0.2f;
    public float defaultVolumeBGM2 = 0.1f;

    private AudioSource audioSource;

    private void Start()
    {
        // Initialize the AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // We'll handle looping manually

        // Set default volumes for each AudioClip
        audioSource.volume = (audioSource.clip == bgm1) ? defaultVolumeBGM1 : defaultVolumeBGM2;

        PlayBGM(bgm2); // Start playing the first BGM
    }

    private void Update()
    {
        // Check if the current BGM has finished playing
        //if (!audioSource.isPlaying)
        //{
        //    // Switch to the other BGM and play it
        //    if (audioSource.clip == bgm1)
        //    {
        //        PlayBGM(bgm2);
        //    }
        //    else
        //    {
        //        PlayBGM(bgm1);
        //    }
        //}
        //
        //// Manual Controls
        //HandleManualControls();
    }

    private void PlayBGM(AudioClip clip)
    {
        // Set the new BGM clip and start playing
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void HandleManualControls()
    {
        // Toggle between BGMs when pressing the 'Space' key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleBGM();
        }

        // Increase volume when pressing the 'Up' arrow key
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseVolume();
        }

        // Decrease volume when pressing the 'Down' arrow key
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseVolume();
        }
    }

    private void ToggleBGM()
    {
        // Toggle between the two BGMs
        if (audioSource.clip == bgm1)
        {
            PlayBGM(bgm2);
        }
        else
        {
            PlayBGM(bgm1);
        }
    }

    private void IncreaseVolume()
    {
        // Increase the volume (clamp between 0 and 1)
        audioSource.volume = Mathf.Clamp01(audioSource.volume + 0.05f);
    }

    private void DecreaseVolume()
    {
        // Decrease the volume (clamp between 0 and 1)
        audioSource.volume = Mathf.Clamp01(audioSource.volume - 0.05f);
    }
}