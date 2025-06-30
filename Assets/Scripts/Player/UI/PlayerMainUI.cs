using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMainUI : MonoBehaviour
{
    public Slider baseHealthSlider;
    public TextMeshProUGUI baseHealthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBaseHealthUI(float newHealth)
    {
        baseHealthSlider.value = newHealth;
        baseHealthText.text = newHealth.ToString();
    }
}
