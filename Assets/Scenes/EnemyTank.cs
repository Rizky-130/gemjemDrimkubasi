using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    [Header("Target")]
    public string towerTag = "Tower";
    private Transform towerTarget;
    private TurretHealth turretHealth;

    [Header("Tank Movement")]
    public float moveSpeed = 0.8f;

    [Tooltip("Tank should stop farther from tower because it is bigger.")]
    public float stopDistance = 2.3f;

    [Header("Tank Attack")]
    public int attackDamage = 10;
    public float attackCooldown = 2.5f;

    [Header("Wave Stat Multipliers")]
    public float hpMultiplier = 3f;
    public float damageMultiplier = 1.4f;

    private float attackTimer = 0f;

    private void Start()
    {
        FindTower();
    }

    private void Update()
    {
        if (towerTarget == null || turretHealth == null)
        {
            FindTower();
            return;
        }

        float distance = Vector2.Distance(transform.position, towerTarget.position);

        if (distance > stopDistance)
        {
            MoveTowardTower();
        }
        else
        {
            AttackTower();
        }
    }

    private void FindTower()
    {
        GameObject towerObject = GameObject.FindGameObjectWithTag(towerTag);

        if (towerObject == null)
        {
            Debug.LogWarning("Tank enemy cannot find object with tag: " + towerTag);
            return;
        }

        towerTarget = towerObject.transform;

        turretHealth = towerObject.GetComponent<TurretHealth>();

        if (turretHealth == null)
            turretHealth = towerObject.GetComponentInParent<TurretHealth>();

        if (turretHealth == null)
            turretHealth = towerObject.GetComponentInChildren<TurretHealth>();

        if (turretHealth == null)
        {
            Debug.LogError("Tower found, but TurretHealth is missing!");
        }
    }

    private void MoveTowardTower()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            towerTarget.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void AttackTower()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (turretHealth != null)
            {
                turretHealth.TakeDamage(attackDamage);
                Debug.Log(gameObject.name + " tank attacked tower for " + attackDamage + " damage.");
            }

            attackTimer = attackCooldown;
        }
    }
}