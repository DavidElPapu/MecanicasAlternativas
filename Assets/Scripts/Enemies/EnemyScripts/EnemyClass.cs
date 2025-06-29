using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public EnemiesSO enemySO;
    public EnemyDetectionRangeScript attackDetectionRange, followDetectionRange;
    protected EnemySpawnManager enemyManager;
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

    protected virtual void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    public virtual void OnSpawn(EnemySpawnManager enemySpawnManager, List<Transform> myWayPoints)
    {
        if (enemySO == null)
            Debug.LogError("No tiene enemySO");
        else
        {
            enemyManager = enemySpawnManager;
            if (enemySO.playerFocus == EnemiesSO.FocusType.Ignore)
                attackDetectionRange.InitializeDetection(this, false, true);
            else
                attackDetectionRange.InitializeDetection(this, true, true);
            if (followDetectionRange != null)
                followDetectionRange.InitializeDetection(this, true, false);
            currentHealth = enemySO.health;
            damageMultiplier = 1f;
            speedMultiplier = 1f;
            currentShieldPoints = enemySO.shieldPoints;
            currentShieldHealth = currentHealth;
            isOnFightMode = false;
            currentWayPoint = 1;
            wayPoints = myWayPoints;
            currentTarget = null;
        }
    }

    protected virtual void Update()
    {
        if (isOnFightMode)
            Attack();
    }

    protected virtual void FixedUpdate()
    {
        if (!isOnFightMode)
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
        //tambien deberia a cada rato checar la vida de su objetivo, por si muere debe dejar de atacar y ademas llamar a su rango deteccion para sacarlo de la lista
    }

    public virtual void Move()
    {
        if (currentTarget == null)
        {
            Debug.Log(currentWayPoint);
            Vector3 targetPos = new Vector3(wayPoints[currentWayPoint].position.x, transform.position.y, wayPoints[currentWayPoint].position.z);
            Vector3 moveDirection = targetPos - transform.position;
            rb.AddForce(moveDirection.normalized * enemySO.speed, ForceMode.Force);
            transform.LookAt(targetPos);
            //Vector3 torqueAxis = Vector3.Cross(transform.forward, moveDirection.normalized);
            //rb.AddTorque(torqueAxis * 0.1f, ForceMode.Force);
            distanceToWayPoint = Vector3.Distance(transform.position, targetPos);
            if (distanceToWayPoint <= 0.5f)
            {
                currentWayPoint++;
                if (currentWayPoint >= wayPoints.Count)
                {
                    Debug.Log("Esto no deberia pasar en un juego real, pero el enemigo logro llegar a la meta");
                    currentWayPoint--;
                }
            }
        }
        else
        {
            Vector3 targetPos = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
            Vector3 moveDirection = targetPos - transform.position;
            rb.AddForce(moveDirection.normalized * enemySO.speed, ForceMode.Force);
            transform.LookAt(targetPos);
        }
    }

    public virtual void OnTargetEnteredAttackZone(GameObject target, bool isPlayer)
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
