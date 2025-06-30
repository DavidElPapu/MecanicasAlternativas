using UnityEngine;
using System;

public class PlayerStatus : MonoBehaviour
{
    public float maxHealth;
    public static event Action PlayerDeath;
    public static event Action PlayerRevive;
    private float currentHealth;
    private bool isAlive;

    void Start()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth < 0)
        {
            isAlive = false;
            PlayerDeath?.Invoke();
        }
    }

    public void GetHeal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void OnRevive()
    {
        currentHealth = maxHealth;
        isAlive = true;
        PlayerRevive?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
