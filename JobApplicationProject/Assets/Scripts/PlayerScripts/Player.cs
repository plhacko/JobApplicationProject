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
            {
                Vector3Int hitObjectVector = new Vector3Int(
                    (int)hitObject.transform.position.x,
                    (int)hitObject.transform.position.y,
                    (int)hitObject.transform.position.z);

                ChunkManager.Instance.SetCubeAt(hitObjectVector, CubeEnum.empty);
            }
        }
    }
}
