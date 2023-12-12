using System.Collections;
using System.Collections.Generic;
using Unity.Editor.Tasks;
using UnityEngine;

public class AudsrcMoving : MonoBehaviour
{
    private AudioSource audsrc;
    [SerializeField]
    private AudioClip audioMoveGround, audioGrinding;
    [SerializeField]
    private float pitchMinMoveGround = 0.8f, pitchMaxMoveGround = 1.5f, pitchMinDestabilize = 0.8f, pitchMaxDestabilize = 1.5f, pitchMinGrinding = 0.8f, pitchMaxGrinding = 1.5f;
    [SerializeField]
    private float minSpeedVolume = 0.2f, maxSpeedVolume = 0.8f, transitionSpeed = 0.5f;
    private int moveType = 1;       // 1 = ground movement
                                    // 2 = airborne destabilization
                                    // 3 = grinding
    // Start is called before the first frame update
    void Start()
    {
        audsrc = GetComponent<AudioSource>();
        if (audsrc == null) {
            Debug.LogError("AudsrcMoving: AudioSource could not be found!");
        }
    }

    public void PlayMoveGround() {
        moveType = 1;
        audsrc.clip = audioMoveGround;
        audsrc.Play();
    }

    public void PlayDestabilize() {
        Debug.Log("DEBUG: " + audsrc);
        audsrc.Stop();
    }

    public void PlayGrinding() {
        moveType = 3;
        audsrc.clip = audioGrinding;
        audsrc.Play();
    }

    public void ModifyMovementSound(float velocity) {
        switch (moveType) {
            case 1:
            audsrc.pitch = Mathf.Lerp(pitchMinMoveGround, pitchMaxMoveGround, velocity);
            audsrc.volume = Mathf.Lerp(0, maxSpeedVolume, velocity);
            //Debug.Log("DEBUG: " + velocity + ", " + audsrc.pitch);
            break;

            case 2:
            audsrc.pitch = Mathf.Lerp(pitchMinDestabilize, pitchMaxDestabilize, velocity);
            break;
            
            case 3:
            audsrc.pitch = Mathf.Lerp(pitchMinGrinding, pitchMaxGrinding, velocity);
            if (velocity < minSpeedVolume) {
                audsrc.volume = 0;
            }
            else {
                // volume = % of max speed reached / % of wheels on the ground
                audsrc.volume = Mathf.Lerp(0, maxSpeedVolume - minSpeedVolume, velocity - minSpeedVolume);
            }
            break;
        }
    }
}
