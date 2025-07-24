using UnityEngine;

public class TrapDefenseClass : DefenseClass
{
    [SerializeField] protected float trapDelay;
    protected bool readyToActivate;

    public override void OnPlacing()
    {
        base.OnPlacing();
        readyToActivate = true;
    }


    protected override void OnWaveStart()
    {
        base.OnWaveStart();
        readyToActivate = true;
    }

    public override void OnUpgrading()
    {
        base.OnUpgrading();
        currentCooldown = defenseLevels[currentLevel].mainCooldown;
    }

    protected override void Update()
    {
        if (!readyToActivate)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                readyToActivate = true;
                currentCooldown = defenseLevels[currentLevel].mainCooldown;
                if (targetsInRange.Count > 0)
                    Invoke("DoMainAction", trapDelay);
            }
        }
    }

    protected override void DoMainAction()
    {
        if (IsInvoking("DoMainAction"))
            CancelInvoke("DoMainAction");
        readyToActivate = false;
    }

    public override void OnTargetEnteredDetectionRange(GameObject target)
    {
        base.OnTargetEnteredDetectionRange(target);
        if (readyToActivate)
            Invoke("DoMainAction", trapDelay);
    }

    public override void OnTargetLeftDetectionRange(GameObject target)
    {
        base.OnTargetLeftDetectionRange(target);
    }
}
