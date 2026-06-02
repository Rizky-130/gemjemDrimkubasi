using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    [Header("Target")]
    public string towerTag = "Tower";
    private Transform towerTarget;
    private TurretHealth turretHealth;

    [Header("Tank Movement")]
    public float moveSpeed = 0.5f;
    public float stopDistance = 1.0f;

    [Header("Tank Attack")]
    public int attackDamage = 20;
    public float attackCooldown = 2.5f;

    [Header("Tank Scaling")]
    public float hpMultiplier = 2.5f;
    public float damageMultiplier = 1.25f;

    private float attackTimer;

    private void Start()
    {
        FindTower();
    }

    private void Update()
    {
        if (towerTarget == null)
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
            Debug.LogError("Tower was found, but it has no TurretHealth script!");
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
                Debug.Log(gameObject.name + " tank attacked tower for " + attackDamage + " damage!");
            }

            attackTimer = attackCooldown;
        }
    }
}