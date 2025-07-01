using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Scriptable Objects/Gun")]
public class GunSO : ScriptableObject
{
    [Header("General Data")]
    public string gunName;
    public string description;
    public LayerMask layer;
    public GameObject bulletPrefab;
    [Header("Gun Data")]
    public float damage;
    public float attackCooldown;
    public float range;
    public float bulletSpeed;
    public float bulletLifeTime;

    public virtual void Attack(Transform gunCannon)
    {
        if (bulletPrefab)
        {
            GameObject newBullet = Instantiate(bulletPrefab, gunCannon.position, gunCannon.rotation);
            newBullet.GetComponent<ProjectileClass>().SetValues(damage, bulletLifeTime);
            newBullet.gameObject.GetComponent<Rigidbody>().AddForce(gunCannon.forward * bulletSpeed, ForceMode.Impulse);
        }
        else
        {
            foreach (RaycastHit hit in Physics.RaycastAll(gunCannon.position, gunCannon.forward, range, layer, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<EnemyClass>().TakeDamage(damage, null);
                    return;
                }
            }
        }
    }
}
