using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Target")]
    public string towerTag = "Tower";
    private Transform towerTarget;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float attackRange = 5f;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 6f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    private float attackTimer;

    [Header("Aiming")]
    public bool rotateToTarget = true;

    private void Start()
    {
        FindTower();
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        if (towerTarget == null)
        {
            FindTower();
            return;
        }

        float distance = Vector2.Distance(transform.position, towerTarget.position);

        if (rotateToTarget)
        {
            RotateTowardTower();
        }

        if (distance > attackRange)
        {
            MoveTowardTower();
        }
        else
        {
            ShootTower();
        }
    }

    private void FindTower()
    {
        GameObject towerObject = GameObject.FindGameObjectWithTag(towerTag);

        if (towerObject == null)
        {
            Debug.LogWarning("Ranged enemy cannot find object with tag: " + towerTag);
            return;
        }

        towerTarget = towerObject.transform;
    }

    private void MoveTowardTower()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            towerTarget.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void RotateTowardTower()
    {
        if (towerTarget == null)
            return;

        Vector2 direction = towerTarget.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootTower()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            return;

        if (enemyBulletPrefab == null)
        {
            Debug.LogError(gameObject.name + " has no enemy bullet prefab assigned!");
            attackTimer = attackCooldown;
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError(gameObject.name + " has no fire point assigned!");
            attackTimer = attackCooldown;
            return;
        }

        GameObject bullet = Instantiate(
            enemyBulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            enemyBullet.damage = attackDamage;
            enemyBullet.speed = bulletSpeed;
            enemyBullet.SetTarget(towerTarget);
        }

        Debug.Log(gameObject.name + " shot tower for " + attackDamage + " damage!");

        attackTimer = attackCooldown;
    }
}