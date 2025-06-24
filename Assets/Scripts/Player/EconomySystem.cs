using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    public int startingMoney;
    private int currentMoney;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMoney = startingMoney;
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void ChangeCurrentMoney(int moneyChange)
    {
        currentMoney += moneyChange;
    }
}
