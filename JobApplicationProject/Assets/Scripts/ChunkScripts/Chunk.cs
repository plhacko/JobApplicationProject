using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

class Chunk : MonoBehaviour
{
    public bool WasModified { get; private set; } = false;
    ChunkData ChunkData = new ChunkData();
    Vector3Int ChunkPosition;
    Dictionary<Vector3Int, GameObject> VisibleCubesDict = new Dictionary<Vector3Int, GameObject>();
    CubePrefabManager CubePrefabManager;

    public static Vector3Int[] AllVisibleVectors = new Vector3Int[]{
            new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(0,0,1),
            new Vector3Int(-1,0,0), new Vector3Int(0,-1,0), new Vector3Int(0,0,-1) };


    void Start()
    {
        CubePrefabManager = CubePrefabManager.Instance;
    }

    public void Initialize(ChunkData chunkData, Vector3Int chunkPosition)
    {
        transform.localPosition = chunkPosition * ChunkData.ChunkSize;

        ChunkData = chunkData;
        ChunkPosition = chunkPosition;
    }
    public CubeEnum GetCubeTypeAt(Vector3Int cubePositionInChunk)
    {
        return ChunkData.Body[cubePositionInChunk.x, cubePositionInChunk.y, cubePositionInChunk.z];
    }

    // changes the CubeEnum type at position
    // if needed destroys its visible part or instantiates it
    // is not reponsible for updating it's surroundings
    public void SetCubeTypeAt(Vector3Int cubePositionInChunk, CubeEnum cubeEnum)
    {
        ChunkData.Body[cubePositionInChunk.x, cubePositionInChunk.y, cubePositionInChunk.z] = cubeEnum;
        WasModified = true;

        if (VisibleCubesDict.ContainsKey(cubePositionInChunk))
        {
            Destroy(VisibleCubesDict[cubePositionInChunk]);
            VisibleCubesDict.Remove(cubePositionInChunk);
        }
        InstanciateCubeAt(cubePositionInChunk);

    }

    // instantiates each visible cube
    // using corutine we can instantiate the chunks gradualy (we don't influnce the performence as much)
    public void DrawChunk()
    {
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkData.ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    if (IsVisibleCheck(new Vector3Int(x, y, z)))
                        InstanciateCubeAt(new Vector3Int(x, y, z));
                }
            }
        }
    }

    bool IsVisibleCheck(Vector3Int cubePositionInChunk)
    {
        foreach (var v in AllVisibleVectors)
        {
            // range check
            Vector3Int neighbourPositionInChuk = cubePositionInChunk + v + ChunkPosition * ChunkData.ChunkSize;
            if (ChunkManager.Instance.GetCubeTypeAt(neighbourPositionInChuk) == CubeEnum.empty)
                return true;
        }
        return false;
    }

    void InstanciateCubeAt(Vector3Int cubePositionInChunk)
    {
        var p = cubePositionInChunk;

        var prefab = CubePrefabManager.Instance.GetCubePrefab(ChunkData.Body[p.x, p.y, p.z]);
        if (prefab == null) { return; }

        var cube = Instantiate(prefab, parent: transform);
        VisibleCubesDict.Add(new Vector3Int(p.x, p.y, p.z), cube);
        cube.transform.localPosition = new Vector3(p.x, p.y, p.z);
    }

    // for specified position instantiates or destroys a cube based on its visibility
    public void UpdateVisibilityAt(Vector3Int cubePositionInChunk)
    {
        if (!VisibleCubesDict.ContainsKey(cubePositionInChunk))
        {
            if (IsVisibleCheck(cubePositionInChunk))
                InstanciateCubeAt(cubePositionInChunk);
        }
        else if (!IsVisibleCheck(cubePositionInChunk))
        {
            Destroy(VisibleCubesDict[cubePositionInChunk]);
            VisibleCubesDict.Remove(cubePositionInChunk);
        }
    }
}

