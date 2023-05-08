using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> Chunks = new Dictionary<Vector3Int, Chunk>();

    [SerializeField] GameObject ChunkPrefab;

    void Start()
    {
        ChunkData cdRock = ChunkData.Rock();
        var rock = Instantiate(ChunkPrefab, parent: transform);
        rock.transform.localPosition = new Vector3(0, 0, 0 * ChunkData.ChunkSize);
        SpawnChunk(rock, cdRock);

        ChunkData cdGrass = ChunkData.Grass();
        var grass = Instantiate(ChunkPrefab, parent: transform);
        grass.transform.localPosition = new Vector3(0, 0, 1 * ChunkData.ChunkSize);
        SpawnChunk(grass, cdGrass);

        ChunkData cdEmpty = ChunkData.Empty();
        var empty = Instantiate(ChunkPrefab, parent: transform);
        empty.transform.localPosition = new Vector3(0, 0, 2 * ChunkData.ChunkSize);
        SpawnChunk(empty, cdEmpty);

        ChunkData cdSnow = ChunkData.Smow();
        var snow = Instantiate(ChunkPrefab, parent: transform);
        snow.transform.localPosition = new Vector3(0, 0, 3 * ChunkData.ChunkSize);
        SpawnChunk(snow, cdSnow);
    }

    void SpawnChunk(GameObject Chunk, ChunkData cd)
    {
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkData.ChunkSize; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    var prefab = CubePrefabManager.Instance.GetCubePrefab(cd.Body[x, y, z]);
                    if (prefab != null)
                    {
                        var go = Instantiate(prefab, parent: Chunk.transform);
                        go.transform.localPosition = new Vector3(x, y, z);
                    }
                }
            }
        }
    }
}
