using UnityEngine;
using System;

public abstract class DefenseClass : MonoBehaviour
{
    public DefensesSO defenseSO;
    public event Action<GameObject> DefenseBroken;
    protected bool isBroken;
    protected float damageMultiplier;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected int currentLevel;
    [SerializeField] protected GameObject defenseModel;
    [SerializeField] protected Material previewMAT;

    public virtual void Awake()
    {
        currentLevel = 0;
    }

    protected virtual void OnWaveStart()
    {

    }

    protected virtual void OnBreakStart()
    {

    }

    public virtual void OnPlacing()
    {
        if (defenseSO == null)
            Debug.LogError("No tiene defenseSO");
        isBroken = false;
        damageMultiplier = 1f;
        currentHealth = defenseSO.health;
        currentLevel = 1;
        //cuando se ponga, mejor que cambie a otro material, no el preview
        ChangeModelMaterials(Color.white);
        defenseModel.GetComponent<BoxCollider>().isTrigger = false;
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
        isBroken = false;
    }

    public virtual void OnUpgrading()
    {
        currentLevel++;
        //Debug.Log("Subi al nivel" + currentLevel);
    }

    public virtual void ChangeModelMaterials(Color color)
    {
        if (color != Color.white)
            color.a = 0.3f;
        previewMAT.color = color;
        foreach (MeshRenderer modelPartsRenderers in defenseModel.GetComponentsInChildren<MeshRenderer>())
        {
            modelPartsRenderers.material = previewMAT;
        }
    }

    public virtual void OnDamaged(float damage)
    {

    }

    public virtual int GetCurrentLevel()
    {
        return currentLevel;
    }
}
