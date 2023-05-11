using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    const int ChunkSizeConst = 16;
    const int ChunkHeightConst = 42;
    const int MaxGroundHeightConst = 20;
    const int MinGroundHeightConst = 5;

    public CubeEnum[,,] Body;

    public static int ChunkSize { get => ChunkSizeConst; }
    public static int ChunkHeight { get => ChunkHeightConst; }
    public static int MinGroundHeight { get => MinGroundHeightConst; }
    public static int MaxGroundHeight { get => MaxGroundHeightConst; }

    public ChunkData(CubeEnum[,,] body) { Body = body; }
    public ChunkData() { Body = new CubeEnum[ChunkSizeConst, ChunkHeightConst, ChunkSizeConst]; }


    public void FillWith(CubeEnum cubeEnum)
    {
        for (int x = 0; x < ChunkSizeConst; x++)
        {
            for (int y = 0; y < ChunkHeightConst; y++)
            {
                for (int z = 0; z < ChunkSizeConst; z++)
                {
                    Body[x, y, z] = cubeEnum;
                }
            }
        }
    }

    public static ChunkData GeneratePerlinChunk(int offsetX, int offsetZ)
    {
        ChunkData chunkData = new ChunkData();

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < MaxGroundHeight; y++)
            {
                for (int z = 0; z < ChunkSizeConst; z++)
                {
                    // generates the groundheight for speciffic x,z coordinate
                    float perlinX = (float)x / ChunkSizeConst + offsetX;
                    float perlinZ = (float)z / ChunkSizeConst + offsetZ;
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
