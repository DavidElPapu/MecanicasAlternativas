using UnityEngine;

public class EnemyDamageArea : MonoBehaviour
{
    protected GameObject owner;
    protected float damage;
    protected bool destroyOnHit, hasValues;


    protected virtual void Awake()
    {
        hasValues = false;
    }

    public virtual void SetValues(GameObject owner, float damage, bool destroyOnHit, float lifeTime)
    {
        this.owner = owner;
        this.damage = damage;
        this.destroyOnHit = destroyOnHit;
        if (lifeTime > 0)
            Destroy(gameObject, lifeTime);
        hasValues = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!hasValues) return;

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyClass>().TakeDamage(damage, owner);
        }
        if (destroyOnHit && !other.isTrigger)
            Destroy(this.gameObject);
    } 
}
