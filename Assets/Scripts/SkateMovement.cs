using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class SkateMovement : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public GameObject m_BoardMesh;
    public Wheel[] m_Wheels;
    public float m_SpeedScalar = 1000f;
    public float m_RotationScalar = 0.1f;
    public float m_MaxSpeed = 7f;
    public float m_JumpStr = 20f;
    public float m_CorrectionFactor = 10f;
    public float m_MinSpeed = 0f;
    public float m_DownwardForce = 0f;

    private bool m_LockedMovement = true;
    private bool m_LockedRotation = true;
    private bool m_DoingTrick;
    private bool m_Reorienting;
    private float m_CurrentSpeed;
    private float m_RotationSpeed;
    private int m_GroundedContacts;

    void Awake()
    {
        m_Reorienting = false;
        m_DoingTrick = false;

        m_CurrentSpeed = 0f;
        m_RotationSpeed = 0f;
        m_GroundedContacts = 0;
    }


    void FixedUpdate()
    {
        //if (!m_LockedMovement)
        {
            // make each wheel spin
            foreach (Wheel w in m_Wheels)
            {
                w.Accelerate(m_SpeedScalar * m_CurrentSpeed);
            }
        }

        // check if we need to reorient again but less agressively
        bool flipped = Math.Abs(transform.rotation.eulerAngles.x) > 180 || Math.Abs(transform.rotation.eulerAngles.z) > 180;
        // if we need to reorient, continue flipping
        if (m_Reorienting || flipped)
        {
            // Slerp towards 0 x z
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = currentRot;
            Vector3 targetEuler = targetRot.eulerAngles;
            targetEuler.x = 0;
            targetEuler.z = 0;
            targetRot.eulerAngles = targetEuler;

            transform.rotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * m_CorrectionFactor);

            // Check if we are reoriented
            if (Math.Abs(currentRot.eulerAngles.x) < 45 && Math.Abs(currentRot.eulerAngles.z) < 45)
            {
                m_Reorienting = false;
                Debug.Log("Reoriented");
            }
        }

        // Apply downward force on the board to keep it "stickier"(?)
        // Get CoM in world coords
        Vector3 CoM = transform.position;// + m_Rigidbody.centerOfMass;
        m_Rigidbody.AddForceAtPosition(transform.up * -1 * m_DownwardForce, CoM, ForceMode.Acceleration);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            m_GroundedContacts++;
            Debug.Log("Touching: " + m_GroundedContacts);
        }
        if (m_GroundedContacts > 0)
        {
            m_LockedMovement = false;
            m_LockedRotation = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            m_GroundedContacts--;
            Debug.Log("Touching: " + m_GroundedContacts);
        }
        if (m_GroundedContacts < 1)
        {
            m_LockedMovement = true;
            m_LockedRotation = true;
        }
    }

    // Set current forward momentum based on input if movement not locked
    void OnAccelerate(InputValue value)
    {
        m_CurrentSpeed = value.Get<float>();
    }
    // set rotation speed
    void OnTurn(InputValue value)
    {
        m_RotationSpeed = value.Get<float>();
        //if (!m_LockedRotation)
        {
            // make each wheel steer
            foreach (Wheel w in m_Wheels)
            {
                w.Steer(m_RotationScalar * m_RotationSpeed);
            }
        }
    }
    // add an upwards force
    void OnJump()
    {
        // check if any wheels are grounded
        foreach (Wheel w in m_Wheels)
        {
            if (w.IsGrounded())
            {
                Debug.Log("Grounded");
                m_LockedMovement = false;
                break;
            }
        }
        // if not grounded, can't jump
        if (m_LockedMovement)
        {
            //Do a trick if not already doing a flip
            if (!m_DoingTrick)
            {
                Debug.Log("Doing a trick?");
            }
            return;
        }
        // check if we are upside down
        bool flipped = Math.Abs(transform.rotation.eulerAngles.x) > 90 || Math.Abs(transform.rotation.eulerAngles.z) > 90;
        if (flipped)
        {
            Debug.Log("Need to flip");
            m_Reorienting = true;
        }
        m_Rigidbody.AddForce(transform.up * m_JumpStr);
    }
}
