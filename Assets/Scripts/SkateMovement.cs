using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class SkateMovement : MonoBehaviour
{
    public MenuController m_MenuController;
    public PlayerInput m_PlayerInput;
    public Rigidbody m_Rigidbody;
    public GameObject m_BoardMesh;
    public GameObject m_BoostTrail;
    public ConstantForce m_ConstantForce;
    public Wheel[] m_Wheels;
    public ScoreManager m_ScoreManager;
    public float m_SpeedScalar = 1000f;
    public float m_BoostScalar = 1000f;
    public float m_RotationScalar = 0.1f;
    public float m_MaxSpeed = 7f;
    public float m_JumpStr = 20f;
    public float m_CorrectionFactor = 10f;
    public float m_MinSpeed = 0f;
    public float m_DownwardForce = 0f;
    public float m_PivotTorqueScalar = 10f;
    public float m_PivotRotateScalar = 6f;
    public float m_ForwardPivotScalar = 2f;
    public float m_GrindPivotScalar = 20f;
    public float m_MaxAngularV = 50f;
    public float m_ManualForce = 1f;
    public float m_ManualSlamScalar = 1f;
    public float m_ConstantForceScalar = 1f;
    public float m_AccelerateInterval = 0.5f;
    public float m_ScoreScalar = 1f;
    public float m_ScoreSpinScalar = 1f;
    public float m_ScoreGrindScalar = 2f;
    public float m_SFXcooldown = 0.5f;
    public int m_MinScoreThreshold = 50;
    public Vector3 m_CenterOfMass = Vector3.zero;


    private bool        m_Reorienting;        // If the board currently needs to reorient itself
    private float       m_CurrentSpeed;       // Speed for forward input
    private float       m_RotationSpeed;      // Speed the rotation input is at
    private float       m_NextAccelerate;     // Used to track when to next push the board
    private float       m_LastLandSFX;        // Tracks las time the land sfx was played
    private int         m_GroundedContacts;   // Number of wheels touching the ground
    private int         m_CurrentComboScore;  // The score in our current scoring combo
    private int         m_TotalScore;         // The score for the whole session
    private bool        m_Destabilizing;      // If the board is currently in destabilizing mode
    private bool        m_Grounded;           // If the board currently has any wheels touching the ground
    private bool        m_Jumping;            // True if player has recently pressed jump input
    private bool        m_Grinding;           // True if player is currently in the middle of a grind
    private bool        m_GrindingCanRotate;  // Used to decide whether or not to respect rotation input while grinding
    private bool        m_InputMode;          // Tracks whether it is menu input or board input : board = true
    private Quaternion  m_JumpRotation;       // Angle board was at as it left for a jump
    private AudsrcMoving m_AudSrc;
    private AudsrcNoloop m_AudSrc_NL;

    private void Start()
    {
        // Initialize all our variables to 0
        m_Reorienting = false;
        m_Destabilizing = false;
        m_Grounded = false;
        m_Jumping = false;
        m_Grinding = false;
        m_GrindingCanRotate = false;
        m_InputMode = true;

        m_CurrentSpeed = 0f;
        m_RotationSpeed = 0f;
        m_GroundedContacts = 0;
        m_NextAccelerate = Time.time;

        m_CurrentComboScore = 0;
        m_TotalScore = 0;

        // Initialize Jump rotation to starting position
        m_JumpRotation = transform.rotation;

        m_Rigidbody.maxAngularVelocity = m_MaxAngularV;
        m_Rigidbody.centerOfMass = m_CenterOfMass;

        m_BoostTrail.SetActive(false);

        m_LastLandSFX = Time.time;
        m_AudSrc = GameObject.Find("Audsrc").GetComponent<AudsrcMoving>();
        m_AudSrc_NL = GameObject.Find("Audsrc_noloop").GetComponent<AudsrcNoloop>();

    }


    void FixedUpdate()
    {
        bool wasAirborn = !m_Grounded;
        CheckGrounded();

        // Add Score
        if (!m_Grounded || m_Grinding)
        {
            float score = m_ScoreScalar;
            if (m_Grinding)
            {
                score *= m_ScoreGrindScalar;
            }
            else
            {
                score *= (m_Rigidbody.angularVelocity.magnitude+1) * m_ScoreSpinScalar;
            }
            m_CurrentComboScore += (int)score;
            // Only display if above a certain threshold
            if (m_CurrentComboScore >= m_MinScoreThreshold)
            {
                m_ScoreManager.latestScoreText.text = m_CurrentComboScore.ToString();
            }
        }

        // If grounded, apply constant downard force
        if (m_Grounded && !m_Destabilizing)
        {
            m_ConstantForce.relativeForce = new Vector3(m_ConstantForce.relativeForce.x, m_DownwardForce, m_ConstantForce.relativeForce.z);
        }
        else
        {
            m_ConstantForce.relativeForce = new Vector3(m_ConstantForce.relativeForce.x, 0, m_ConstantForce.relativeForce.z);
        }

        // If we just landed:
        if (wasAirborn && m_Grounded)
        {
            // Stop board from continuing to rotate
            m_Rigidbody.angularVelocity = Vector3.zero;

            // If we just landed from an inputted jump, snap to rotation based on recorded value
            if (m_Jumping)
            {
                Vector3 currentEulers = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentEulers.x, m_JumpRotation.eulerAngles.y, currentEulers.z);
                // Mark that we have landed
                m_Jumping = false;
            }
            if (!m_Grinding) {
                m_AudSrc.PlayMoveGround();
            }
            // Play landing sound effect
            if (Time.time - m_LastLandSFX > m_SFXcooldown)
            {
                m_AudSrc_NL.PlayLand();
                m_LastLandSFX = Time.time;
            }

            // End Combo
            if (m_CurrentComboScore >= m_MinScoreThreshold)
            {
                m_TotalScore += m_CurrentComboScore;
                m_ScoreManager.highScoreText.text = m_TotalScore.ToString();
                m_ScoreManager.PlaySound();
            }
            m_CurrentComboScore = 0;
        }
        // If we launched off of something
        if (!wasAirborn && !m_Grounded && !m_Jumping)
        {
            //m_AudSrc_NL.PlayJump();
            m_AudSrc.PlayDestabilize();
        }
        // Accelerate if grounded
        if (m_Grounded && !m_Destabilizing && Time.time >= m_NextAccelerate && m_Rigidbody.velocity.magnitude < m_MaxSpeed)
        {
            m_Rigidbody.AddForce(m_CurrentSpeed * transform.forward * m_SpeedScalar);
            m_NextAccelerate = Time.time + m_AccelerateInterval;
        }

        // Check if we are destabilizing. If so, pivot midair
        if (m_Destabilizing)
        {
            if (m_Grinding)
            {
                if (m_GrindingCanRotate)
                {
                    m_BoardMesh.transform.Rotate(Vector3.up * m_GrindPivotScalar * m_RotationSpeed);
                }
            }
            else
            {

                if (m_Grounded)
                {
                    m_ConstantForce.relativeTorque = Vector3.right * m_ConstantForceScalar;
                    m_Rigidbody.AddTorque(transform.up * m_RotationSpeed * m_PivotTorqueScalar);
                }
                else
                {
                    m_Rigidbody.AddTorque(transform.right * m_CurrentSpeed * m_PivotTorqueScalar * m_ForwardPivotScalar);
                    //transform.Rotate(transform.right * m_CurrentSpeed * m_PivotRotateScalar * m_ForwardPivotScalar, Space.World);
                    m_Rigidbody.AddTorque(transform.up * m_RotationSpeed * m_PivotTorqueScalar);
                    //transform.Rotate(transform.up * m_RotationSpeed * m_PivotRotateScalar, Space.World);
                }
            }
        }
        else if (!m_Destabilizing && m_Grounded)
        {

            m_ConstantForce.relativeTorque = Vector3.zero;
        }

        // check if we need to reorient again but less agressively
        float xFabs = Math.Abs(transform.localRotation.x);
        float zFabs = Math.Abs(transform.localRotation.z);
        bool flipped = !m_Destabilizing && (xFabs >= 170 && xFabs <= 350 || zFabs >= 170 && zFabs <= 350);
        // if we need to reorient, continue flipping
        if (m_Reorienting || flipped)
        {
            Debug.Log("Reorienting");
            m_Reorienting = true;
            // Slerp towards 0 x z
            Quaternion targetRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * m_CorrectionFactor);

            // Check if we are reoriented
            if (Math.Abs(transform.rotation.eulerAngles.x) < 45 && Math.Abs(transform.rotation.eulerAngles.z) < 45)
            {
                m_Reorienting = false;
                Debug.Log("Reoriented");
            }
        }

        // If not grinding, set the pitch based on the skateboard's velocity / max speed
        if (!m_Grinding) {
            m_AudSrc.ModifyMovementSound(m_Rigidbody.velocity.magnitude / m_MaxSpeed);
        }
        else {
            m_AudSrc.ModifyMovementSound(1.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("ground"))
        {
            m_GroundedContacts++;
            m_Grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("ground"))
        {
            m_GroundedContacts--;
            m_Grounded = m_GroundedContacts > 0;
        }
    }

    // Check each wheel to see if its currently grounded
    // Update grounded status and count appropriately
    private void CheckGrounded()
    {
        // Reset Grounded Status
        m_Grounded = false;
        // Check each wheel
        foreach (Wheel w in m_Wheels)
        {
            if (w.IsGrounded())
            {
                m_Grounded = true;
            }
        }
        m_Grounded |= m_GroundedContacts > 0;
    }

    // Set current forward momentum based on input if movement not locked
    void OnAccelerate(InputValue value)
    {
        CheckGrounded();

        m_CurrentSpeed = value.Get<float>();

        if (m_Grounded && Time.time >= m_NextAccelerate && m_Rigidbody.velocity.magnitude < m_MaxSpeed)
        {
            m_Rigidbody.AddForce(m_CurrentSpeed * transform.forward * m_SpeedScalar);
            m_NextAccelerate = Time.time + m_AccelerateInterval;
        }
    }
    // set rotation speed
    void OnTurn(InputValue value)
    {
        m_RotationSpeed = value.Get<float>();
        // make each wheel steer
        foreach (Wheel w in m_Wheels)
        {
            w.Steer(m_RotationScalar * m_RotationSpeed);
        }
        
    }
    // add an upwards force
    void OnJump()
    {
        CheckGrounded();
        // if not grounded, can't jump
        if (!m_Grounded)
        {
            return;
        }
        // Record current rotation
        m_JumpRotation = transform.rotation;
        // Make jump
        m_Jumping = true;
        m_Rigidbody.AddForce(Vector3.up * m_JumpStr);
        m_AudSrc_NL.PlayJump();
        m_AudSrc.PlayDestabilize();
    }

    /**
     * Update status of Destabilizing bool
     * attempt to lift nose if currently grounded
     * 
     * Pending: Slam ground on exit? (Not sure if this is actually going to help or hurt)
     */
    void OnDestabilize(InputValue value)
    {
        CheckGrounded();

        m_Destabilizing = value.Get<float>() > 0.5;

        if (m_Grounded)
        {
            if (m_Destabilizing)
            {
                m_Rigidbody.AddTorque(transform.right * m_ManualForce);
            }
            else
            {
                m_Rigidbody.AddTorque(transform.right * m_ManualForce * m_ManualSlamScalar);
            }
        }
        if (!m_Destabilizing)
        {
            m_ConstantForce.relativeTorque = Vector3.zero;
        }
    }

    void OnBoost(InputValue value)
    {
        m_BoostTrail.SetActive(value.Get<float>() > 0);
        m_ConstantForce.relativeForce = new Vector3(m_ConstantForce.relativeForce.x, m_ConstantForce.relativeForce.y, value.Get<float>() * m_BoostScalar);
    }


    private void OnPause(InputValue value)
    {
        if (m_InputMode)
        {
            m_PlayerInput.SwitchCurrentActionMap("Menu");
            Time.timeScale = 0f;
        }
        else
        {
            m_PlayerInput.SwitchCurrentActionMap("PlayerMovement");
            Time.timeScale = 1f;
        }
        m_InputMode = !m_InputMode;
        m_MenuController.OnActivate(value);
    }
    // Both of these call their corresponding function in the menu controller
    void OnNavigate(InputValue value)
    {
        m_MenuController.OnNavigate(value);
    }
    void OnAdjust(InputValue value)
    {
        m_MenuController.OnAdjust(value);
    }
    void OnMenuPress(InputValue value)
    {
        if (!m_MenuController.HandleButtonPress())
        {
            m_InputMode = !m_InputMode;
            m_PlayerInput.SwitchCurrentActionMap("PlayerMovement");
            Time.timeScale = 1f;
        }
    }

    /**
     * Called whenever the board makes contact with a rail
     */
    public void OnGrind(bool canRotate)
    {
        m_Grinding = true;
        m_GrindingCanRotate = canRotate;
        // Debug.Log("STARTED GRIND!");
        m_AudSrc_NL.PlayGrindEnter();
        m_AudSrc.PlayGrinding();
    }
    public void OnGrindEnd()
    {
        m_Grinding = false;
        m_BoardMesh.transform.localRotation = Quaternion.Euler(Vector3.zero);
        // Debug.Log("STOPPED GRIND!");
        m_AudSrc_NL.PlayGrindExit();
        m_AudSrc.PlayDestabilize();
    }
    public bool IsGrinding()
    {
        return m_Grinding;
    }
}
