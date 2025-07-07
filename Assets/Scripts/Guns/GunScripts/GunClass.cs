using System.Collections.Generic;
using UnityEngine;

public class GunClass : MonoBehaviour
{
    public GunSO defenseSO;
    //public DefenseDetectionRangeScript detectionRange; deteccion melee o bulletPrefab
    protected bool isAttacking;
    protected int currentLevel;
    protected float currentAttackCooldown, damageMultiplier;
    //[SerializeField] protected GameObject defenseModel; gunSpawn municion

    public virtual void Awake()
    {
        isAttacking = false;
    }
}
