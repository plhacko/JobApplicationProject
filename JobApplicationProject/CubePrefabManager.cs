using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubePrefabManager : Singleton<MonoBehaviour>
{
    // TODO: make into a tupple, that is visible from the inspector
    [SerializeField] GameObject[] CubePrefabs;

    GameObject GetPrefab()
}
