using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    // [SerializeField] private GameObject[] structurePrefab;

    [SerializeField] private int day = 0;
    public int Day { get { return day; } set { day = value; } }
    [SerializeField] private float dayTimer = 0f;
    [SerializeField] private float secondsPerDat = 5f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        if (Settings.loadingGame == true)
        {
            Debug.Log("Loading Mode");
            SpawnAllUnits();
            SpawnAllStructures();
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        CheckTimeForDay();
    }

    private void CheckTimeForDay()
    {
        dayTimer += Time.deltaTime;
        if (dayTimer >= secondsPerDat)
        {
            dayTimer = 0f;
            day++;
            MainUI.instance.UpdateDayText();
            TechManager.instance.CheckAllResearch();
            MainUI.instance.UpdateTechBtns();
        }
    }

    /*
        private void SpawnAllUnits()
        {
            List<UnitData> tempUnits = new List<UnitData>(SaveManager.instance.saveWorkers);
            SaveManager.instance.saveWorkers.Clear();

            foreach (UnitData data in tempUnits)
            {
                GameObject workerObj = Instantiate(LaborMarket.instance.WorkerPrefab, data.position, data.rotation);
                Worker w = workerObj.GetComponent<Worker>();
                w.HP = data.hp;
                w.State = data.state;
            }
        }
        private void SpawnAllStructures()
        {
            List<StructureData> tempStructures = new List<StructureData>(SaveManager.instance.saveStructures);
            SaveManager.instance.saveStructures.Clear();
            foreach (StructureData data in tempStructures)
            {
                GameObject structureObj = Instantiate(structurePrefab[data.prefabID], data.position, data.rotation);
                Structure s = structureObj.GetComponent<Structure>();
                s.HP = data.hp;
                s.IsHousing = data.isHousing;
                s.IsWarehouse = data.isWarehouse;
            }
    */
}
