using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StructureData
{
    public string name;
    public int prefabID;

    public bool isHousing;
    public bool isWarehouse;

    public int hp;

    public Vector3 position;
    public Quaternion rotation;
}

