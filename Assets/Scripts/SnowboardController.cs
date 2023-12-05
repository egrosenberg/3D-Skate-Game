using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowboardController : MonoBehaviour
{
    public float acceleration = 10f;     // Acceleration force
    public float maxSpeed = 40f;         // Maximum speed
    public float turnTorque = 20f;       // Base turning torque
    public float turnDamping = 0.9f;     // Damping to reduce turning sensitivity
    public float friction = 0.96f;       // Ground friction

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();     // Get the Rigidbody component of the GameObject

        // Set up initial Rigidbody properties
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        bool isMovingForward = verticalInput > 0;

        // Calculate the force direction for acceleration
        Vector3 forceDirection = transform.forward * verticalInput * acceleration;

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;  // Limit the speed to the maximum
        }

        rb.velocity *= friction;  // Apply ground friction
        rb.AddForce(forceDirection);  // Apply the force for acceleration

        // Calculate turning torque based on the current forward velocity
        if (isMovingForward)
        {
            float slopeEffect = rb.velocity.magnitude / maxSpeed;  // Adjust for slope effect
            float torqueAmount = horizontalInput * turnTorque * slopeEffect;
            rb.AddTorque(Vector3.up * torqueAmount);

            // Apply damping to reduce turning sensitivity
            rb.angularVelocity *= turnDamping;
        }
    }
}
