using UnityEngine;
using System;

public class PlayerStatus : MonoBehaviour
{
    public PlayerMainUI playerUI;
    public float maxHealth;
    public static event Action PlayerDeath;
    public static event Action PlayerRevive;
    [SerializeField] private float currentHealth;
    private bool isAlive;

    void Start()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isAlive = false;
            currentHealth = 0;
            PlayerDeath?.Invoke();
        }
        playerUI.ChangePlayerHealth(currentHealth);
    }

    public void GetHeal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        playerUI.ChangePlayerHealth(currentHealth);
    }

    public void OnRevive()
    {
        currentHealth = maxHealth;
        playerUI.ChangePlayerHealth(currentHealth);
        isAlive = true;
        PlayerRevive?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
