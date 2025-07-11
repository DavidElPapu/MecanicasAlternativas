using UnityEngine;
using System.Collections;

public class AreaDamageProjectile : EnemyDamageArea
{
    public GameObject projectileModel;
    public GameObject damageArea;
    public float areaTime;

    public override void SetValues(GameObject owner, float damage, bool destroyOnHit, float lifeTime)
    {
        this.owner = owner;
        this.damage = damage;
        this.destroyOnHit = destroyOnHit;
        damageArea.GetComponent<EnemyDamageArea>().SetValues(owner, damage, false, 0f);
        damageArea.SetActive(false);
        if (lifeTime > 0)
            StartCoroutine(StartAreaDamage(lifeTime));
        hasValues = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!hasValues) return;

        if (destroyOnHit && !other.isTrigger)
        {
            StopAllCoroutines();
            StartCoroutine(StartAreaDamage(0f));
        }
    }

    protected IEnumerator StartAreaDamage(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        damageArea.SetActive(true);
        projectileModel.SetActive(false);
        yield return new WaitForSeconds(areaTime);
        damageArea.SetActive(false);
        Destroy(gameObject);
    }
}
