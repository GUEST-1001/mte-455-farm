using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public enum Gender
{
    male,
    female,
}

public class Worker : Unit
{

    private int id;
    public int ID { get { return id; } set { id = value; } }

    [SerializeField] private int charSkinID;
    public int CharSkinID { get { return charSkinID; } set { charSkinID = value; } }
    public GameObject[] charSkin;

    [SerializeField] private int charFaceID;
    public int CharFaceID { get { return charFaceID; } set { charFaceID = value; } }
    public Sprite[] charFacePic;

    [SerializeField] private string staffName;
    public string StaffName { get { return staffName; } set { staffName = value; } }

    [SerializeField] private int dailyWage;
    public int DailyWage { get { return dailyWage; } set { dailyWage = value; } }

    [SerializeField] private Gender staffGender = Gender.male;
    public Gender StaffGender { get { return staffGender; } set { staffGender = value; } }

    [SerializeField] private bool hired = false;
    public bool Hired { get { return hired; } set { hired = value; } }

    //Mining
    [SerializeField] private GameObject targetMine;
    public GameObject TargetMine { get { return targetMine; } set { targetMine = value; } }
    [SerializeField] private GameObject targetTree;
    public GameObject TargetTree { get { return targetTree; } set { targetTree = value; } }

    private int maxAmount = 30;

    [SerializeField] private int curAmount;
    public int CurAmount { get { return curAmount; } set { curAmount = value; } }

    //Miner State Timer
    [SerializeField] private float miningTimer = 0f;
    [SerializeField] private float miningTimeWait = 1f;

    //Miner Dig Timer
    [SerializeField] private float timeLastDig;
    [SerializeField] private float digRate = 3f;


    protected override void Update()
    {
        base.Update();

        miningTimer += Time.deltaTime;
        if (miningTimer >= miningTimeWait)
        {
            miningTimer = 0f;
            CheckWorkerState();
        }
        Debug.Log(TargetUnit);
    }



    public void InitiateCharID(int i)
    {
        charSkinID = i;
        charFaceID = i;
    }

    public void SetGender()
    {
        if (charSkinID == 1 || charSkinID == 4)
        {
            staffGender = Gender.female;
        }
    }

