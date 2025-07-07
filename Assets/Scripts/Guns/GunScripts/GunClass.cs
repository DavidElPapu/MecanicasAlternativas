using System.Collections.Generic;
using UnityEngine;

public abstract class GunClass : MonoBehaviour
{
    public GunSO defenseSO;
    //public DefenseDetectionRangeScript detectionRange; deteccion melee o bulletPrefab
    protected bool isAttacking;
    protected int currentLevel;
    protected float currentAttackCooldown, damageMultiplier;
    //[SerializeField] protected GameObject defenseModel; gunSpawn municion

    protected virtual void Awake()
    {
        isAttacking = false;
        currentLevel = 1;
        currentAttackCooldown = defenseSO.attackCooldown;
        damageMultiplier = 1f;
    }

    protected virtual void Update()
    {
        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown <= 0 && isAttacking)
            {
                Attack();
                currentAttackCooldown = defenseSO.attackCooldown;
            }
        }
    }

    public virtual void OnSelect()
    {

    }

    public virtual void OnDeselect()
    {
        isAttacking = false;
    }

    public virtual void OnAttackStart()
    {
        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = defenseSO.attackCooldown;
        }
        isAttacking = true;
    }

    public virtual void OnAttackEnd()
    {
        isAttacking = false;
    }

    public abstract void Attack();
}
