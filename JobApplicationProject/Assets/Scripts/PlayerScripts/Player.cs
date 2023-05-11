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
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hitObject = MouseLook.GetRaycastHit();

            if (hitObject.transform != null)
            {
                // gets directional vector from the middle of the cube to the point whre the ray hitted it
                var dirVec = hitObject.point - hitObject.transform.position;

                // isolates the biggest number from this directional vector and normalizes it
                float max = Mathf.Max(Mathf.Abs(dirVec.x), Mathf.Abs(dirVec.y), Mathf.Abs(dirVec.z));
                dirVec.x = Mathf.Abs(dirVec.x) >= max ? dirVec.x : 0;
                dirVec.y = Mathf.Abs(dirVec.y) >= max ? dirVec.y : 0;
                dirVec.z = Mathf.Abs(dirVec.z) >= max ? dirVec.z : 0;
                dirVec.Normalize();

                // gets position of the neighbour cube with the same face
                Vector3Int hitObjectVector = new Vector3Int(
                    (int)(hitObject.transform.position.x + dirVec.x),
                    (int)(hitObject.transform.position.y + dirVec.y),
                    (int)(hitObject.transform.position.z + dirVec.z));

                // instantiates the cube
                if (ChunkManager.Instance.GetCubeTypeAt(hitObjectVector) == CubeEnum.empty)
                    ChunkManager.Instance.SetCubeAt(hitObjectVector, CubeEnum.snow);
            }
        }
    }
}
