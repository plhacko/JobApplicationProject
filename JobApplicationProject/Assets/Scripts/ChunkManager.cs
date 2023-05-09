using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ChunkManager : MonoBehaviour
{
    Dictionary<Vector2Int, GameObject> Chunks = new Dictionary<Vector2Int, GameObject>();
    List<Vector2Int> ActiveChunks = new List<Vector2Int>();

    [SerializeField] GameObject ChunkPrefab;
    [SerializeField] Player Player;

    CubeEnum GetBlockTypeAt(Vector3Int v)
    {

        return CubeEnum.empty;
    }

    private void Update()
    {
        // gets player position in the chunk
        int playerX = (int)Math.Floor(Player.transform.position.x / ChunkData.ChunkSize);
        int playerZ = (int)Math.Floor(Player.transform.position.z / ChunkData.ChunkSize);
        // debug text // TODO: rm
        GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>().text = $"{playerX} : {playerZ}";


        // list of chunks, that should be vidible
        List<Vector2Int> newActiveChunks = new List<Vector2Int>();
        const int size = 5;
        for (int x = -size; x <= size; x++)
        {
            for (int z = -size; z <= size; z++)
            { newActiveChunks.Add(new Vector2Int(playerX + x, playerZ + z)); }
        }

        // removes all inactive chunks
        foreach (Vector2Int c in ActiveChunks)
        {
            if (!newActiveChunks.Contains(c) && Chunks.ContainsKey(c))
            {
                var _go = Chunks[c];
                Chunks.Remove(c);
                StartCoroutine(DestroyChunk(_go));
            }
        }

        // generates chunk data and paints the chunk
        foreach (Vector2Int v in newActiveChunks)
        {
            if (!Chunks.ContainsKey(v))
            {
                Vector3Int offset = new Vector3Int(v.x, 0, v.y);
                ChunkData cdRock = ChunkData.GeneratePerlinChunk(offset.x, offset.z);

                Chunks[v] = InstantiateChunk(cdRock, offset);
            }
        }

        ActiveChunks = newActiveChunks;
    }

    GameObject InstantiateChunk(ChunkData chunkData, Vector3Int offset)
    {
        var chunk = Instantiate(ChunkPrefab, parent: transform);
        chunk.transform.localPosition = offset * ChunkData.ChunkSize;
        StartCoroutine(DrawChunk(chunk, chunkData));
        return chunk;
    }
    // destroys each visible cube
    // using corutine we can destroy the chunks gradualy (we don't influnce the performence as much)
    IEnumerator DestroyChunk(GameObject Chunk)
    {
        int i = 0;
        foreach (Transform child in Chunk.transform)
        {
            i++;
            Destroy(child.gameObject);

            if (i % ChunkData.ChunkSize * 2 == 0)
                yield return null;
        }
        Destroy(Chunk);
    }
    // instantiates each visible cube
    // using corutine we can instantiate the chunks gradualy (we don't influnce the performence as much)
    IEnumerator DrawChunk(GameObject chunk, ChunkData cd)
    {
        yield return null;
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkData.ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    var prefab = CubePrefabManager.Instance.GetCubePrefab(cd.Body[x, y, z]);
                    if (prefab != null && isVisibleCheck(new Vector3Int(x, y, z)))
                    {
                        if (chunk != null)
                        {
                            var go = Instantiate(prefab, parent: chunk.transform);
                            go.transform.localPosition = new Vector3(x, y, z);
                        }
                    }
                }
            }
            yield return null;
        }

        bool isVisibleCheck(Vector3Int pos)
        {
            Vector3Int[] visibleVectors = new Vector3Int[]
            {new Vector3Int(1,0,0),new Vector3Int(0,1,0), new Vector3Int(0,0,1),
            new Vector3Int(-1,0,0),new Vector3Int(0,-1,0), new Vector3Int(0,0,-1) };

            foreach (var v in visibleVectors)
            {
                var _ = pos + v;

                // rangeCheck
                int s = ChunkData.ChunkSize;
                if (_.x < 0 || _.y < 0 || _.z < 0
                    || _.x >= s || _.y >= s || _.z >= s)
                    continue; // TODO: fix bug: cubes that are on the edge of the chunk may be missing

                if (cd.Body[_.x, _.y, _.z] == CubeEnum.empty)
                    return true;
            }
            return false;
        }
    }
}
