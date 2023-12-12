using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour
{
    public GameObject m_BoardObject;
    public GameObject m_Camera;
    public Rigidbody m_BoardRigidbody;
    public float m_FollowSpeed = 1f;
    public float m_VertFollowSpeed = 0.05f;
    public float m_OrbitSpeed = 1f;
    public float m_OrbitSlerpSpeed = 1f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Follow XZ
        Vector3 targetPosition = m_BoardObject.transform.position;
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * m_FollowSpeed);
        // Follow Y slower
        targetPosition = m_BoardObject.transform.position;
        targetPosition = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
        transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * m_VertFollowSpeed);
        // Follow rotation
        //Quaternion targetRotation = Quaternion.Slerp(transform.rotation, m_BoardObject.transform.rotation, Time.deltaTime * m_OrbitSpeed);
        Quaternion targetRotation = Quaternion.LookRotation(m_BoardRigidbody.velocity.normalized);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_OrbitSlerpSpeed);

        m_Camera.transform.LookAt(m_BoardObject.transform);
    }
}
