using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TreeEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ChunkManager : MonoBehaviour
{
    // active chunks
    Dictionary<Vector2Int, GameObject> Chunks = new Dictionary<Vector2Int, GameObject>();
    List<Vector2Int> ActiveChunks = new List<Vector2Int>();

    // queues to build/destory for corutines ChunkBuilderCorutine and ChunkDestroyerCorutine 
    Queue<Tuple<ChunkData, GameObject>> ChunkBuildQueue = new Queue<Tuple<ChunkData, GameObject>>();
    Queue<GameObject> ChunkDestroyQueue = new Queue<GameObject>();

    CubePrefabManager CubePrefabManager;
    [SerializeField] GameObject ChunkPrefab;
    [SerializeField] Player Player;

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
                ChunkDestroyQueue.Enqueue(_go);
            }
        }

        // generates chunk data and paints the chunk
        foreach (Vector2Int v in newActiveChunks)
        {
            if (!Chunks.ContainsKey(v))
            {
                Vector3Int offset = new Vector3Int(v.x, 0, v.y);
                ChunkData chunkData = ChunkData.GeneratePerlinChunk(offset.x, offset.z);

                var chunk = Instantiate(ChunkPrefab, parent: transform);
                chunk.transform.localPosition = offset * ChunkData.ChunkSize;
                Chunks[v] = chunk;
                ChunkBuildQueue.Enqueue(Tuple.Create(chunkData, chunk));
            }
        }

        ActiveChunks = newActiveChunks;
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
                var data = ChunkBuildQueue.Dequeue();
                ChunkData chunkData = data.Item1;
                GameObject chunk = data.Item2;
                DrawChunk(chunk, chunkData);
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
                GameObject go = ChunkDestroyQueue.Dequeue();
                Destroy(go);
            }
            yield return null;
        }
    }

    // instantiates each visible cube
    // using corutine we can instantiate the chunks gradualy (we don't influnce the performence as much)
    void DrawChunk(GameObject chunk, ChunkData cd)
    {
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < ChunkData.ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    var prefab = CubePrefabManager.GetCubePrefab(cd.Body[x, y, z]);
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
