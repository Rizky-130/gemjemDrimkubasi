using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 1;
    public float lifeTime = 5f;

    [Header("Out of Bounds")]
    public float extraScreenMargin = 0.1f;

    private Camera mainCamera;
    private bool hasHit = false;

    private void Start()
    {
        mainCamera = Camera.main;

        // Backup destroy timer
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        DestroyIfOutOfBounds();
    }

    private void DestroyIfOutOfBounds()
    {
        if (mainCamera == null)
            return;

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        bool isOutOfBounds =
            viewportPosition.x < -extraScreenMargin ||
            viewportPosition.x > 1f + extraScreenMargin ||
            viewportPosition.y < -extraScreenMargin ||
            viewportPosition.y > 1f + extraScreenMargin;

        if (isOutOfBounds)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
            return;

        // 1. If bullet hits HM block drop, collect it into TempStorage
        HMBlockWorldDropper hmBlock = collision.GetComponentInParent<HMBlockWorldDropper>();

        if (hmBlock != null)
        {
            hasHit = true;

            hmBlock.CollectToTempStorage();

            Destroy(gameObject);
            return;
        }

        // 2. If bullet hits enemy, damage enemy
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            hasHit = true;

            enemy.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }
    }
}