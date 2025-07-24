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
    [Header("Stats")]
    protected bool isActive;
    protected int currentLevel;
    protected float currentCooldown, damageMultiplier;
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
        if (isActive)
            isActive = false;
    }

    public virtual void OnPlacing()
    {
        if (defenseLevels[currentLevel] == null)
            Debug.LogError("No tiene defenseSO");
        if (detectionRange != null)
            detectionRange.InitializeDetection(this);
        isActive = true;
        currentCooldown = defenseLevels[currentLevel].mainCooldown;
        damageMultiplier = 1f;
        currentTarget = null;
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
        if (!isActive || enemyInteraction == EnemyInteractionType.IgnoreEnemyAndDefense || enemyInteraction == EnemyInteractionType.IgnoreDefenseTargetEnemy)
            return false;
        else
            return true;
    }
}
