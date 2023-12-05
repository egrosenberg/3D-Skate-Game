using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;             // The target to follow
    public Vector3 offset = new Vector3(0, 3, -5);  // Offset from the target
    public float smoothSpeed = 10f;       // Smoothness of camera movement
    public float rotationDamping = 5f;    // Speed of camera rotation

    private Transform snowboardTransform;

    private void Start()
    {
        snowboardTransform = player;  // Initialize the snowboardTransform to follow the player.
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the desired camera position based on offset and target position
            Vector3 desiredPosition = player.position + player.rotation * offset;

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Adjust the camera's focus point while maintaining some height
            Vector3 lookAtPosition = snowboardTransform.position + Vector3.up * 1.5f; // Adjust the Y value for camera height
            Quaternion desiredRotation = Quaternion.LookRotation(lookAtPosition - transform.position);

            // Smoothly rotate the camera towards the desired rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationDamping * Time.deltaTime);
        }
    }
}
