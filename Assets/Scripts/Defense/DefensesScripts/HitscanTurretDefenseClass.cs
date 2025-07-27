using UnityEngine;
using System.Collections;

public class HitscanTurretDefenseClass : TurretDefenseClass
{
    public GameObject shootingEffect, hitEffect;
    public LayerMask collisionLayer;
    public float effectTime;

    protected override void Awake()
    {
        base.Awake();
        if (shootingEffect.activeSelf)
            shootingEffect.SetActive(false);
        if (hitEffect.activeSelf)
            hitEffect.SetActive(false);
    }

    protected override void Shoot()
    {
        shootingEffect.transform.position = turretCannons[currentLevel].position;
        shootingEffect.transform.LookAt(Camera.main.transform.position);
        StartCoroutine(ShowEffect(shootingEffect));
        Ray ray = new Ray(turretCannons[currentLevel].position, turretCannons[currentLevel].forward);
        if (Physics.Raycast(ray, out RaycastHit hit, defenseLevels[currentLevel].range + recoilDistance + 1f, collisionLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject != null)
            {
                hitEffect.transform.position = hit.point;
                hitEffect.transform.LookAt(Camera.main.transform.position);
                StartCoroutine(ShowEffect(hitEffect));
                if (hit.collider.gameObject.CompareTag("Enemy"))
                    hit.collider.gameObject.GetComponent<EnemyClass>().TakeDamage(defenseLevels[currentLevel].damage * damageMultiplier, gameObject);
            }
        }
    }

    protected virtual IEnumerator ShowEffect(GameObject effect)
    {
        effect.SetActive(true);
        yield return new WaitForSeconds(effectTime);
        effect.SetActive(false);
    }
}
