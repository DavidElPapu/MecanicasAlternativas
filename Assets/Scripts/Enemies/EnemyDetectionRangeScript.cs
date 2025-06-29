using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRangeScript : MonoBehaviour
{
    private EnemyClass enemy;
    private bool targetPlayer, isInitialized, isAttackRange;
    private List<GameObject> targetsInRange = new List<GameObject>();

    public void InitializeDetection(EnemyClass enemy, bool canTargetPlayer, bool isAttackRange)
    {
        this.enemy = enemy;
        targetPlayer = canTargetPlayer;
        this.isAttackRange = isAttackRange;
        isInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized)
            return;
        if (other.gameObject.CompareTag("Player") && targetPlayer)
        {
            //falta checar que el jugador este vivo
            if (isAttackRange)
                enemy.OnTargetEnteredAttackZone(other.gameObject, true);
            else
                Debug.Log("Toca Seguir al jugador");
            targetsInRange.Add(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Defense") && isAttackRange)
        {
            //falta checar que la defensa este activa
            enemy.OnTargetEnteredAttackZone(other.gameObject, false);
            targetsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isInitialized)
            return;
        if (targetsInRange.Contains(other.gameObject))
        {
            targetsInRange.Remove(other.gameObject);
            if (isAttackRange)
            {
                if (targetsInRange.Count > 0)
                    enemy.OnTargetLeftAttackZone(targetsInRange[0]);
                else
                    enemy.OnTargetLeftAttackZone(null);
            }
            else if (targetsInRange.Count > 0) 
            {
                //aca toca hacer lo mismo, pero para el rango de seguimiento
            }
        }
    }
}
