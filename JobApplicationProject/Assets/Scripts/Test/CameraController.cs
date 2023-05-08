// Generated by ChatGPT

using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Define camera movement speed and rotation speed
    public float moveSpeed = 10.0f;
    public float rotateSpeed = 100.0f;

    // Define camera movement axes
    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode moveBackwardKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode moveUpKey = KeyCode.Space;
    public KeyCode moveDownKey = KeyCode.LeftShift;

    // Define camera rotation axes
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode rotateUpKey = KeyCode.R;
    public KeyCode rotateDownKey = KeyCode.F;

    void Update()
    {
        // Get the camera's current position and rotation
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        // Move the camera based on input keys
        if (Input.GetKey(moveForwardKey)) position += transform.forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(moveBackwardKey)) position -= transform.forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(moveLeftKey)) position -= transform.right * moveSpeed * Time.deltaTime;
        if (Input.GetKey(moveRightKey)) position += transform.right * moveSpeed * Time.deltaTime;
        if (Input.GetKey(moveUpKey)) position += transform.up * moveSpeed * Time.deltaTime;
        if (Input.GetKey(moveDownKey)) position -= transform.up * moveSpeed * Time.deltaTime;

        // Rotate the camera based on input keys
        if (Input.GetKey(rotateLeftKey)) rotation *= Quaternion.Euler(0, -rotateSpeed * Time.deltaTime, 0);
        if (Input.GetKey(rotateRightKey)) rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0);
        if (Input.GetKey(rotateUpKey)) rotation *= Quaternion.Euler(-rotateSpeed * Time.deltaTime, 0, 0);
        if (Input.GetKey(rotateDownKey)) rotation *= Quaternion.Euler(rotateSpeed * Time.deltaTime, 0, 0);

        // Apply the new position and rotation to the camera
        transform.position = position;
        transform.rotation = rotation;
    }
}