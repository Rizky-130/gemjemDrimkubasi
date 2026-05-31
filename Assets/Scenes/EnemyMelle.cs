using UnityEngine;

public class EnemyMelle : MonoBehaviour
{
    [Header("Target")]
    public string towerTag = "Tower";
    private Transform towerTarget;
    private TurretHealth turretHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 0.8f;

    [Header("Attack")]
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;

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
            Debug.LogWarning("Enemy cannot find object with tag: " + towerTag);
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
                Debug.Log(gameObject.name + " attacked the turret!");
            }

            attackTimer = attackCooldown;
        }
    }
}