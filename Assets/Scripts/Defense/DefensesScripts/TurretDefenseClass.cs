using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public abstract class TurretDefenseClass : BreakableDefenseClass
{
    public enum TargetPriorities
    {
        First,
        Strongest,
        Tankiest,
        Random
    }
    public TargetPriorities targetPriority;
    public List<Transform> turretCannons = new List<Transform>();
    public List<Transform> turretHeads = new List<Transform>();
    public float reloadTime, recoilDistance, recoilTime;
    [Header("Ammo UI")]
    public Slider ammoSlider;
    public TextMeshProUGUI ammoText;

    protected float currentReloadCooldown;
    protected int currentAmmo;
    protected bool isAttacking;

    protected override void Awake()
    {
        base.Awake();
        isAttacking = false;
        currentCooldown = 0f;
        damageMultiplier = 1f;
        currentTarget = null; 
        detectionRange.gameObject.transform.localScale = new Vector3(defenseLevels[currentLevel].range * 2, defenseLevels[currentLevel].range * 2, defenseLevels[currentLevel].range * 2);
    }
    public override void OnPlacing()
    {
        base.OnPlacing();
        if (ammoSlider && ammoText)
        {
            currentAmmo = defenseLevels[currentLevel].maxUses;
            ammoSlider.wholeNumbers = true;
            ammoSlider.maxValue = currentAmmo;
            ammoSlider.value = currentAmmo;
            ammoText.text = currentAmmo.ToString();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isActive && currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0 && isAttacking)
            {
                currentCooldown = defenseLevels[currentLevel].mainCooldown;
                DoMainAction();
            }
        }
        if (isActive && defenseLevels[currentLevel].maxUses > 0 && currentReloadCooldown > 0)
        {
            currentReloadCooldown -= Time.deltaTime;
            if(currentReloadCooldown <= 0)
            {
                currentAmmo = defenseLevels[currentLevel].maxUses;
                ammoSlider.value = currentAmmo;
                ammoText.text = currentAmmo.ToString();
            }
        }
    }

    protected override void DoMainAction()
    {
        currentTarget = GetTargetEnemy();
        if (currentTarget == null || (defenseLevels[currentLevel].maxUses > 0 && currentAmmo <= 0)) return;
        turretHeads[currentLevel].LookAt(currentTarget.transform.position);
        if (defenseLevels[currentLevel].maxUses > 0)
        {
            currentAmmo -= 1;
            if (currentAmmo <= 0)
            {
                ammoSlider.value = 0;
                ammoText.text = "Recargando";
                currentReloadCooldown = reloadTime;
            }
            else
            {
                ammoSlider.value = currentAmmo;
                ammoText.text = currentAmmo.ToString();
            }
        }
        StartCoroutine(RecoilTurretHead());
        Shoot();
    }

    protected abstract void Shoot();

    protected virtual GameObject GetTargetEnemy()
    {
        GameObject targetEnemy = null;
        if (targetsInRange.Count <= 0)
            return targetEnemy;
        switch (targetPriority)
        {
            case TargetPriorities.First:
                int highestWayPoint = 0;
                float bestDistance = 1000f;
                foreach (GameObject enemy in targetsInRange)
                {
                    if (enemy.GetComponent<EnemyClass>().GetDistanceToBase().wayPoint >= highestWayPoint)
                    {
                        if (enemy.GetComponent<EnemyClass>().GetDistanceToBase().distance < bestDistance)
                        {
                            targetEnemy = enemy;
                            highestWayPoint = enemy.GetComponent<EnemyClass>().GetDistanceToBase().wayPoint;
                            bestDistance = enemy.GetComponent<EnemyClass>().GetDistanceToBase().distance;
                        }
                    }
                }
                break;
            case TargetPriorities.Strongest:
                float highestStrenght = 0;
                foreach (GameObject enemy in targetsInRange)
                {
                    if (enemy.GetComponent<EnemyClass>().GetStrenght() > highestStrenght)
                    {
                        targetEnemy = enemy;
                        highestStrenght = enemy.GetComponent<EnemyClass>().GetStrenght();
                    }
                }
                break;
            case TargetPriorities.Tankiest:
                float highestHealth = 0;
                foreach (GameObject enemy in targetsInRange)
                {
                    if (enemy.GetComponent<EnemyClass>().GetHealth() > highestHealth)
                    {
                        targetEnemy = enemy;
                        highestHealth = enemy.GetComponent<EnemyClass>().GetHealth();
                    }
                }
                break;
            case TargetPriorities.Random:
                targetEnemy = targetsInRange[Random.Range(0, targetsInRange.Count)];
                break;
            default:
                Debug.LogError("No tiene priority");
                break;
        }
        return targetEnemy;
    }

    public override void OnTargetEnteredDetectionRange(GameObject target)
    {
        base.OnTargetEnteredDetectionRange(target);
        target.GetComponent<EnemyClass>().EnemyDeath += OnTargetLeftDetectionRange;
        if (!isAttacking)
        {
            if (currentCooldown <= 0)
            {
                currentCooldown = defenseLevels[currentLevel].mainCooldown;
                DoMainAction();
            }
            isAttacking = true;
        }

    }

    public override void OnTargetLeftDetectionRange(GameObject target)
    {
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            target.GetComponent<EnemyClass>().EnemyDeath -= OnTargetLeftDetectionRange;
            if (targetsInRange.Count <= 0)
                isAttacking = false;
        }
    }

    protected override void OnRepair(bool activateOnRepair)
    {
        base.OnRepair(activateOnRepair);
        if (defenseLevels[currentLevel].maxUses > 0)
        {
            currentAmmo = defenseLevels[currentLevel].maxUses;
            ammoSlider.value = currentAmmo;
            ammoText.text = currentAmmo.ToString();
        }
    }

    public override void OnUpgrading()
    {
        base.OnUpgrading();
        if (defenseLevels[currentLevel].maxUses > 0)
        {
            currentAmmo = defenseLevels[currentLevel].maxUses;
            ammoSlider.maxValue = currentAmmo;
            ammoSlider.value = currentAmmo;
            ammoText.text = currentAmmo.ToString();
        }
        if (defenseLevels[currentLevel].range > 0)
            detectionRange.gameObject.transform.localScale = new Vector3(defenseLevels[currentLevel].range * 2, defenseLevels[currentLevel].range * 2, defenseLevels[currentLevel].range * 2);
    }

    protected virtual IEnumerator RecoilTurretHead()
    {
        turretHeads[currentLevel].Translate(Vector3.back * recoilDistance);
        yield return new WaitForSeconds(recoilTime);
        turretHeads[currentLevel].Translate(Vector3.forward * recoilDistance);
    }
}
