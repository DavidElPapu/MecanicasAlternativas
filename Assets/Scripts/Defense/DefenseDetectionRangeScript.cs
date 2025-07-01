using UnityEngine;

public class DefenseDetectionRangeScript : MonoBehaviour
{
    private DefenseClass defense;
    private bool isInitialized;

    public void InitializeDetection(DefenseClass defense)
    {
        this.defense = defense;
        isInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized)
            return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            defense.OnTargetEnteredDetectionRange(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isInitialized)
            return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            defense.OnTargetLeftDetectionRange(other.gameObject);
        }
            
    }
}
