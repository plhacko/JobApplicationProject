using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePrefabManager : UnityEngine.Singleton<CubePrefabManager>
{
    [SerializeField] GameObject ErrorCubePrefab;
    [Tooltip("Coresponds to the CubeEnums")]
    [SerializeField] GameObject[] CubePrefabs;

    public GameObject GetCubePrefab(CubeEnum c)
    {
        if (CubePrefabs.Length > (int)c)
            return CubePrefabs[(int)c];
        else
            return ErrorCubePrefab;
    }
}

