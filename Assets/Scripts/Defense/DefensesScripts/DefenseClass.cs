using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public abstract class DefenseClass : MonoBehaviour
{
    [Header("General Data")]
    public string defenseName;
    public string description;
    public Sprite icon;
    public Vector3Int size = new Vector3Int(1, 1, 1);
    [Header("Defense Data")]
    public List<ObjectData.CellState> validCells = new List<ObjectData.CellState>();
    public enum EnemyInteractionType
    {
        IgnoreEnemyAndDefense,
        IgnoreEnemyTargetDefense,
        IgnoreDefenseTargetEnemy,
        TargetDefenseAndEnemy,
    }
    public EnemyInteractionType enemyInteraction;
    [Header("Components")]
    public List<DefensesSO> defenseLevels = new List<DefensesSO>();
    public List<GameObject> defenseModels = new List<GameObject>();
    public DefenseDetectionRangeScript detectionRange;
    public Material defenseMAT, previewMAT;
    public event Action<GameObject> DefenseBroken;
    [Header("Stats")]
    protected bool isBroken, isActive;
    protected int currentLevel;
    protected float currentHealth, currentCooldown, damageMultiplier;
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
        if (defenseLevels[currentLevel] == null)
            Debug.LogError("No tiene defenseSO");
        if (detectionRange != null)
            detectionRange.InitializeDetection(this);
        isBroken = false;
        isActive = true;
        currentHealth = defenseLevels[currentLevel].maxHealth;
        currentCooldown = defenseLevels[currentLevel].mainCooldown;
        damageMultiplier = 1f;
        currentTarget = null;
        //defenseModel.GetComponent<BoxCollider>().isTrigger = false;
        foreach (MeshRenderer modelPartsRenderers in defenseModels[currentLevel].GetComponentsInChildren<MeshRenderer>())
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
        currentHealth = defenseLevels[0].maxHealth;

    }

    public virtual void OnUpgrading()
    {
        defenseModels[currentLevel].SetActive(false);
        currentLevel++;
        defenseModels[currentLevel].SetActive(true);
    }

    public virtual void ChangeModelMaterials(Color color)
    {
        color.a = 0.3f;
        previewMAT.color = color;
        foreach (MeshRenderer modelPartsRenderers in defenseModels[currentLevel].GetComponentsInChildren<MeshRenderer>())
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

    public virtual int GetUpgradePrice()
    {
        return defenseLevels[currentLevel + 1].price;
    }

    public virtual bool CanBeUpgraded()
    {
        if ((currentLevel + 1) < defenseLevels.Count)
            return true;
        else
            return false;
    }

    public virtual bool CanBeAttackedByEnemy()
    {
        if (!isActive || isBroken)
            return false;
        else if (enemyInteraction == EnemyInteractionType.IgnoreEnemyAndDefense || enemyInteraction == EnemyInteractionType.IgnoreDefenseTargetEnemy)
            return false;
        else
            return true;
    }
}
