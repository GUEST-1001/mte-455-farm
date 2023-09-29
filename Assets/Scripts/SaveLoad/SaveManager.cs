using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveManager
{
    public string sceneName;

    public int saveMoney;
    public int saveWheat;
    public int saveMelon;
    public int saveCorn;
    public int saveMilk;
    public int saveApple;
    public int saveStone;
    public int saveWood;

    public List<UnitData> saveWorkers = new List<UnitData>();
    public List<StructureData> saveStructures = new List<StructureData>();
    public int saveAvailStaff;

    public static SaveManager instance = new SaveManager();

    //Save GameManager into Savedata
    public void Save()
    {
        sceneName = SceneManager.GetActiveScene().name;
        AddResourceSite();
        AddPlayerResource();
    }

    private void AddResourceSite()
    {

    }

    private void AddPlayerResource()
    {
        saveMoney = Office.instance.Money;
        saveWheat = Office.instance.Wheat;
        saveMelon = Office.instance.Melon;
        saveCorn = Office.instance.Corn;
        saveMilk = Office.instance.Milk;
        saveApple = Office.instance.Apple;
        saveStone = Office.instance.Stone;
        saveWood = Office.instance.Wood;
    }

    public void ClearData()
    {
        instance = new SaveManager();
    }

}
