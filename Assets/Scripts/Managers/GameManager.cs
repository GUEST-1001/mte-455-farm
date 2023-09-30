using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    // [SerializeField] private GameObject[] structurePrefab;

    [SerializeField] private int day = 0;
    public int Day { get { return day; } set { day = value; } }
    [SerializeField] private float dayTimer = 0f;
    [SerializeField] private float secondsPerDat = 5f;
    [SerializeField] private float enemySpqwnTime = 5f;
    [SerializeField] private int maxEnemy = 15;

    [SerializeField] private GameObject EnemyParent;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject plane;
    private Bounds planeBounds;

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
        planeBounds = plane.GetComponent<Renderer>().bounds;
        InvokeRepeating("RondomSpawn", 5f, enemySpqwnTime);
    }

    // Update is called once per frame
    void Update()
    {
        CheckTimeForDay();
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     RondomSpawn();
        // }
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

    private void RondomSpawn()
    {
        if (getChildren(EnemyParent) < maxEnemy)
        {
            float rndX, rndZ;
            rndX = Random.Range(planeBounds.min.x, planeBounds.max.x);
            rndZ = Random.Range(planeBounds.min.z, planeBounds.max.z);
            Vector3 spawnpoint = new Vector3(rndX, 0f, rndZ);
            GameObject enemyPreF = Instantiate(enemy, spawnpoint, Quaternion.identity);
            enemyPreF.transform.SetParent(EnemyParent.transform);
        }
    }

    public int getChildren(GameObject obj)
    {
        int count = 0;

        foreach (Transform child in obj.transform)
            count++;

        return count;
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
