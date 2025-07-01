using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class DefenseClass : MonoBehaviour
{
    public DefensesSO defenseSO;
    public DefenseDetectionRangeScript detectionRange;
    [SerializeField] protected Material defenseMAT;
    public event Action<GameObject> DefenseBroken;
    protected bool isBroken, isActive;
    protected int currentLevel;
    protected float currentHealth, currentCooldown, damageMultiplier;
    [SerializeField] protected GameObject defenseModel;
    [SerializeField] protected Material previewMAT;
    protected GameObject currentTarget;
    protected List<GameObject> targetsInRange = new List<GameObject>();

    public virtual void Awake()
    {
        currentLevel = 0;
        isActive = false;
    }

    protected virtual void Update()
    {
        //if (!isActive)
        //    return;
        //if (isActing)
        //    DoMainAction();
    }

    protected abstract void DoMainAction();

    protected virtual void OnWaveStart()
    {
        if (!isActive)
            isActive = true;
    }

    protected virtual void OnBreakStart()
    {
        OnRepair();
        if (isActive)
            isActive = false;
    }

    public virtual void OnPlacing()
    {
        if (defenseSO == null)
            Debug.LogError("No tiene defenseSO");
        if (detectionRange != null)
            detectionRange.InitializeDetection(this);
        isBroken = false;
        isActive = true;
        currentLevel = 1;
        currentHealth = defenseSO.health;
        currentCooldown = defenseSO.mainCooldown;
        damageMultiplier = 1f;
        currentTarget = null;
        //defenseModel.GetComponent<BoxCollider>().isTrigger = false;
        foreach (MeshRenderer modelPartsRenderers in defenseModel.GetComponentsInChildren<MeshRenderer>())
        {
            modelPartsRenderers.material = defenseMAT;
        }
        LevelManager.OnWaveStart += OnWaveStart;
        LevelManager.OnBreakStart += OnBreakStart;
    }

    public virtual void OnDeleting()
    {
        //En teoria aqui no hace falta llamar el evento de defense broken ya que no deberia haber enemigos cuando se quitan defensas
        Destroy(this.gameObject);
    }

    protected virtual void OnBroken()
    {
        DefenseBroken?.Invoke(gameObject);
        isBroken = true;
    }

    protected virtual void OnRepair()
    {
        if (isBroken)
            isBroken = false;
        currentHealth = defenseSO.health;

    }

    public virtual void OnUpgrading()
    {
        currentLevel++;
        //Debug.Log("Subi al nivel" + currentLevel);
    }

    public virtual void ChangeModelMaterials(Color color)
    {
        color.a = 0.3f;
        previewMAT.color = color;
        foreach (MeshRenderer modelPartsRenderers in defenseModel.GetComponentsInChildren<MeshRenderer>())
        {
            modelPartsRenderers.material = previewMAT;
        }
    }

    public virtual void OnDamaged(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            OnBroken();
    }

    public virtual void OnTargetEnteredDetectionRange(GameObject target)
    {
        targetsInRange.Add(target);
    }

    public virtual void OnTargetLeftDetectionRange(GameObject target)
    {
        if (targetsInRange.Contains(target))
            targetsInRange.Remove(target);
    }

    public virtual int GetCurrentLevel()
    {
        return currentLevel;
    }

    public virtual bool CanBeAttackedByEnemy()
    {
        if (!isActive || isBroken)
            return false;
        else if (defenseSO.enemyInteraction == DefensesSO.EnemyInteractionType.IgnoreEnemyAndDefense || defenseSO.enemyInteraction == DefensesSO.EnemyInteractionType.IgnoreDefenseTargetEnemy)
            return false;
        else
            return true;
    }
}
