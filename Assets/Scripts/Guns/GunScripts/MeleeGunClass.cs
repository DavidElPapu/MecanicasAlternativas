using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class MeleeGunClass : GunClass
{
    public GameObject hitbox;

    [SerializeField] protected float hitboxTime;

    protected override void Awake()
    {
        base.Awake();
        hitbox.GetComponent<EnemyDamageArea>().SetValues(gameObject, gunSO.damage, false);
    }

    public override void OnSelect()
    {
        base.OnSelect();

    }

    public override void OnDeselect()
    {
        base.OnDeselect();
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
