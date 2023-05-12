using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
struct CubePrefabTupplle
{
    public CubeEnum CubeEnum;
    public GameObject Prefab;
    public float MineTime;
}

public class CubePrefabManager : Singleton<CubePrefabManager>
{
    [SerializeField] GameObject ErrorCubePrefab;
    [SerializeField] float DefaultMiningTime = 10f;

    [Header("Coresponds to the CubeEnums (CubeEnum is uneditable)")]
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


#if UNITY_EDITOR
    [CustomEditor(typeof(CubePrefabManager))]
    class CubePrefabManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CubePrefabManager cubePrefabManager = (CubePrefabManager)target;

            var AllCubeEnums = (CubeEnum[])Enum.GetValues(typeof(CubeEnum));

            if (cubePrefabManager.CubePrefabs.Length != AllCubeEnums.Length)
            {
                var newCubePrefabs = new CubePrefabTupplle[AllCubeEnums.Length];

                Array.Copy(
                    cubePrefabManager.CubePrefabs, newCubePrefabs,
                    Math.Min(cubePrefabManager.CubePrefabs.Length, newCubePrefabs.Length));

                cubePrefabManager.CubePrefabs = newCubePrefabs;
            }

            for (int i = 0; i < AllCubeEnums.Length; i++)
            {
                cubePrefabManager.CubePrefabs[i].CubeEnum = AllCubeEnums[i];
            }

        }
    }
}
#endif
