using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool m_CanSteer = false;
    public bool m_CanDrive = false;
    public float m_TurnOffset = 0f;
    public float m_MaxTurnAngle = 90f;

    private WheelCollider m_WheelCollider;

    // Start is called before the first frame update
    void Start()
    {
        m_WheelCollider = GetComponentInChildren<WheelCollider>();
    }

    /**
     * Accelerates wheel by setting motor torque on the wheel
     * 
     * @param power: float containing motor torque to add
     */
    public void Accelerate(float power)
    {
        if (m_CanDrive)
        {
            m_WheelCollider.motorTorque = power;
        }
        else
        {
            m_WheelCollider.brakeTorque = 0f;
        }
    }

    public void Steer(float steerVal)
    {
        if (m_CanSteer)
        {
            float turnAngle = steerVal * m_MaxTurnAngle + m_TurnOffset;
            m_WheelCollider.steerAngle = turnAngle;
        }
    }

    // returns whether or not the wheel is grounded
    public bool IsGrounded()
    {
        return m_WheelCollider.isGrounded;
    }

    // returns side slip / extremum slip
    public string DebugSlip()
    {
        return "";// m_WheelCollider m_WheelCollider.sidewaysFriction.extremumSlip;
    }
}
