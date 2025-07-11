using System.Collections.Generic;
using UnityEngine;

public abstract class GunClass : MonoBehaviour
{
    public GunSO gunSO;
    public GameObject gunModel;
    //public DefenseDetectionRangeScript detectionRange; deteccion melee o bulletPrefab
    protected bool isAttacking, isActive;
    protected int currentLevel;
    protected float currentAttackCooldown, damageMultiplier;
    //[SerializeField] protected GameObject defenseModel; gunSpawn municion

    protected virtual void Awake()
    {
        isAttacking = false;
        isActive = false;
        currentLevel = 1;
        currentAttackCooldown = 0f;
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
                currentAttackCooldown = gunSO.attackCooldown;
            }
        }
        if (isActive)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, gunSO.range, gunSO.collisionLayer, QueryTriggerInteraction.Ignore))
                gunModel.transform.LookAt(hit.point);
            else
                gunModel.transform.LookAt(ray.GetPoint(gunSO.range));

        }
    }

    public virtual void OnSelect(bool wasAttacking)
    {
        gunModel.SetActive(true);
        isActive = true;
        if (wasAttacking)
            OnAttackStart();
    }

    public virtual void OnDeselect()
    {
        gunModel.SetActive(false);
        isActive = false;
        isAttacking = false;
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
