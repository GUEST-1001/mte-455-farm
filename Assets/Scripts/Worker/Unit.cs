using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;


public enum UnitState
{
    Idle,
    Walk,
    Plow,
    Sow,
    Water,
    Harvest,
    MoveToAttackUnit,
    AttackUnit,
    MoveToAttackBuilding,
    AttackBuilding,
    Mining,
    MoveToMining,
    Deliver,
    MoveToDeliver,
    Die,
    MoveToTree,
    MoveToDeliverTree,
    DeliverTree,
    Treeing
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected int hp = 100;
    public int HP { get { return hp; } set { hp = value; } }

    [SerializeField] protected GameObject targetUnit;
    public GameObject TargetUnit { get { return targetUnit; } set { targetUnit = value; } }

    [SerializeField] protected UnitState state;
    public UnitState State { get { return state; } set { state = value; } }

    protected NavMeshAgent navAgent;
    public NavMeshAgent NavAgent { get { return navAgent; } set { navAgent = value; } }

    protected float distance;

    [SerializeField] protected GameObject targetStructure;
    public GameObject TargetStructure { get { return targetStructure; } set { targetStructure = value; } }

    [SerializeField] protected float detectRange = 50f;
    public float DetectRange { get { return detectRange; } set { detectRange = value; } }

    [SerializeField] protected float attackRange = 2f;
    public float AttackRange { get { return attackRange; } set { attackRange = value; } }

    [SerializeField] protected int attackPower = 5;
    public int AttackPower { get { return attackPower; } set { attackPower = value; } }

    //Timer
    [SerializeField] protected float CheckStateTimer = 0f;
    [SerializeField] protected float CheckStateTimeWait = 0.5f;

    [SerializeField] protected GameObject[] tools;
    [SerializeField] protected GameObject weapon;

    public UnityEvent<UnitState> onStateChange;

    [SerializeField] protected bool alive = true;


    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

        CheckStaffState();
    }

    protected void CheckStaffState()
    {
        CheckStateTimer += Time.deltaTime;

        if (CheckStateTimer >= CheckStateTimeWait)
        {
            CheckStateTimer = 0;
            SwitchStaffState();
        }
    }

    protected void SwitchStaffState()
    {
        switch (state)
        {
            case UnitState.Walk:
                WalkUpdate();
                break;
            case UnitState.MoveToAttackBuilding:
                MoveToAttackBuilding();
                break;
            case UnitState.AttackBuilding:
                AttackBuilding();
                break;
            case UnitState.MoveToAttackUnit:
                MoveToAttackUnit();
                break;
            case UnitState.AttackUnit:
                AttackUnit();
                break;
        }
    }

    protected void WalkUpdate()
    {
        distance = Vector3.Distance(navAgent.destination, transform.position);

        if (distance <= 3f)
        {
            navAgent.isStopped = true;
            SetUnitState(UnitState.Idle);
        }
    }

    public void SetToWalk(Vector3 dest)
    {
        SetUnitState(UnitState.Walk);

        navAgent.SetDestination(dest);
        navAgent.isStopped = false;
    }

    public void SetToWarp(Vector3 dest)
    {
        SetUnitState(UnitState.Idle);

        navAgent.Warp(dest);
        navAgent.isStopped = true;
    }

    protected void MoveToAttackBuilding()
    {
        if (targetStructure == null)
        {
            SetUnitState(UnitState.Idle);
            navAgent.isStopped = true;
            return;
        }
        else
        {
            navAgent.SetDestination(targetStructure.transform.position);
            navAgent.isStopped = false;
        }

        distance = Vector3.Distance(transform.position, targetStructure.transform.position);

        if (distance <= attackRange)
            SetUnitState(UnitState.AttackBuilding);
    }

    protected void AttackBuilding()
    {
        EquipWeapon();
        if (navAgent != null)
            navAgent.isStopped = true;

        if (targetStructure != null)
        {
            LookAt(targetStructure.transform.position);
            Building b = targetStructure.GetComponent<Building>();

            b.TakeDamage(AttackPower);
        }
    }

    protected void DisableWeapon()
    {
        weapon.SetActive(false);
    }

    protected void EquipWeapon()
    {
        weapon.SetActive(true);
    }

    // rotate to face the given position
    protected void LookAt(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void TakeDamage(Unit attacker)
    {
        CheckSelfDefence(attacker);
        hp -= attacker.AttackPower;

        if (hp <= 0)
            Destroy(gameObject);
    }
    public void TakeDamage(Turret attacker)
    {
        CheckSelfDefence(attacker);
        hp -= attacker.ShootDamage;

        if (hp <= 0)
            Destroy(gameObject);
    }

    protected void MoveToAttackUnit()
    {
        if (targetUnit == null)
        {
            SetUnitState(UnitState.Idle);
            navAgent.isStopped = true;
            return;
        }
        else
        {
            navAgent.SetDestination(targetUnit.transform.position);
            navAgent.isStopped = false;
        }

        distance = Vector3.Distance(transform.position, targetUnit.transform.position);

        if (distance <= attackRange)
            SetUnitState(UnitState.AttackUnit);
    }

    protected void AttackUnit()
    {
        EquipWeapon();

        if (navAgent != null)
            navAgent.isStopped = true;

        if (targetUnit != null)
        {
            LookAt(targetUnit.transform.position);

            Unit u = targetUnit.GetComponent<Unit>();
            u.TakeDamage(this);
        }
        else
        {
            SetUnitState(UnitState.Idle);
        }
    }

    public void CheckSelfDefence(Unit u)
    {
        if (u.gameObject != null)
        {
            targetUnit = u.gameObject;
            SetUnitState(UnitState.MoveToAttackUnit);
        }
    }
    public void CheckSelfDefence(Turret u)
    {
        if (u.gameObject != null)
        {
            targetUnit = u.gameObject;
            SetUnitState(UnitState.MoveToAttackUnit);
        }
    }

    public void SetUnitState(UnitState s)
    {
        if (onStateChange != null) //if there is an icon
            onStateChange.Invoke(s);

        state = s;
    }

    public void DestroGameOject()
    {
        Destroy(this);
    }

}
