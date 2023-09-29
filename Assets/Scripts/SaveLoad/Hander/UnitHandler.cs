using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHandler : MonoBehaviour
{
    public UnitData unitData;
    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateData", 0.0f, 0.5f);

        AddToSaveManager();
    }

    private void AddToSaveManager()
    {
        SaveManager.instance.saveWorkers.Add(unitData);
    }

    private void UpdateData()
    {
        unitData.name = unit.name;
        unitData.position = transform.position;
        unitData.rotation = transform.rotation;
        unitData.hp = unit.HP;
        unitData.state = unit.State;
    }

    private void OnDestroy()
    {
        SaveManager.instance.saveWorkers.Remove(unitData);
    }

}
