using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public CubeEnum[,,] Body;

    // helpfull methods that makes the following code shorter
    static int ChunkSize { get => ChunkManager.ChunkSize; }
    static int ChunkHeight { get => ChunkManager.ChunkHeight; }
    static int MinGroundHeight { get => ChunkManager.MinGroundHeight; }
    static int MaxGroundHeight { get => ChunkManager.MaxGroundHeight; }

    public ChunkData(CubeEnum[,,] body) { Body = body; }
    public ChunkData() { Body = new CubeEnum[ChunkSize, ChunkHeight, ChunkSize]; }


    public void FillWith(CubeEnum cubeEnum)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    Body[x, y, z] = cubeEnum;
                }
            }
        }
    }

    public static ChunkData GeneratePerlinChunk(Vector3Int offset)
    {
        ChunkData chunkData = new ChunkData();

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < MaxGroundHeight; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    // generates the groundheight for speciffic x,z coordinate
                    float perlinX = (float)x / ChunkSize+ offset.x;
                    float perlinZ = (float)z / ChunkSize+ offset.z;
                    float perlinGroundHeight = MinGroundHeight + Mathf.PerlinNoise(perlinX, perlinZ) * (MaxGroundHeight - MinGroundHeight);

                    // the top layer is grass or snow (if it is high enough)
                    // the rest is filled with rocks
                    if (perlinGroundHeight > y + 1)
                    {
                        chunkData.Body[x, y, z] = CubeEnum.rock;
                    }
                    else if (perlinGroundHeight > y)
                    {
                        if (y > MaxGroundHeight * 0.7)
                            chunkData.Body[x, y, z] = CubeEnum.snow;
                        else
                            chunkData.Body[x, y, z] = CubeEnum.grass;
                    }
                }
            }
        }
        return chunkData;
    }
}
