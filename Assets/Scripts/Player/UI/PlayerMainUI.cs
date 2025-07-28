using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMainUI : MonoBehaviour
{
    public GameObject deathUI, mainUI;
    public Slider baseHealthSlider, playerHealthSlider;
    public TextMeshProUGUI baseHealthText, playerHealthText, waveStatusText, enemiesAliveText, moneyText, selectedItemNameText, selectedItemInfoText;
    public Image[] hotbarItemsIcons = new Image[9];
    public Image selectedItemIcon;
    public Sprite lockedItemSprite;


    public void OnGameStart()
    {
        mainUI.SetActive(true);
    }

    public void SetHotbarIcons(Sprite[] icons)
    {
        for (int i = 0; i < hotbarItemsIcons.Length; i++)
        {
            if (icons[i] != null)
            {
                if (!hotbarItemsIcons[i].gameObject.activeSelf)
                    hotbarItemsIcons[i].gameObject.SetActive(true);
                hotbarItemsIcons[i].sprite = icons[i];
            }
            else
                hotbarItemsIcons[i].gameObject.SetActive(false);
        }
    }

    public void ChangeSelectedItemUI(Sprite newIcon, string newName, string newInfo)
    {
        selectedItemIcon.sprite = newIcon;
        selectedItemNameText.text = newName;
        selectedItemInfoText.text = newInfo;
    }

    public void ToggleDeathUI(bool showUI)
    {
        if (showUI)
            deathUI.SetActive(true);
        else
            deathUI.SetActive(false);
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
