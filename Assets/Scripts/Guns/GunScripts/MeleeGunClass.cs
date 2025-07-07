using UnityEngine;

public class MeleeGunClass : GunClass
{
    //public DefenseDetectionRangeScript detectionRange; deteccion melee o bulletPrefab
    //[SerializeField] protected GameObject defenseModel; gunSpawn municion

    protected override void Awake()
    {
        isAttacking = false;
        currentLevel = 1;
        currentAttackCooldown = defenseSO.attackCooldown;
        damageMultiplier = 1f;
    }

    public override void OnSelect()
    {

    }

    public override void OnDeselect()
    {
        isAttacking = false;
    }

    public override void OnAttackStart()
    {
        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = defenseSO.attackCooldown;
        }
        isAttacking = true;
    }

    public override void OnAttackEnd()
    {
        isAttacking = false;
    }

    public override void Attack()
    {

    }
}
