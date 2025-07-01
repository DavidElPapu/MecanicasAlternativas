using UnityEngine;

public class ProjectileClass : MonoBehaviour
{
    protected float damage;
    protected float lifeTime;
    protected bool isInitialized;

    public virtual void SetValues(float projectileDamage, float projectileLife)
    {
        damage = projectileDamage;
        lifeTime = projectileLife;
        isInitialized = true;
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || !isInitialized)
            return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnImpact(other.gameObject);
        }
    }

    protected virtual void OnImpact(GameObject enemy)
    {
        enemy.GetComponent<EnemyClass>().TakeDamage(damage, null);
        Destroy(gameObject);
    }
}
