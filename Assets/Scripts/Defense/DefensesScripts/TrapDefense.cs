using UnityEngine;

public class TrapDefense : DefenseClass
{
    [SerializeField] protected float trapDelay;
    protected bool readyToActivate;

    public override void OnPlacing()
    {
        base.OnPlacing();
        readyToActivate = true;
    }

    protected override void OnRepair()
    {
        readyToActivate = true;
    }

    protected override void Update()
    {
        if (!readyToActivate)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                readyToActivate = true;
                currentCooldown = defenseSO.mainCooldown;
                if (targetsInRange.Count > 0)
                    Invoke("DoMainAction", trapDelay);
            }
        }
    }

    protected override void DoMainAction()
    {
        CancelInvoke("DoMainAction");
        readyToActivate = false;

    }

    public override void OnTargetEnteredDetectionRange(GameObject target)
    {
        base.OnTargetEnteredDetectionRange(target);
        Invoke("DoMainAction", trapDelay);
    }

    public override void OnTargetLeftDetectionRange(GameObject target)
    {
        base.OnTargetLeftDetectionRange(target);
        Invoke("DoMainAction", trapDelay);
    }
}
