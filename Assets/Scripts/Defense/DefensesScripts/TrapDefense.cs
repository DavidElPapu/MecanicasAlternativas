using UnityEngine;

public class TrapDefense : MonoBehaviour
{
    public DetectionRangeScript detectionRange;
    public DefensesSO defenseSO;
    private float mainCooldownCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCooldownCount = 0;
    }

    void Update()
    {
        if(mainCooldownCount >= defenseSO.mainCooldown)
        {
            if (detectionRange.enemiesInRange.Count > 0f)
                Invoke("Attack", defenseSO.secondaryCooldown);
        }
        else
        {
            mainCooldownCount += Time.deltaTime;
        }
    }

    private void Attack()
    {
        //realiza su ataque
        mainCooldownCount = 0f;
    }
}