    public void ChangeCharSkin()
    {
        for (int i = 0; i < charSkin.Length; i++)
        {
            if (i == charSkinID)
            {
                charSkin[i].SetActive(true);
            }
            else
            {
                charSkin[i].SetActive(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != targetStructure)
        {
            return;
        }

        Farm farm = other.gameObject.GetComponent<Farm>();

        if ((other.tag == "Farm") && (farm != null) && (farm.HP < 100))
        {
            switch (farm.Stage)
            {
                case FarmStage.plowing:
                    SetUnitState(UnitState.Plow);
                    EquipTool(0); //Hoe
                    farm.CheckTimeForWork();
                    break;
                case FarmStage.sowing:
                    SetUnitState(UnitState.Sow);
                    EquipTool(1); //Sack
                    farm.CheckTimeForWork();
                    break;
                case FarmStage.maintaining:
                    SetUnitState(UnitState.Water);
                    EquipTool(2); //Watering Can
                    farm.CheckTimeForWork();
                    break;
                case FarmStage.harvesting:
                    SetUnitState(UnitState.Harvest);
                    DisableAllTools();
                    farm.CheckTimeForWork();
                    break;
            }
        }

        Mines mine = other.gameObject.GetComponent<Mines>();
        if ((other.tag == "Mine") && (mine != null) && (mine.HP < 100))
        {
            LookAt(targetMine.transform.position);
            EquipTool(3);
            SetUnitState(UnitState.Mining);
        }
    }

    public void HideCharSkin()
    {
        foreach (GameObject obj in charSkin)
        {
            obj.SetActive(false);
        }
    }

    public void DisableAllTools()
    {
        for (int i = 0; i < tools.Length; i++)
            tools[i].SetActive(false);
    }

    private void EquipTool(int i)
    {
        DisableAllTools();
        tools[i].SetActive(true);
    }

    #region Resource
    public void StartMining(GameObject mine)
    {
        if (mine == null)
        {
            targetMine = null;
            SetUnitState(UnitState.MoveToDeliver);
            navAgent.SetDestination(targetStructure.transform.position);
        }
        else
        {
            SetUnitState(UnitState.MoveToMining);
            navAgent.SetDestination(mine.transform.position);
        }
        navAgent.isStopped = false;
    }
    public void StartTree(GameObject tree)
    {
        if (tree == null)
        {
            targetTree = null;
            SetUnitState(UnitState.MoveToDeliverTree);
            navAgent.SetDestination(targetStructure.transform.position);
        }
        else
        {
            SetUnitState(UnitState.MoveToTree);
            navAgent.SetDestination(tree.transform.position);
        }
        navAgent.isStopped = false;
    }

    void MoveToMiningUpdate()
    {
        if (targetMine == null)
        {
            GameObject newMine = FindingTarget.CheckForNearestMine(targetStructure.transform.position,
                                                                        100f,
                                                                        "Mine");
            StartMining(newMine);
        }

        DisableAllTools();
        //Equip PickAxe

        if (Vector3.Distance(transform.position, navAgent.destination) <= 1f)
        {
            LookAt(navAgent.destination);
            SetUnitState(UnitState.Mining);
        }
    }

    void MoveToTreeUpdate()
    {
        if (targetMine == null)
        {
            GameObject newTree = FindingTarget.CheckForNearestTree(targetStructure.transform.position,
                                                                        100f,
                                                                        "Tree");
            StartTree(newTree);
        }

        DisableAllTools();
        //Equip PickAxe

        if (Vector3.Distance(transform.position, navAgent.destination) <= 1f)
        {
            LookAt(navAgent.destination);
            SetUnitState(UnitState.Treeing);
        }
    }

    void MiningUpdate()
    {
        Mines mine;
        if (targetMine != null)
            mine = targetMine.GetComponent<Mines>();
        else
        {
            GameObject newMine = FindingTarget.CheckForNearestMine(targetStructure.transform.position,
                                                                        100f,
                                                                        "Mine");
            targetMine = newMine;
            StartMining(newMine);
            return;
        }

        // DisableAllTools();
        //Equip PickAxe

        if (Time.time - timeLastDig > digRate)
        {
            timeLastDig = Time.time;

            if (curAmount < maxAmount)
            {
                mine.Deplete(3);
                curAmount += 3;

                if (curAmount > maxAmount)
                    curAmount = maxAmount;
            }
            else //Move to deliver at HQ
            {
                SetUnitState(UnitState.MoveToDeliver);
                navAgent.SetDestination(targetStructure.transform.position);
                navAgent.isStopped = false;
            }
        }
    }
    void TreeUpdate()
    {
        Mines mine;
        if (targetMine != null)
            mine = targetMine.GetComponent<Mines>();
        else
        {
            GameObject newTree = FindingTarget.CheckForNearestTree(targetStructure.transform.position,
                                                                        100f,
                                                                        "Tree");
            targetMine = newTree;
            StartTree(newTree);
            return;
        }

        // DisableAllTools();
        //Equip PickAxe

        if (Time.time - timeLastDig > digRate)
        {
            timeLastDig = Time.time;

            if (curAmount < maxAmount)
            {
                mine.Deplete(3);
                curAmount += 3;

                if (curAmount > maxAmount)
                    curAmount = maxAmount;
            }
            else //Move to deliver at HQ
            {
                SetUnitState(UnitState.MoveToDeliverTree);
                navAgent.SetDestination(targetStructure.transform.position);
                navAgent.isStopped = false;
            }
        }
    }

    private void MoveToDeliverUpdate()
    {
        if (targetStructure == null)
        {
            SetUnitState(UnitState.Idle);
            return;
        }

        DisableAllTools();
        //Equip Load

        if (Vector3.Distance(transform.position, targetStructure.transform.position) <= 5f)
        {
            SetUnitState(UnitState.Deliver);
            navAgent.isStopped = true;
        }
    }

    private void MoveToDeliverTreeUpdate()
    {
        if (targetStructure == null)
        {
            SetUnitState(UnitState.Idle);
            return;
        }

        DisableAllTools();
        //Equip Load

        if (Vector3.Distance(transform.position, targetStructure.transform.position) <= 5f)
        {
            SetUnitState(UnitState.DeliverTree);
            navAgent.isStopped = true;
        }
    }

    private void DeliverUpdate()
    {
        //This unit stops when there is no target resource to go back and he has nothing to deliver
        if (targetStructure == null)
        {
            SetUnitState(UnitState.Idle);
            return;
        }

        // Deliver the resource to player
        Office.instance.Stone += curAmount;
        curAmount = 0;
        MainUI.instance.UpdateResourceUI();

        // Go back to mining
        if (targetMine != null)
        {
            StartMining(targetMine);
        }
        else
        {
            GameObject newMine = FindingTarget.CheckForNearestMine(targetStructure.transform.position,
                                                                    100f,
                                                                    "Mine");
            if (newMine != null)
                StartMining(newMine);
            else
            {
                targetStructure = null;
                SetUnitState(UnitState.Idle);
                navAgent.isStopped = true;
            }
        }
    }
    private void DeliverTreeUpdate()
    {
        //This unit stops when there is no target resource to go back and he has nothing to deliver
        if (targetStructure == null)
        {
            SetUnitState(UnitState.Idle);
            return;
        }

        // Deliver the resource to player
        Office.instance.Wood += curAmount;
        curAmount = 0;
        MainUI.instance.UpdateResourceUI();

        // Go back to mining
        if (targetTree != null)
        {
            StartTree(targetTree);
        }
        else
        {
            GameObject newTree = FindingTarget.CheckForNearestTree(targetStructure.transform.position,
                                                                    100f,
                                                                    "Tree");
            if (newTree != null)
                StartTree(newTree);
            else
            {
                targetStructure = null;
                SetUnitState(UnitState.Idle);
                navAgent.isStopped = true;
            }
        }
    }

    #endregion

    private void CheckWorkerState()
    {
        switch (state)
        {
            case UnitState.MoveToMining:
                MoveToMiningUpdate();
                break;
            case UnitState.MoveToTree:
                MoveToTreeUpdate();
                break;
            case UnitState.Mining:
                MiningUpdate();
                EquipTool(3);
                break;
            case UnitState.Treeing:
                TreeUpdate();
                EquipWeapon();
                break;
            case UnitState.MoveToDeliver:
                MoveToDeliverUpdate();
                break;
            case UnitState.MoveToDeliverTree:
                MoveToDeliverTreeUpdate();
                break;
            case UnitState.Deliver:
                DeliverUpdate();
                break;
            case UnitState.DeliverTree:
                DeliverTreeUpdate();
                break;
        }
    }
}
