using UnityEngine;
using System.Collections;

public class HitscanGunClass : GunClass
{
    public Transform gunCannon;
    public GameObject shootingEffect, hitEffect;
    public float effectTime;

    protected override void Awake()
    {
        isAttacking = false;
        isActive = false;
        currentLevel = 1;
        currentAttackCooldown = 0f;
        damageMultiplier = 1f;
        shootingEffect.transform.position = gunCannon.transform.position;
        shootingEffect.transform.LookAt(Camera.main.transform.position);
        if (shootingEffect.activeSelf)
            shootingEffect.SetActive(false);
        if (hitEffect.activeSelf)
            hitEffect.SetActive(false);
    }

    public override void Attack()
    {
        StartCoroutine(ShowEffect(shootingEffect));
        Ray ray = new Ray(gunCannon.transform.position, gunCannon.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, gunSO.range, gunSO.collisionLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject != null)
            {
                hitEffect.transform.position = hit.point;
                hitEffect.transform.LookAt(Camera.main.transform.position);
                StartCoroutine(ShowEffect(hitEffect));
                if (hit.collider.gameObject.CompareTag("Enemy"))
                    hit.collider.gameObject.GetComponent<EnemyClass>().TakeDamage(gunSO.damage, null);
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
