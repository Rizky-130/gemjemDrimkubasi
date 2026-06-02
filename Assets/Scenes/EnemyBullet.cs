using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 1;
    public float speed = 6f;
    public float lifeTime = 5f;

    private Transform target;
    private Vector2 moveDirection;
    private bool hasHit = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        moveDirection = (target.position - transform.position).normalized;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
            return;

        TurretHealth turretHealth = collision.GetComponent<TurretHealth>();

        if (turretHealth == null)
            turretHealth = collision.GetComponentInParent<TurretHealth>();

        if (turretHealth == null)
            turretHealth = collision.GetComponentInChildren<TurretHealth>();

        if (turretHealth != null)
        {
            hasHit = true;

            turretHealth.TakeDamage(damage);

            Debug.Log("Enemy bullet hit tower for " + damage + " damage!");

            Destroy(gameObject);
        }
    }
}