using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
    const int ChunkSizeConst = 16;
    public CubeEnum[,,] Body;

    public static int ChunkSize { get => ChunkSizeConst; }

    public ChunkData(CubeEnum[,,] body) { Body = body; }
    public ChunkData()
    {
        Body = new CubeEnum[ChunkSizeConst, ChunkSizeConst, ChunkSizeConst];
    }


    public void FillWith(CubeEnum cubeEnum)
    {
        for (int x = 0; x < ChunkSizeConst; x++)
        {
            for (int y = 0; y < ChunkSizeConst; y++)
            {
                for (int z = 0; z < ChunkSizeConst; z++)
                {
                    Body[x, y, z] = cubeEnum;
                }
            }
        }
    }

    // Static methods
    public static ChunkData Empty()
    {
        ChunkData cd = new ChunkData();
        cd.FillWith(CubeEnum.empty);
        return cd;
    }
    public static ChunkData Rock()
    {
        ChunkData cd = new ChunkData();
        cd.FillWith(CubeEnum.rock);
        return cd;
    }
    public static ChunkData Grass()
    {
        ChunkData cd = new ChunkData();
        cd.FillWith(CubeEnum.grass);
        return cd;
    }
    public static ChunkData Smow()
    {
        ChunkData cd = new ChunkData();
        cd.FillWith(CubeEnum.snow);
        return cd;
    }

    public static ChunkData PerlinRock(int offsetX, int offsetZ)
    {
        ChunkData cd = new ChunkData();

        for (int x = 0; x < ChunkSizeConst; x++)
        {
            for (int y = 0; y < ChunkSizeConst; y++)
            {
                for (int z = 0; z < ChunkSizeConst; z++)
                {
                    float c1 = (float)x / ChunkSize + offsetX;
                    float c2 = (float)z / ChunkSize + offsetZ;

                    if (Mathf.PerlinNoise(c1, c2) * ChunkSize > y)
                        cd.Body[x, y, z] = CubeEnum.rock;
                }
            }
        }

        return cd;
    }
}
