using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;

class Chunk : MonoBehaviour
{
    ChunkData ChunkData;
    private void Initialize(ChunkData chunkData)
    {
        ChunkData = chunkData;
    }
}
