using UnityEngine;

public class HitscanGunClass : GunClass
{
    public LayerMask enemyLayer;

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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, gunSO.range, enemyLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
                hit.collider.gameObject.GetComponent<EnemyClass>().TakeDamage(gunSO.damage, null);
        }
    }
}
