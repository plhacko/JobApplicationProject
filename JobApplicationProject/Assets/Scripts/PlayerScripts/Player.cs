using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    MouseLook MouseLook;
    CubeEnum BuildingMaterial = CubeEnum.snow;

    float CurrentMiningTime = 0f;
    Vector3Int CurrentMiningTarget;
    float RequiredMiningTime;

    Transform GUICursor;

    void Start()
    {
        GUICursor = GameObject.Find("Cursor").transform;
        MouseLook = GetComponentInChildren<MouseLook>();
    }

    void Update()
    {
        // destroy cubes
        if (Input.GetMouseButton(0))
        { MineCubeAtRaycast(); }
        else { CurrentMiningTime = 0f; }

        // gives some feedback to the player
        GUICursor.localScale = Vector3.one * (1 + CurrentMiningTime);

        // chcecks if player hasents changed the cube to build with
        PickBuildingCube();

        // build cuubes
        if (Input.GetMouseButtonDown(1))
        { PlaceCubeAtRaycast(); }
    }

    private void PlaceCubeAtRaycast()
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
                ChunkManager.Instance.SetCubeAt(hitObjectVector, BuildingMaterial);
        }
    }

    private void PickBuildingCube()
    {
        //change build cubes
        if (Input.GetKeyDown(KeyCode.Alpha1)) { BuildingMaterial = CubeEnum.rock; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { BuildingMaterial = CubeEnum.grass; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { BuildingMaterial = CubeEnum.snow; }

        // TODO: rm : debug
        GameObject.Find("BuildingBlockText").GetComponent<TextMeshProUGUI>().text = $"building block: {BuildingMaterial}";
    }

    private void MineCubeAtRaycast()
    {
        RaycastHit hitObject = MouseLook.GetRaycastHit();
        if (hitObject.transform == null) return;

        Vector3Int newMiningTarget = new Vector3Int(
            (int)hitObject.transform.position.x,
            (int)hitObject.transform.position.y,
            (int)hitObject.transform.position.z);

        // chech for switching targets
        if (CurrentMiningTarget != newMiningTarget)
        {
            CurrentMiningTarget = newMiningTarget;
            CubeEnum newMiningTargetType = ChunkManager.Instance.GetCubeTypeAt(CurrentMiningTarget);
            RequiredMiningTime = CubePrefabManager.Instance.GetRequiredMiningTime(newMiningTargetType);
            CurrentMiningTime = 0f;
        }
        else
        { CurrentMiningTime += Time.deltaTime; }

        // mine (destroy) a cube
        if (RequiredMiningTime <= CurrentMiningTime)
        {
            ChunkManager.Instance.SetCubeAt(CurrentMiningTarget, CubeEnum.empty);
            CurrentMiningTime = 0f;
        }
    }
}
