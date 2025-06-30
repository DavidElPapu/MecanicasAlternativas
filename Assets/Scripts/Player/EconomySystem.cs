using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    public PlayerMainUI playerUI;
    public int startingMoney;
    private int currentMoney;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMoney = startingMoney;
        playerUI.ChangeMoneyText(currentMoney);
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void ChangeCurrentMoney(int moneyChange)
    {
        currentMoney += moneyChange;
        if (currentMoney < 0)
            currentMoney = 0;
        playerUI.ChangeMoneyText(currentMoney);
    }
}
