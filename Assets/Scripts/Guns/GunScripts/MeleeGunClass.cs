using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class MeleeGunClass : GunClass
{
    public GameObject hitbox;
    [SerializeField] protected float hitboxTime;

    protected virtual void Start()
    {
        hitbox.GetComponent<EnemyDamageArea>().SetValues(null, gunSO.damage, false, 0f);
        hitbox.SetActive(false);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        hitbox.SetActive(false);
    }

    public override void Attack()
    {
        StartCoroutine(ActivateMeleeHitbox());
    }

    protected virtual IEnumerator ActivateMeleeHitbox()
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(hitboxTime);
        hitbox.SetActive(false);
    }
}
