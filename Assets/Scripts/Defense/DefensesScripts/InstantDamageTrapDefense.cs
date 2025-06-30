using UnityEngine;

public class InstantDamageTrapDefense : TrapDefense
{
    protected override void DoMainAction()
    {
        CancelInvoke("DoMainAction");
        readyToActivate = false;
        foreach (GameObject target in targetsInRange)
        {
            target.GetComponent<EnemyClass>().TakeDamage(defenseSO.damage, gameObject);
        }
    }
}
