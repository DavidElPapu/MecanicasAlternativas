using UnityEngine;

public class InstantDamageTrapDefense : TrapDefenseClass
{
    protected override void DoMainAction()
    {
        base.DoMainAction();
        foreach (GameObject target in targetsInRange)
        {
            target.GetComponent<EnemyClass>().TakeDamage(defenseLevels[currentLevel].damage * damageMultiplier, gameObject);
        }
    }
}
