using System.Collections.Generic;
using UnityEngine;

public abstract class GunClass : MonoBehaviour
{
    public GunSO gunSO;
    public GameObject gunModel;
    public Sprite gunIcon;
    //public DefenseDetectionRangeScript detectionRange; deteccion melee o bulletPrefab
    protected bool isAttacking, isActive;
    protected int currentLevel;
    protected float currentAttackCooldown, damageMultiplier;
    //[SerializeField] protected GameObject defenseModel; gunSpawn municion

    protected virtual void Awake()
    {
        isAttacking = false;
        currentLevel = 1;
        currentAttackCooldown = gunSO.attackCooldown;
        damageMultiplier = 1f;
        OnDeselect();
    }

    protected virtual void Update()
    {
        if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown <= 0 && isAttacking)
            {
                Attack();
                currentAttackCooldown = gunSO.attackCooldown;
            }
        }
        if (!isActive) return;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        gunModel.transform.LookAt(ray.GetPoint(50f));
    }

    public virtual void OnSelect()
    {
        gunModel.SetActive(true);
        isActive = true;
    }

    public virtual void OnDeselect()
    {
        gunModel.SetActive(false);
        isAttacking = false;
        isActive = false;
    }

    public virtual void OnAttackStart()
    {
        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = gunSO.attackCooldown;
        }
        isAttacking = true;
    }

    public virtual void OnAttackEnd()
    {
        isAttacking = false;
    }

    public abstract void Attack();
}
