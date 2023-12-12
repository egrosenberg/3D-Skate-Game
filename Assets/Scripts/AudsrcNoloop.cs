using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudsrcNoloop : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioLand;
    public AudioClip audioGrindEnter;
    public AudioClip audioGrindExit;
    private AudioSource audsrc;
    // Start is called before the first frame update
    void Start()
    {
        audsrc = GetComponent<AudioSource>();
        if (audsrc == null) {
            Debug.LogError("AudsrcNoloop: AudioSource could not be found!");
        }
    }

    public void PlayJump() {
        audsrc.PlayOneShot(audioJump);
    }
    public void PlayLand() {
        audsrc.PlayOneShot(audioLand);
    }
    public void PlayGrindEnter() {
        audsrc.PlayOneShot(audioGrindEnter);
    }
    public void PlayGrindExit() {
        audsrc.PlayOneShot(audioGrindExit);
    }
}
