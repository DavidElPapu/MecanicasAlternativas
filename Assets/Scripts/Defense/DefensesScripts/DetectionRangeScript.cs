using System.Collections.Generic;
using UnityEngine;

public class DetectionRangeScript : MonoBehaviour
{
    public List<GameObject> enemiesInRange = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            enemiesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            enemiesInRange.Remove(other.gameObject);
    }
}
