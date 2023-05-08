using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> Chunks = new Dictionary<Vector3Int, Chunk>();

    [SerializeField] GameObject ChunkPrefab;

    void Start()
    {
        // SimpleChunkTesting();

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                SimplePerlinChunkTesting(new Vector3Int(x, 0, z));
            }
        }
    }

    void SimplePerlinChunkTesting(Vector3Int offset)
    {
        ChunkData cdRock = ChunkData.PerlinRock(offset.x, offset.z);

        var rock = Instantiate(ChunkPrefab, parent: transform);
        rock.transform.localPosition = offset * ChunkData.ChunkSize;
        SpawnChunk(rock, cdRock);
    }


    void SimpleChunkTesting()
    {
        ChunkData cdRock = ChunkData.Rock();
        var rock = Instantiate(ChunkPrefab, parent: transform);
        rock.transform.localPosition = new Vector3(0, 0, 0) * ChunkData.ChunkSize;
        SpawnChunk(rock, cdRock);

        ChunkData cdGrass = ChunkData.Grass();
        var grass = Instantiate(ChunkPrefab, parent: transform);
        grass.transform.localPosition = new Vector3(0, 0, 1) * ChunkData.ChunkSize;
        SpawnChunk(grass, cdGrass);

        ChunkData cdEmpty = ChunkData.Empty();
        var empty = Instantiate(ChunkPrefab, parent: transform);
        empty.transform.localPosition = new Vector3(0, 0, 2) * ChunkData.ChunkSize;
        SpawnChunk(empty, cdEmpty);

        ChunkData cdSnow = ChunkData.Smow();
        var snow = Instantiate(ChunkPrefab, parent: transform);
        snow.transform.localPosition = new Vector3(0, 0, 3) * ChunkData.ChunkSize;
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
                    if (prefab != null && isVisibleCheck(new Vector3Int(x, y, z)))
                    {
                        var go = Instantiate(prefab, parent: Chunk.transform);
                        go.transform.localPosition = new Vector3(x, y, z);
                    }
                }
            }
        }

        bool isVisibleCheck(Vector3Int pos)
        {
            Vector3Int[] visibleVectors = new Vector3Int[]
            {new Vector3Int(1,0,0),new Vector3Int(0,1,0), new Vector3Int(0,0,1),
            new Vector3Int(-1,0,0),new Vector3Int(0,-1,0), new Vector3Int(0,0,-1) };

            foreach (var v in visibleVectors)
            {
                var _ = pos + v;

                try //TODO: redo
                {
                    if (cd.Body[_.x, _.y, _.z] == CubeEnum.empty)
                        return true;
                }
                catch (System.Exception)
                { }
            }
            return false;
        }
    }
}
