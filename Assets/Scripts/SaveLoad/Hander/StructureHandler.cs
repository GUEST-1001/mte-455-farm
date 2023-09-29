using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureHandler : MonoBehaviour
{
    public StructureData structureData;
    private Structure structure;

    void Awake()
    {
        structure = GetComponent<Structure>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateData", 0.0f, 0.5f);
        AddToSaveManager();
    }

    private void AddToSaveManager()
    {
        SaveManager.instance.saveStructures.Add(structureData);
    }

    private void UpdateData()
    {
        structureData.name = structure.name;
        structureData.position = transform.position;
        structureData.rotation = transform.rotation;
        structureData.hp = structure.HP;
        structureData.prefabID = structure.ID;
        structureData.isHousing = structure.IsHousing;
        structureData.isWarehouse = structure.IsWarehouse;
    }

    private void OnDestroy()
    {
        SaveManager.instance.saveStructures.Remove(structureData);
    }

}
