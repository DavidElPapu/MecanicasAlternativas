using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyClass>().TakeDamage(damage, null);
        }
    }
}
