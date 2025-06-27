using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public EnemiesSO enemySO;
    public EnemySpawnManager enemyManager;
    protected Rigidbody rb;
    [Header("Stats")]
    [SerializeField] protected float currentHealth;
    protected float damageMultiplier;
    protected float speedMultiplier;
    [SerializeField] protected int currentShieldPoints;
    [SerializeField] protected float currentShieldHealth;
    [Header("Behaviour")]
    protected bool isOnFightMode;
    protected int currentWayPoint;
    protected float distanceToWayPoint;
    protected List<Transform> wayPoints = new List<Transform>();
    protected GameObject currentTarget;

    public virtual void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    public virtual void OnSpawn()
    {
        if (enemySO == null || enemyManager == null)
            Debug.LogError("No tiene enemySO o no tiene enemyManager");
        else
        {
            currentHealth = enemySO.health;
            damageMultiplier = 1f;
            speedMultiplier = 1f;
            currentShieldPoints = enemySO.shieldPoints;
            currentShieldHealth = currentHealth;
            isOnFightMode = false;
            currentWayPoint = 1;
            //wayPoints = enemyManager.EnemyWayPoints;
            currentTarget = null;
        }
    }

    public virtual void Update()
    {
        if (isOnFightMode)
            Attack();
        else
            Move();
    }

    public virtual void OnDeath()
    {
        Debug.Log("Me mori");
    }

    public virtual void TakeDamage(float damage, DefensesSO.DebuffType debuff, bool ignoreShield)
    {
        if (currentShieldPoints > 0 && !ignoreShield)
        {
            currentShieldHealth -= damage;
            if (currentShieldHealth <= 0f) 
            {
                currentShieldPoints--;
                if (currentShieldPoints > 0)
                    currentShieldHealth = enemySO.health;
            }
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0f)
                OnDeath();
        }
    }

    public virtual void Attack()
    {
        /* Solo puede atacar o moverse, no puede hacer las 2 al mismo tiempo, en teoria siempre estara caminando, pero cuando se llame
         * a una funcion del attackRange o detectionRange (este sera otro script para un trigger enter), entonces va a entrar en modo combate y atacar*/
    }

    public virtual void Move()
    {
        if (currentTarget == null)
        {
            Vector3 targetPos = new Vector3(wayPoints[currentWayPoint].position.x, transform.position.y, wayPoints[currentWayPoint].position.z);
            rb.Move(targetPos, wayPoints[currentWayPoint].rotation);
            distanceToWayPoint = Vector3.Distance(transform.position, targetPos);
            if (distanceToWayPoint <= 0.5f)
            {
                currentWayPoint++;
                if (currentWayPoint >= wayPoints.Count)
                    Debug.Log("Esto no deberia pasar en un juego real, pero el enemigo logro llegar a la meta");
            }
        }
        else
        {
            Vector3 targetPos = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
            rb.MovePosition(targetPos);
            transform.LookAt(targetPos);
        }
    }

    public virtual void OnTargetEnteredAttackZone(GameObject target)
    {
        isOnFightMode = true;
        currentTarget = target;
    }

    public virtual void OnTargetLeftAttackZone(GameObject newTarget)
    {
        currentTarget = newTarget;
        if (!currentTarget)
            isOnFightMode = false;
    }

    public virtual (int wayPoint, float distance) GetDistanceToBase()
    {
        return (currentWayPoint, distanceToWayPoint);
    }
}
