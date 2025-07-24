using UnityEngine;
using System;
using UnityEngine.UI;

public class BreakableDefenseClass : DefenseClass
{
    public GameObject brokenDefenseModel;
    public Collider modelHibox;
    public event Action<GameObject> DefenseBroken;
    protected float currentHealth;

    public override void Awake()
    {
        base.Awake();
        modelHibox.isTrigger = true;
        if (brokenDefenseModel.activeSelf)
            brokenDefenseModel.SetActive(false);
    }

    protected override void OnBreakStart()
    {
        base.OnBreakStart();
        OnRepair(false);
    }

    public override void OnPlacing()
    {
        base.OnPlacing();
        currentHealth = defenseLevels[currentLevel].maxHealth;
        modelHibox.isTrigger = false;
    }

    protected override void DoMainAction()
    {
        //Nada xd
    }

    protected virtual void OnBroken()
    {
        isActive = false;
        modelHibox.isTrigger = true;
        defenseModels[currentLevel].SetActive(false);
        brokenDefenseModel.SetActive(true);
        DefenseBroken?.Invoke(gameObject);
    }

    protected virtual void OnRepair(bool activateOnRepair)
    {
        if (!isActive)
        {
            if (activateOnRepair)
                isActive = true;
            modelHibox.isTrigger = false;
            brokenDefenseModel.SetActive(false);
            defenseModels[currentLevel].SetActive(true);
        }
        currentHealth = defenseLevels[currentLevel].maxHealth;
    }

    public virtual void OnDamaged(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            OnBroken();
    }
}
