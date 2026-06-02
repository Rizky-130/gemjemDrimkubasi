using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 1;
    public float lifeTime = 5f;

    [Header("Sprite")]
    public Sprite normalSprite;
    public Sprite hitSprite;
    public float hitSpriteDuration = 0.1f;

    [Header("Face Direction")]
    public bool faceMoveDirection = true;

    [Tooltip("Use -90 if your bullet sprite points upward by default. Use 0 if it points right.")]
    public float angleOffset = -90f;

    [Header("Out of Bounds")]
    public float extraScreenMargin = 0.1f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D bulletCollider;

    private bool hasHit = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (hasHit)
            return;

        DestroyIfOutOfBounds();
    }

    private void LateUpdate()
    {
        if (hasHit)
            return;

        if (faceMoveDirection)
        {
            FaceDirection();
        }
    }

    private void FaceDirection()
    {
        if (rb == null)
            return;

        Vector2 direction = rb.velocity;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            angle + angleOffset
        );
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

        // Hit HM block drop
        HMBlockWorldDropper hmBlock = collision.GetComponentInParent<HMBlockWorldDropper>();

        if (hmBlock != null)
        {
            hasHit = true;

            hmBlock.CollectToTempStorage();

            Destroy(gameObject);
            return;
        }

        // Hit enemy
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            hasHit = true;

            enemy.TakeDamage(damage);

            StartCoroutine(PlayHitSpriteThenDestroy());
            return;
        }
    }

    private IEnumerator PlayHitSpriteThenDestroy()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (bulletCollider != null)
        {
            bulletCollider.enabled = false;
        }

        if (spriteRenderer != null && hitSprite != null)
        {
            spriteRenderer.sprite = hitSprite;
        }

        yield return new WaitForSeconds(hitSpriteDuration);

        Destroy(gameObject);
    }
}