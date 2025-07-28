using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public abstract class EnemyClass : MonoBehaviour
{
    public EnemiesSO enemySO;
    public EnemyDetectionRangeScript attackDetectionRange;
    public float attackRange;
    public event Action<GameObject> EnemyDeath;
    protected EnemySpawnManager enemyManager;
    protected Rigidbody rb; 
    [Header("Stats")]
    protected float currentHealth;
    protected bool isStunned;
    protected float damageMultiplier;
    protected float speedMultiplier;
    protected float speedDebuff, damageDebuff, poisonDebuff;
    [SerializeField] protected int currentShieldPoints;
    [SerializeField] protected float currentShieldHealth;
    [Header("Behaviour")]
    protected bool isOnFightMode;
    protected int currentWayPoint;
    protected float distanceToWayPoint, attackCooldownCount;
    protected List<Transform> wayPoints = new List<Transform>();
    protected GameObject currentTarget;
    protected List<GameObject> targetsInRange = new List<GameObject>();
    [Header("UI")]
    public Transform enemyCanvas;
    public Slider enemyHealthSlider, enemyShieldHealthSlider;
    public TextMeshProUGUI enemyHealthText, enemyShieldPointsText;

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
            attackDetectionRange.InitializeDetection(this);
            currentHealth = enemySO.health;
            isStunned = false;
            damageMultiplier = 1f;
            speedMultiplier = 1f;
            speedDebuff = 1f;
            damageDebuff = 1f;
            poisonDebuff = 0f;
            currentShieldPoints = enemySO.shieldPoints;
            if (currentShieldPoints > 0)
                currentShieldHealth = currentHealth;
            else
                currentShieldHealth = 0f;
            isOnFightMode = false;
            currentWayPoint = 1;
            attackCooldownCount = enemySO.attackCooldown;
            wayPoints = myWayPoints;
            currentTarget = null;
            enemyHealthSlider.maxValue = currentHealth;
            enemyHealthSlider.value = currentHealth;
            enemyHealthText.text = currentHealth.ToString();
            enemyShieldHealthSlider.maxValue = currentHealth;
            enemyShieldHealthSlider.value = currentShieldHealth;
            enemyShieldPointsText.text = currentShieldPoints.ToString();
        }
    }

    protected virtual void Update()
    {
        if (isOnFightMode && !isStunned)
            Attack();
        MoveCanvasToCamera();
    }

    protected virtual void FixedUpdate()
    {
        if (!isOnFightMode && !isStunned)
            Move();
    }

    public virtual void OnDeath()
    {
        EnemyDeath?.Invoke(gameObject);
    }

    public virtual void TakeDamage(float damage, GameObject damageDealer)
    {
        if (damageDealer == null)
        {
            //fue el jugador
        }
        else if (damageDealer.CompareTag("Defense"))
        {
            DefenseClass defenseScript = damageDealer.GetComponent<DefenseClass>();
            DefensesSO defenseData = defenseScript.defenseLevels[defenseScript.GetCurrentLevel()];
            if (defenseData.appliedDebuffs.Count > 0)
            {
                for (int i = 0; i < defenseData.appliedDebuffs.Count; i++)
                {
                    GetDebuff(defenseData.appliedDebuffs[i], defenseData.debuffsIntensities[i], defenseData.debuffsTimes[i]);
                }
            }
        }
        if (currentShieldPoints > 0)
        {
            currentShieldHealth -= damage;
            if (currentShieldHealth <= 0f) 
            {
                currentShieldPoints--;
                if (currentShieldPoints > 0)
                    currentShieldHealth = enemySO.health;
            }
            enemyShieldHealthSlider.value = currentShieldHealth;
            enemyShieldPointsText.text = currentShieldPoints.ToString();
        }
        else
        {
            currentHealth -= damage;
            enemyHealthSlider.value = currentHealth;
            enemyHealthText.text = currentHealth.ToString();
            if (currentHealth <= 0f)
                OnDeath();
        }
    }

    protected virtual void GetDebuff(DefensesSO.DebuffType debuff, float intensity, float duration)
    {
        switch (debuff)
        {
            case DefensesSO.DebuffType.Stun:
                isStunned = true;
                if (IsInvoking("StopStun"))
                    CancelInvoke("StopStun");
                Invoke("StopStun", duration);
                break;
            case DefensesSO.DebuffType.Slow:
                if (speedDebuff > intensity)
                    speedDebuff = intensity;
                if (IsInvoking("StopSlow"))
                    CancelInvoke("StopSlow");
                Invoke("StopSlow", duration);
                break;
            case DefensesSO.DebuffType.Poison:
                if (poisonDebuff < intensity)
                {
                    poisonDebuff = intensity;
                    if (IsInvoking("ApplyPoison"))
                        CancelInvoke("ApplyPoison");
                    //Por ahora todos los venenos aplican daño cada 0.8 segundos, lo que cambia es el daño
                    InvokeRepeating("ApplyPoison", 0.8f, 0.8f);
                }
                if (IsInvoking("StopPoison"))
                    CancelInvoke("StopPoison");
                Invoke("StopPoison", duration);
                break;
            case DefensesSO.DebuffType.Weakness:
                if (damageDebuff > intensity)
                    damageDebuff = intensity;
                if (IsInvoking("StopWeakness"))
                    CancelInvoke("StopWeakness");
                Invoke("StopWeakness", duration);
                break;
            default:
                break;
        }
    }

    protected virtual void StopStun()
    {
        isStunned = false;
    }

    protected virtual void StopSlow()
    {
        speedDebuff = 1f;
    }

    protected virtual void ApplyPoison()
    {
        currentHealth -= poisonDebuff;
        enemyHealthSlider.value = currentHealth;
        enemyHealthText.text = currentHealth.ToString();
        if (currentHealth <= 0f)
            OnDeath();
    }

    protected virtual void StopPoison()
    {
        poisonDebuff = 0f;
        if (IsInvoking("ApplyPoison"))
            CancelInvoke("ApplyPoison");
    }

    protected virtual void StopWeakness()
    {
        damageDebuff = 1f;
    }

    public virtual void Attack()
    {
        if (attackCooldownCount > 0)
        {
            attackCooldownCount -= Time.deltaTime;
            if(attackCooldownCount <= 0f)
            {
                attackCooldownCount = enemySO.attackCooldown;
                Vector3 rayDirection = currentTarget.transform.position - transform.position;
                foreach (RaycastHit hit in Physics.RaycastAll(transform.position, rayDirection.normalized, attackRange))
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        hit.collider.gameObject.GetComponent<PlayerStatus>().TakeDamage(enemySO.damage * damageMultiplier * damageDebuff);
                    }
                    else if (hit.collider.gameObject.CompareTag("Defense"))
                    {
                        if (hit.collider.gameObject.TryGetComponent(out BreakableDefenseClass breakableDefenseScript))
                        {
                            breakableDefenseScript.OnDamaged(enemySO.damage * damageMultiplier * damageDebuff);
                        }
                    }
                }
            }
        }
    }

    public virtual void Move()
    {
        if (currentTarget == null)
        {
            Vector3 targetPos = new Vector3(wayPoints[currentWayPoint].position.x, transform.position.y, wayPoints[currentWayPoint].position.z);
            Vector3 moveDirection = targetPos - transform.position;
            rb.AddForce(moveDirection.normalized * enemySO.speed * speedMultiplier * speedDebuff, ForceMode.Force);
            transform.LookAt(targetPos);
            //Vector3 torqueAxis = Vector3.Cross(transform.forward, moveDirection.normalized);
            //rb.AddTorque(torqueAxis * 0.1f, ForceMode.Force);
            distanceToWayPoint = Vector3.Distance(transform.position, targetPos);
            if (distanceToWayPoint <= 0.5f)
            {
                currentWayPoint++;
                if (currentWayPoint >= wayPoints.Count)
                {
                    //Debug.Log("Esto no deberia pasar en un juego real, pero el enemigo logro llegar a la meta");
                    currentWayPoint--;
                }
            }
        }
        //else
        //{
        //    Vector3 targetPos = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
        //    Vector3 moveDirection = targetPos - transform.position;
        //    rb.AddForce(moveDirection.normalized * enemySO.speed, ForceMode.Force);
        //    transform.LookAt(targetPos);
        //}
    }

    public virtual void OnTargetEnteredAttackZone(GameObject target, bool isPlayer)
    {
        if (isPlayer)
        {
            if (enemySO.playerFocus == EnemiesSO.FocusType.Ignore)
                return;
            targetsInRange.Add(target);
            PlayerStatus.PlayerDeath += OnPlayerDeath;
            currentTarget = target;
            if (!isOnFightMode)
            {
                isOnFightMode = true;
                attackCooldownCount = enemySO.attackCooldown;
            }
        }
        else
        {
            if (target.GetComponent<DefenseClass>().CanBeAttackedByEnemy())
            {
                if (target.TryGetComponent(out BreakableDefenseClass targetDefenseScript))
                {
                    targetsInRange.Add(target);
                    targetDefenseScript.DefenseBroken += OnDefenseBroke;
                    if (!isOnFightMode)
                    {
                        isOnFightMode = true;
                        currentTarget = target;
                        attackCooldownCount = enemySO.attackCooldown;
                    }
                }
                else
                    Debug.LogError("Se intenta atacar una defensa que no hereda de breakable defense, osea que no puede ser atacada");
            }
        }
    }

    public virtual void OnTargetLeftAttackZone(GameObject goneTarget, bool isPlayer)
    {
        if (targetsInRange.Contains(goneTarget))
        {
            targetsInRange.Remove(goneTarget);
            if (isPlayer)
                PlayerStatus.PlayerDeath -= OnPlayerDeath;
            else
                goneTarget.GetComponent<BreakableDefenseClass>().DefenseBroken -= OnDefenseBroke;
            if (currentTarget == goneTarget)
            {
                if (targetsInRange.Count > 0)
                    currentTarget = targetsInRange[0];
                else
                {
                    currentTarget = null;
                    isOnFightMode = false;
                }
            }
        }
    }

    protected virtual void OnPlayerDeath()
    {
        OnTargetLeftAttackZone(currentTarget, true);
    }

    protected virtual void OnDefenseBroke(GameObject defense)
    {
        OnTargetLeftAttackZone(defense, false);
    }

    public virtual (int wayPoint, float distance) GetDistanceToBase()
    {
        return (currentWayPoint, distanceToWayPoint);
    }

    public virtual float GetStrenght()
    {
        return (enemySO.damage * damageMultiplier * damageDebuff);
    }

    public virtual float GetHealth()
    {
        return currentHealth;
    }

    protected virtual void MoveCanvasToCamera()
    {
        enemyCanvas.LookAt(Camera.main.transform.position);
    }
}
