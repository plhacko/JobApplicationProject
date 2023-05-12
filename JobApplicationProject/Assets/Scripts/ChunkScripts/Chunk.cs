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
    public bool IsDrawn { get; set; } = false;
    public ChunkData ChunkData { get; private set; } = new ChunkData();
    public Vector3Int ChunkPosition { get; private set; }
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
        transform.localPosition = chunkPosition * ChunkManager.ChunkSize;

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
        // vertical range check (block at height 0 must not me changed)
        if (cubePositionInChunk.y < 1 || cubePositionInChunk.y >= ChunkManager.ChunkHeight)
            return;

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
        for (int x = 0; x < ChunkManager.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkManager.ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkManager.ChunkSize; z++)
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
            Vector3Int neighbourPositionInChuk = cubePositionInChunk + v + ChunkPosition * ChunkManager.ChunkSize;
            if (ChunkManager.Instance.GetCubeTypeAt(neighbourPositionInChuk) == CubeEnum.empty)
                return true;
        }
        return false;
    }

    void InstanciateCubeAt(Vector3Int cubePositionInChunk)
    {
        if (cubePositionInChunk.y < 0 || cubePositionInChunk.y >= ChunkManager.ChunkHeight)
            return;

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

