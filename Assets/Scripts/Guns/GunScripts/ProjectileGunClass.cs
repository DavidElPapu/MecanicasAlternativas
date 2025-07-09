using UnityEngine;

public class ProjectileGunClass : GunClass
{
    public GameObject projectilePrefab;
    public Transform gunCannon;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileLifeTime;

    public override void Attack()
    {
        GameObject newBullet = Instantiate(projectilePrefab, gunCannon.position, gunCannon.rotation);
        newBullet.GetComponentInChildren<EnemyDamageArea>().SetValues(null, gunSO.damage, true, projectileLifeTime);
        newBullet.gameObject.GetComponent<Rigidbody>().AddForce(gunCannon.forward * projectileSpeed, ForceMode.Impulse);
    }
}
