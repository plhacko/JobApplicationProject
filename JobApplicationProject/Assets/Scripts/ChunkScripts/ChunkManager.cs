using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TreeEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ChunkManager : Singleton<ChunkManager>
{
    // active chunks
    Dictionary<Vector2Int, Chunk> ChunkDict = new Dictionary<Vector2Int, Chunk>();
    List<Vector2Int> ActiveChunks = new List<Vector2Int>();

    // queues to build/destory for corutines ChunkBuilderCorutine and ChunkDestroyerCorutine 
    Queue<Chunk> ChunkBuildQueue = new Queue<Chunk>();
    Queue<Chunk> ChunkDestroyQueue = new Queue<Chunk>();

    CubePrefabManager CubePrefabManager;
    [SerializeField] GameObject ChunkPrefab;
    [SerializeField] Player Player;

    [SerializeField] int RenderDistance = 5;

    void Start()
    {
        CubePrefabManager = CubePrefabManager.Instance;

        StartCoroutine(ChunkBuilderCorutine());
        StartCoroutine(ChunkDestroyerCorutine());
    }
    private void Update()
    {
        // gets player position in the chunk
        int playerX = (int)Math.Floor(Player.transform.position.x / ChunkData.ChunkSize);
        int playerZ = (int)Math.Floor(Player.transform.position.z / ChunkData.ChunkSize);
        // debug text // TODO: rm
        GameObject.Find("PlayerChunkPositionText").GetComponent<TextMeshProUGUI>().text = $"{playerX} : {playerZ}";


        // list of chunks, that should be vidible
        List<Vector2Int> newActiveChunks = new List<Vector2Int>();
        for (int x = -RenderDistance; x <= RenderDistance; x++)
        {
            for (int z = -RenderDistance; z <= RenderDistance; z++)
            { newActiveChunks.Add(new Vector2Int(playerX + x, playerZ + z)); }
        }

        // removes all inactive chunks
        foreach (Vector2Int c in ActiveChunks)
        {
            if (!newActiveChunks.Contains(c) && ChunkDict.ContainsKey(c))
            {
                var _go = ChunkDict[c];
                ChunkDict.Remove(c);
                ChunkDestroyQueue.Enqueue(_go);
            }
        }

        // generates chunk data and paints the chunk
        foreach (Vector2Int v in newActiveChunks)
        {
            if (!ChunkDict.ContainsKey(v))
            {
                Vector3Int offset = new Vector3Int(v.x, 0, v.y);
                ChunkData chunkData = ChunkData.GeneratePerlinChunk(offset.x, offset.z);
                Chunk chunk = Instantiate(ChunkPrefab, parent: transform).GetComponent<Chunk>();

                chunk.transform.localPosition = offset * ChunkData.ChunkSize;
                chunk.Initialize(chunkData);
                ChunkDict[v] = chunk;
                ChunkBuildQueue.Enqueue(chunk);
            }
        }

        ActiveChunks = newActiveChunks;
    }


    public void SetCubeAt(Vector3Int blockPosition, CubeEnum cubeEnum)
    {
        Vector2Int chunkPosition = new Vector2Int(
            div(blockPosition.x, ChunkData.ChunkSize),
            div(blockPosition.z, ChunkData.ChunkSize));
        Chunk chunk = ChunkDict[chunkPosition];


        Vector3Int cubePosition = new Vector3Int(
            mod(blockPosition.x, ChunkData.ChunkSize),
            blockPosition.y,
            mod(blockPosition.z, ChunkData.ChunkSize));
        chunk.SetCubeTypeAt(cubePosition, cubeEnum);

        int mod(int x, int m) => (x % m + m) % m;
        int div(int a, int b)
        {
            int res = a / b;
            return (a < 0 && a != b * res) ? res - 1 : res;
        }
    }

    CubeEnum GetBlockTypeAt(Vector3Int v)
    {
        return CubeEnum.empty;
    }

    // corutine that should be called only once on the start
    // every frame draws one chunk from the ChunkBuildQueue
    // (drawing multiple chunks a frame might cause lag)
    IEnumerator ChunkBuilderCorutine()
    {
        while (true)
        {
            if (ChunkBuildQueue.Count > 0)
            {
                Chunk chunk = ChunkBuildQueue.Dequeue();
                if (chunk != null)
                    chunk.DrawChunk();
            }
            yield return null;
        }
    }

    // corutine that should be called only once on the start
    // every frame destroys one chunk from the ChunkDestroyQueue
    // (destroying multiple chunks a frame might cause lag)
    IEnumerator ChunkDestroyerCorutine()
    {
        while (true)
        {
            if (ChunkDestroyQueue.Count > 0)
            {
                Chunk chunk = ChunkDestroyQueue.Dequeue();
                Destroy(chunk.gameObject);
            }
            yield return null;
        }
    }
}
