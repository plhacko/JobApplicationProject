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
        {
            var v = hitObject.point - hitObject.transform.position;
            float max = Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
            v.x = Mathf.Abs(v.x) >= max ? v.x : 0;
            v.y = Mathf.Abs(v.y) >= max ? v.y : 0;
            v.z = Mathf.Abs(v.z) >= max ? v.z : 0;
            v.Normalize();
            Vector3Int p = new Vector3Int((int)v.x, (int)v.y, (int)v.z);

            text.text = ($"{hitObject.point} \n {hitObject.transform.position} \n {hitObject.transform.position + p}");
        }
        else
            text.text = "";

        return hitObject;
    }




}
