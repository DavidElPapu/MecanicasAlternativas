using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMainUI : MonoBehaviour
{
    public Slider baseHealthSlider, playerHealthSlider;
    public TextMeshProUGUI baseHealthText, playerHealthText, waveStatusText, enemiesAliveText, moneyText;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeMoneyText(int money)
    {
        moneyText.text = "$" + money.ToString();
    }

    public void ChangeEnemiesAliveText(int enemiesAlive)
    {
        if (enemiesAlive <= 0)
            enemiesAliveText.text = "...";
        else
            enemiesAliveText.text = "Enemies Alive: " + enemiesAlive.ToString();
    }

    public void ChangeWaveStatus(string statusText)
    {
        waveStatusText.text = statusText;
    }

    public void ChangePlayerHealth(float newHealth)
    {
        playerHealthSlider.value = newHealth;
        playerHealthText.text = newHealth.ToString();
    }

    public void ChangeBaseHealthUI(float newHealth)
    {
        baseHealthSlider.value = newHealth;
        baseHealthText.text = newHealth.ToString();
    }
}
