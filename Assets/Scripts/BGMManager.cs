using UnityEngine;
using UnityEngine.UI;

public class BGMManager : MonoBehaviour
{
    public AudioClip[] m_BackgroundMusic;

    public Slider volumeSlider; // Reference to the volume slider in the UI

    public float m_DefaultVolume = 0.2f;

    private AudioSource audioSource;
    private int m_TrackNumber;

    private void Start()
    {
        // Initialize the AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false; // We'll handle looping manually

        // Set default volumes for each AudioClip
        audioSource.volume = m_DefaultVolume;

        m_TrackNumber = 0;

        PlayBGM(m_BackgroundMusic[m_TrackNumber]); // Start playing the first BGM
    }

    private void Update()
    {
        // Check if the current BGM has finished playing
        if (!audioSource.isPlaying)
        {
            //Play next in queue
            m_TrackNumber++;
            if (m_TrackNumber >= m_BackgroundMusic.Length)
            {
                m_TrackNumber = 0;
            }
            PlayBGM(m_BackgroundMusic[m_TrackNumber]);
        }
    }

    private void PlayBGM(AudioClip clip)
    {
        // Set the new BGM clip and start playing
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void ChangeVolume(float newVolume)
    {
        // Change the volume based on the slider value
        audioSource.volume = newVolume;
    }

    public void ToggleVolume()
    {
        if (audioSource.volume > 0)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = m_DefaultVolume;
        }
    }
}
