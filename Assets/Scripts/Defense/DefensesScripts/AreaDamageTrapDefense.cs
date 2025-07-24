using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class AreaDamageTrapDefense : TrapDefenseClass
{
    public GameObject damageArea;
    public float areaTime;
    public bool isOneTimeUse;

    public override void OnPlacing()
    {
        base.OnPlacing();
        damageArea.SetActive(true);
        damageArea.GetComponent<EnemyDamageArea>().SetValues(gameObject, defenseLevels[currentLevel].damage, false, 0);
        damageArea.SetActive(false);
    }

    protected override void OnWaveStart()
    {
        base.OnWaveStart();
        if (!defenseModels[currentLevel].activeSelf)
            defenseModels[currentLevel].SetActive(true);
    }

    protected override void Update()
    {
        if (!readyToActivate && !isOneTimeUse)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                readyToActivate = true;
                currentCooldown = defenseLevels[currentLevel].mainCooldown;
                if (!defenseModels[currentLevel].activeSelf)
                    defenseModels[currentLevel].SetActive(true);
                if (targetsInRange.Count > 0)
                    Invoke("DoMainAction", trapDelay);
            }
        }
    }

    public override void OnUpgrading()
    {
        base.OnUpgrading();
        damageArea.SetActive(true);
        damageArea.GetComponent<EnemyDamageArea>().SetValues(gameObject, defenseLevels[currentLevel].damage, false, 0);
        damageArea.SetActive(false);
        if (defenseLevels[currentLevel].range > 0)
            damageArea.transform.localScale = new Vector3(defenseLevels[currentLevel].range, defenseLevels[currentLevel].range, defenseLevels[currentLevel].range);
    }

    protected override void DoMainAction()
    {
        base.DoMainAction();
        StartCoroutine(ActivateDamageArea());
    }

    protected virtual IEnumerator ActivateDamageArea()
    {
        damageArea.SetActive(true);
        defenseModels[currentLevel].SetActive(false);
        yield return new WaitForSeconds(areaTime);
        damageArea.SetActive(false);
    }
}
