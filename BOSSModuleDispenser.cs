using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSDispenser : MonoBehaviour
{
    [Header("Only add boss prefabs")]
    public GameObject[] bossList;

    public GameObject GetBossPrefab(int index)
    {
        return bossList[index];
    }
}
