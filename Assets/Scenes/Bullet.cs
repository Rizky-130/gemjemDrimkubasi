using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 1;
    public float lifeTime = 5f;

    [Header("Out of Bounds")]
    public float extraScreenMargin = 0.1f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Backup destroy timer, just in case
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
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}