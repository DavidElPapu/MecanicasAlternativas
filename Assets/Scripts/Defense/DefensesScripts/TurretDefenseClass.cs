using UnityEngine;

public class TurretDefenseClass : DefenseClass
{
    public float attackRange;

    protected override void Update()
    {
        if (isActive)
        {
            if (currentCooldown > 0 && targetsInRange.Count > 0)
            {
                currentCooldown -= Time.deltaTime;
                if (currentCooldown <= 0)
                {
                    currentCooldown = defenseLevels[currentLevel].mainCooldown;
                    TargetFirstEnemy();
                    DoMainAction();
                }
            }
        }
    }

    protected override void DoMainAction()
    {
        if (currentTarget)
        {
            //por ahora solo ataca con raycast y tiene piercing, pero si hay proyectiles, esto se arreglaria
            Vector3 rayDirection = currentTarget.transform.position - transform.position;
            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, rayDirection.normalized, attackRange))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<EnemyClass>().TakeDamage(defenseLevels[currentLevel].damage * damageMultiplier, gameObject);
                }
            }
        }
    }

    protected virtual void TargetFirstEnemy()
    {
        int highestWayPoint = 0;
        float bestDistance = 1000f;
        currentTarget = null;
        foreach (GameObject enemy in targetsInRange)
        {
            if (enemy.GetComponent<EnemyClass>().GetDistanceToBase().wayPoint >= highestWayPoint)
            {
                if (enemy.GetComponent<EnemyClass>().GetDistanceToBase().distance < bestDistance)
                {
                    currentTarget = enemy;
                    highestWayPoint = enemy.GetComponent<EnemyClass>().GetDistanceToBase().wayPoint;
                    bestDistance = enemy.GetComponent<EnemyClass>().GetDistanceToBase().distance;
                }
            }
        }
    }

    public override void OnTargetEnteredDetectionRange(GameObject target)
    {
        base.OnTargetEnteredDetectionRange(target);
        target.GetComponent<EnemyClass>().EnemyDeath += OnTargetLeftDetectionRange;

    }

    public override void OnTargetLeftDetectionRange(GameObject target)
    {
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            target.GetComponent<EnemyClass>().EnemyDeath -= OnTargetLeftDetectionRange;
        }
    }
}
