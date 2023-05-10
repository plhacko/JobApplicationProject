using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    MouseLook MouseLook;

    void Start()
    {
        MouseLook = GetComponentInChildren<MouseLook>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitObject = MouseLook.GetRaycastHit();
            if (hitObject.transform != null)
                Destroy(hitObject.transform.gameObject);
        }
    }
}
