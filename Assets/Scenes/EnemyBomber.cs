using UnityEngine;

public class EnemyBomber : MonoBehaviour
{
    [Header("Target")]
    public string towerTag = "Tower";
    private Transform towerTarget;
    private TurretHealth turretHealth;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float explosionDistance = 0.9f;

    [Header("Explosion")]
    public int explosionDamage = 10;
    public float explosionDelay = 0.5f;
    public GameObject explosionEffectPrefab;

    private bool isExploding = false;
    private float explosionTimer;

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

        if (isExploding)
        {
            HandleExplosionCountdown();
            return;
        }

        float distance = Vector2.Distance(transform.position, towerTarget.position);

        if (distance > explosionDistance)
        {
            MoveTowardTower();
        }
        else
        {
            StartExplosion();
        }
    }

    private void FindTower()
    {
        GameObject towerObject = GameObject.FindGameObjectWithTag(towerTag);

        if (towerObject == null)
        {
            Debug.LogWarning("Bomber enemy cannot find object with tag: " + towerTag);
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

    private void StartExplosion()
    {
        if (isExploding)
            return;

        isExploding = true;
        explosionTimer = explosionDelay;

        Debug.Log(gameObject.name + " is about to explode!");
    }

    private void HandleExplosionCountdown()
    {
        explosionTimer -= Time.deltaTime;

        if (explosionTimer <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (turretHealth != null)
        {
            turretHealth.TakeDamage(explosionDamage);
            Debug.Log(gameObject.name + " exploded and damaged the tower!");
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}