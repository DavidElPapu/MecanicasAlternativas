using UnityEngine;
using System.Collections;

public class Grenade : ProjectileClass
{
    public GameObject explosion;
    public float explosionDuration = 0.2f;

    public override void SetValues(float projectileDamage, float projectileLife)
    {
        explosion.GetComponent<ExplosionDamage>().damage = projectileDamage;
        if (explosion.activeSelf)
            explosion.SetActive(false);
        lifeTime = projectileLife;
        isInitialized = true;
        Destroy(gameObject, lifeTime);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || !isInitialized)
            return;
        OnImpact(other.gameObject);
    }

    protected override void OnImpact(GameObject enemy)
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        StartCoroutine(Explode());
    }

    protected IEnumerator Explode()
    {
        explosion.SetActive(true);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(explosionDuration);
        explosion.SetActive(false);
        Destroy(gameObject);
    }
}
