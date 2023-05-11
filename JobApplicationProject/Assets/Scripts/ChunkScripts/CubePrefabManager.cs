using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePrefabManager : Singleton<CubePrefabManager>
{
    [SerializeField] GameObject ErrorCubePrefab;
    [SerializeField] float DefaultMiningTime = 10f;

    [Tooltip("Coresponds to the CubeEnums")]
    [SerializeField] CubePrefabTupplle[] CubePrefabs;
    public GameObject GetCubePrefab(CubeEnum c)
    {
        if (CubePrefabs.Length > (int)c)
            return CubePrefabs[(int)c].Prefab;
        else
            return ErrorCubePrefab;
    }

    public float GetRequiredMiningTime(CubeEnum c)
    {
        if (CubePrefabs.Length > (int)c)
            return CubePrefabs[(int)c].MineTime;
        else
            return DefaultMiningTime;
    }

}
[Serializable]
struct CubePrefabTupplle
{
    public GameObject Prefab;
    public float MineTime;    
}

