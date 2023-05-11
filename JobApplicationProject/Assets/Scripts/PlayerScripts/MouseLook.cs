using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float MouseSensitivity = 500f;

    Transform PlayerBody;
    float yRotation;

    // TODO: redo to be settalbe from player
    float MaxDistance = 10f;

    void Start()
    {
        PlayerBody = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // up and down rotation (only camre)
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(yRotation, 0, 0);

        // left and right rotation (the whole player)
        PlayerBody.Rotate(Vector3.up * mouseX);

        // raycast, debug // TODO: rm
        GetRaycastHit();
    }

    public RaycastHit GetRaycastHit()
    {
        Ray CameraRay = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));


        // TODO: rm debug
        var text = GameObject.Find("RaycastObjectText").GetComponent<TextMeshProUGUI>();
        if (Physics.Raycast(CameraRay, out RaycastHit hitObject, maxDistance: MaxDistance))
            text.text = ($"{hitObject.point.ToString()} \n {hitObject.transform.position.ToString()}");
        else
            text.text = "";

        return hitObject;
    }




}
