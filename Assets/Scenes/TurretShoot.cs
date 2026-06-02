using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Tooltip("Optional. If empty, this object will rotate instead.")]
    public Transform turretPivot;

    [Header("Shooting")]
    public float bulletSpeed = 12f;

    [Header("Fallback Stats If TowerStats Missing")]
    public int fallbackDamage = 10;

    [Tooltip("Shots per second. 1 = one shot per second, 2 = two shots per second.")]
    public float fallbackFireRate = 1f;

    [Header("Aiming")]
    public bool rotateToMouse = true;

    private Camera mainCamera;
    private float nextFireTime;

    private void Start()
    {
        mainCamera = Camera.main;

        if (turretPivot == null)
        {
            turretPivot = transform;
        }
    }

    private void Update()
    {
        if (GameUIManager.Instance != null)
        {
            if (!GameUIManager.Instance.CanPlayerAct())
                return;
        }

        if (rotateToMouse)
        {
            RotateToMouse();
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();

            float currentFireRate = GetCurrentFireRate();
            float cooldown = 1f / currentFireRate;

            nextFireTime = Time.time + cooldown;
        }
    }

    private void RotateToMouse()
    {
        if (mainCamera == null)
            return;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        Vector2 direction = mouseWorldPosition - turretPivot.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        turretPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab is missing!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("Fire Point is missing!");
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.damage = GetCurrentDamage();
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = firePoint.right * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet has no Rigidbody2D.");
        }

        Debug.Log("Tower fired! Damage: " + GetCurrentDamage() + " | Fire Rate: " + GetCurrentFireRate());
    }

    private int GetCurrentDamage()
    {
        if (TowerStats.Instance != null)
        {
            return TowerStats.Instance.damage;
        }

        return fallbackDamage;
    }

    private float GetCurrentFireRate()
    {
        if (TowerStats.Instance != null)
        {
            return Mathf.Max(0.1f, TowerStats.Instance.fireRate);
        }

        return Mathf.Max(0.1f, fallbackFireRate);
    }
}