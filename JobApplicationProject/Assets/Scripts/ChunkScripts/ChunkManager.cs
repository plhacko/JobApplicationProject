using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using TreeEditor;
using UnityEditor.Overlays;
using UnityEngine;
using static UnityEditor.Progress;

public class ChunkManager : Singleton<ChunkManager>
{
    // active chunks
    Dictionary<Vector2Int, Chunk> ChunkDict = new Dictionary<Vector2Int, Chunk>();
    List<Vector2Int> ActiveChunks = new List<Vector2Int>();

    // saved modified chunks
    Dictionary<Vector2Int, ChunkData> SavedChunkDict = new Dictionary<Vector2Int, ChunkData>();

    // queues to build/destory for corutines ChunkBuilderCorutine and ChunkDestroyerCorutine 
    Queue<Chunk> ChunkBuildQueue = new Queue<Chunk>();
    Queue<Chunk> ChunkDestroyQueue = new Queue<Chunk>();

    [SerializeField] GameObject ChunkPrefab;
    [SerializeField] Player Player;

    [SerializeField] int RenderDistance = 5;

    public static int ChunkSize { get; } = 16;
    public static int ChunkHeight { get; } = 42;
    public static int MinGroundHeight { get; } = 5;
    public static int MaxGroundHeight { get; } = 20;

    Vector3Int SeedOffset;

    void Start()
    {
        StartCoroutine(ChunkBuilderCorutine());
        StartCoroutine(ChunkDestroyerCorutine());

        SeedOffset = new Vector3Int(
            UnityEngine.Random.Range(0, 10_000),
            0,
            UnityEngine.Random.Range(0, 10_000));

    }
    private void Update()
    {
        // gets player position in the chunk
        int playerX = (int)Math.Floor(Player.transform.position.x / ChunkSize);
        int playerZ = (int)Math.Floor(Player.transform.position.z / ChunkSize);
        // debug text // TODO: rm
        GameObject.Find("PlayerChunkPositionText").GetComponent<TextMeshProUGUI>().text = $"{playerX} : {playerZ}";


        // list of chunks, that should be visible
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
                Chunk chunk = ChunkDict[c];
                ChunkDict.Remove(c);
                ChunkDestroyQueue.Enqueue(chunk);

                if (chunk.WasModified)
                {
                    SavedChunkDict[c] = chunk.ChunkData;
                }
            }
        }

        // generates chunk data and paints the chunk
        foreach (Vector2Int v in newActiveChunks)
        {
            // generate data
            if (!ChunkDict.ContainsKey(v))
            {
                Vector3Int offset = new Vector3Int(v.x, 0, v.y);
                // generates new data only if needed
                ChunkData chunkData = SavedChunkDict.ContainsKey(v) ? SavedChunkDict[v] : ChunkData.GeneratePerlinChunk(offset + SeedOffset);
                Chunk chunk = Instantiate(ChunkPrefab, parent: transform).GetComponent<Chunk>();

                chunk.Initialize(chunkData, offset);
                ChunkDict[v] = chunk;
            }
            // paint if near player
            Chunk c = ChunkDict[v];
            if (!c.IsDrawn && RenderDistance > Vector3Int.Distance(c.ChunkPosition, new Vector3Int(playerX, 0, playerZ)))
            {
                ChunkBuildQueue.Enqueue(c);
                c.IsDrawn = true;
            }
        }

        ActiveChunks = newActiveChunks;
    }


    public void SetCubeAt(Vector3Int cubePosition, CubeEnum cubeEnum)
    {
        Chunk chunk = ChunkDict[GetChunkPosition(cubePosition)];
        Vector3Int cubePositionInChunk = GetCubePositionInChunk(cubePosition);
        chunk.SetCubeTypeAt(cubePositionInChunk, cubeEnum);

        // update the surrounding
        foreach (Vector3Int v in Chunk.AllVisibleVectors)
        {
            Chunk neighbourChunk = ChunkDict[GetChunkPosition(cubePosition + v)];
            Vector3Int neighbourCubePositionInChunk = GetCubePositionInChunk(cubePosition + v);

            neighbourChunk.UpdateVisibilityAt(neighbourCubePositionInChunk);
        }

    }
    // from global position of the cube gets from its chunk the correct type
    public CubeEnum GetCubeTypeAt(Vector3Int cubePosition)
    {
        Vector2Int chunkPosition = GetChunkPosition(cubePosition);

        // range check
        // TODO: for now if we are at the edge of lastly generated chunk we regard as if there is an air
        // that results in creating a full wall of unnecessary gameobjects
        // this might be solved by generating one more layer of chunks, that will not be drawn
        if (!ChunkDict.ContainsKey(chunkPosition))
            return CubeEnum.empty;

        // getting the coresponding chunk
        Chunk chunk = ChunkDict[chunkPosition];

        // range check for the top and bottom vertical layer
        if (cubePosition.y >= ChunkHeight)
            return CubeEnum.empty;
        else if (cubePosition.y < 0)
            return CubeEnum.rock;

        // returns the cube type from corresponding chunk
        Vector3Int cubePositionInChunk = GetCubePositionInChunk(cubePosition);
        return chunk.GetCubeTypeAt(cubePositionInChunk);
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

    // tools to navigate chunks
    static int mod(int x, int m) => (x % m + m) % m;
    static int div(int a, int b)
    {
        int res = a / b;
        return (a < 0 && a != b * res) ? res - 1 : res;
    }
    static Vector2Int GetChunkPosition(Vector3Int cubePosition)
        => new Vector2Int(div(cubePosition.x, ChunkSize), div(cubePosition.z, ChunkSize));

    static Vector3Int GetCubePositionInChunk(Vector3Int cubePosition)
    => new Vector3Int(
            mod(cubePosition.x, ChunkSize),
            cubePosition.y,
            mod(cubePosition.z, ChunkSize));
}
