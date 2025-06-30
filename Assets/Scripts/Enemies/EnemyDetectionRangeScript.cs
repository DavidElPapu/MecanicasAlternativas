using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRangeScript : MonoBehaviour
{
    private EnemyClass enemy;
    private bool isInitialized;

    public void InitializeDetection(EnemyClass enemy)
    {
        this.enemy = enemy;
        isInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.OnTargetEnteredAttackZone(other.gameObject, true);
        }
        else if (other.gameObject.CompareTag("Defense"))
        {
            //falta checar que la defensa este activa
            enemy.OnTargetEnteredAttackZone(other.gameObject, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isInitialized)
            return;
        if (other.gameObject.CompareTag("Player"))
            enemy.OnTargetLeftAttackZone(other.gameObject, true);
        else if (other.gameObject.CompareTag("Defense"))
            enemy.OnTargetLeftAttackZone(other.gameObject, false);
    }
}
