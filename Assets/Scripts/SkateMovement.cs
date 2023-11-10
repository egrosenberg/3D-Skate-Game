using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class SkateMovement : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public float m_SpeedScalar = 1f;
    public float m_RotationScalar = 0.1f;
    public float m_MaxSpeed = 7f;
    public float m_JumpStr = 20f;
    public float m_CorrectionFactor = 10f;
    public float m_MinSpeed = 0f;

    private bool m_LockedMovement = true;
    private bool m_LockedRotation = true;
    private bool m_Reorienting;
    private float m_CurrentSpeed;
    private float m_RotationSpeed;
    private float m_OrientTime;
    private int m_GroundedContacts;

    void Awake()
    {
        m_Reorienting = false;

        m_CurrentSpeed = 0f;
        m_RotationSpeed = 0f;
        m_Rigidbody.maxLinearVelocity = m_MaxSpeed;
        m_OrientTime = 0f;
        m_GroundedContacts = 0;
    }


    void FixedUpdate()
    {
        if (!m_LockedMovement)
        {
            m_Rigidbody.AddForce(transform.forward * m_SpeedScalar * m_CurrentSpeed);
        }
        if (!m_LockedRotation)
        {
            m_Rigidbody.AddTorque(transform.up * m_RotationScalar * m_RotationSpeed);
        }

        // if we need to reorient, continue flipping
        if (m_Reorienting)
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

        // if we are below min velocity, set to min velocity
        //if (m_GroundedContacts > 0 && m_Rigidbody.velocity.magnitude < m_MinSpeed)
        //{
        //    m_Rigidbody.velocity = transform.forward * m_MinSpeed;
        //}
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
    }
    // add an upwards force
    void OnJump()
    {
        // if not grounded, can't jump
        if (m_GroundedContacts < 1)
        {
            return;
        }
        // check if we are upside down
        bool flipped = Math.Abs(transform.rotation.eulerAngles.x) > 90 || Math.Abs(transform.rotation.eulerAngles.z) > 90;
        if (flipped)
        {
            Debug.Log("Need to flip");
            m_Reorienting = true;
            m_OrientTime = 0f;
        }
        m_Rigidbody.AddForce(Vector3.up * m_JumpStr);
    }
}
