using UnityEngine;

public class HitscanGunClass : GunClass
{
    protected override void Awake()
    {
        isAttacking = false;
        isActive = false;
        currentLevel = 1;
        currentAttackCooldown = 0f;
        damageMultiplier = 1f;
    }

    public override void Attack()
    {
        Ray ray = new Ray(gunModel.transform.position, gunModel.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, gunSO.range, gunSO.collisionLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Enemy"))
                hit.collider.gameObject.GetComponent<EnemyClass>().TakeDamage(gunSO.damage, null);
        }
    }
}
