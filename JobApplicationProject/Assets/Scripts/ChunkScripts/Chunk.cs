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
    Dictionary<Vector3Int, GameObject> VisibleCubesDict = new Dictionary<Vector3Int, GameObject>();
    CubePrefabManager CubePrefabManager;

    static Vector3Int[] AllVisibleVectors = new Vector3Int[]{
            new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(0,0,1),
            new Vector3Int(-1,0,0), new Vector3Int(0,-1,0), new Vector3Int(0,0,-1) };


    void Start()
    {
        CubePrefabManager = CubePrefabManager.Instance;
    }

    public void Initialize(ChunkData chunkData)
    {
        ChunkData = chunkData;
    }

    public void SetCubeTypeAt(Vector3Int point, CubeEnum cubeEnum)
    {
        ChunkData.Body[point.x, point.y, point.z] = cubeEnum;
        WasModified = true;

        Destroy(VisibleCubesDict[point]);
        VisibleCubesDict.Remove(point);
        InstanciateCubeAt(point);


        // update the surrounding
        foreach (Vector3Int v in AllVisibleVectors)
        {
            Vector3Int _ = point + v;

            // rangeCheck
            int chunkSize = ChunkData.ChunkSize;
            int chunkHeight = ChunkData.ChunkHeight;
            if (_.x < 0 || _.y < 0 || _.z < 0
                || _.x >= chunkSize || _.y >= chunkHeight || _.z >= chunkSize)
            { continue; }

            if (!VisibleCubesDict.ContainsKey(_))
            {
                if (IsVisibleCheck(_))
                {
                    InstanciateCubeAt(_);
                }
            }
            else
            {
                if (!IsVisibleCheck(_))
                {
                    Destroy(VisibleCubesDict[_]);
                    VisibleCubesDict.Remove(_);
                }

            }
        }
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

    bool IsVisibleCheck(Vector3Int pos)
    {
        foreach (var v in AllVisibleVectors)
        {
            var _ = pos + v;

            // rangeCheck
            int s = ChunkData.ChunkSize;
            if (_.x < 0 || _.y < 0 || _.z < 0
                || _.x >= s || _.y >= s || _.z >= s)
                continue; // TODO: fix bug: cubes that are on the edge of the chunk may be missing

            if (ChunkData.Body[_.x, _.y, _.z] == CubeEnum.empty)
                return true;
        }
        return false;
    }

    void InstanciateCubeAt(Vector3Int p)
    {
        var prefab = CubePrefabManager.Instance.GetCubePrefab(ChunkData.Body[p.x, p.y, p.z]);
        if (prefab == null) { return; }

        var cube = Instantiate(prefab, parent: transform);
        VisibleCubesDict.Add(new Vector3Int(p.x, p.y, p.z), cube);
        cube.transform.localPosition = new Vector3(p.x, p.y, p.z);

    }
}

