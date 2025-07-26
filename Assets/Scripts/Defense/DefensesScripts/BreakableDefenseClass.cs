using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class BreakableDefenseClass : DefenseClass
{
    public GameObject brokenDefenseModel;
    public Collider modelHibox;
    public event Action<GameObject> DefenseBroken;
    protected float currentHealth;
    [Header("UI")]
    public GameObject statsCanvas;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    protected override void Awake()
    {
        base.Awake();
        modelHibox.isTrigger = true;
        if (brokenDefenseModel.activeSelf)
            brokenDefenseModel.SetActive(false);
        if (statsCanvas.activeSelf)
            statsCanvas.SetActive(false);
    }
    protected override void Update()
    {
        MoveCanvasToCamera();
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
        statsCanvas.SetActive(true);
        healthSlider.maxValue = currentHealth;
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
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
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    public virtual void OnDamaged(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            healthSlider.value = 0;
            healthText.text = "Destruido";
            OnBroken();
        }
        else
        {
            healthSlider.value = currentHealth;
            healthText.text = currentHealth.ToString();
        }
    }

    public override void OnUpgrading()
    {
        base.OnUpgrading();
        currentHealth = defenseLevels[currentLevel].maxHealth;
        healthSlider.maxValue = currentHealth;
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    protected virtual void MoveCanvasToCamera()
    {
        statsCanvas.transform.LookAt(Camera.main.transform.position);
    }
}
