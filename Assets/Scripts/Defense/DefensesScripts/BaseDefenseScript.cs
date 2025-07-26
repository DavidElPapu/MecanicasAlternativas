using UnityEngine;

public class BaseDefenseScript : BreakableDefenseClass
{
    public LevelManager levelManager;
    public float maxBaseHealth;
    //protected float healingPerWave;

    protected override void Awake()
    {
        isActive = true;
        currentHealth = maxBaseHealth;
    }

    protected override void Update()
    {
        //Nada
    }

    protected override void DoMainAction()
    {
        //Nada
    }

    protected override void OnWaveStart()
    {
        //nada
    }

    protected override void OnBreakStart()
    {
        //nada
    }

    public override void OnPlacing()
    {
        //No se puede colocar
    }

    public override void OnDeleting()
    {
        //Ni eliminar
    }

    protected override void OnBroken()
    {
        //Ni romper
    }

    protected override void OnRepair(bool activateOnRepair)
    {
        //Ni reparar

    }

    public override void OnUpgrading()
    {
        //Ni mejorar (por ahora)
    }

    public override void ChangeModelMaterials(Color color)
    {
        //no
    }

    public override void OnDamaged(float damage)
    {
        currentHealth -= damage;
        levelManager.playerUI.ChangeBaseHealthUI(currentHealth);
        if (currentHealth <= 0)
            levelManager.BaseDied();
    }

    public override bool CanBeAttackedByEnemy()
    {
        return true;
    }

    protected override void MoveCanvasToCamera()
    {
        //No tiene canvas
    }
}
