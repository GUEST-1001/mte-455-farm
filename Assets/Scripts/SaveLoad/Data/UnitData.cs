using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string name;

    public int prefabID; //unit ID that the player has in list of all available unit classes
    public int hp;

    public Vector3 position;
    public Quaternion rotation;

    public UnitState state;

}
