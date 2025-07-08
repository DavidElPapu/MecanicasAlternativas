using UnityEngine;

public class EnemyDamageArea : MonoBehaviour
{
    private GameObject owner;
    private float damage;
    private bool destroyOnHit, hasValues;


    private void Awake()
    {
        hasValues = false;
    }

    public void SetValues(GameObject owner, float damage, bool destroyOnHit)
    {
        this.owner = owner;
        this.damage = damage;
        this.destroyOnHit = destroyOnHit;
        hasValues = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasValues) return;

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyClass>().TakeDamage(damage, owner);
            if (destroyOnHit)
                Destroy(this.gameObject);
        }
    } 
}
