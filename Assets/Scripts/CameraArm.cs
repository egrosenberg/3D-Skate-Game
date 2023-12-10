using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour
{
    public GameObject m_BoardObject;
    public float m_FollowSpeed = 1f;
    public float m_OrbitSpeed = 1f;


    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, m_BoardObject.transform.position, Time.deltaTime * m_FollowSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_BoardObject.transform.rotation, Time.deltaTime * m_OrbitSpeed);
    }
}
