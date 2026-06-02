using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Shooting")]
    public float bulletSpeed = 12f;

    [Header("Fallback Stats If TowerStats Missing")]
    public int fallbackDamage = 10;
    public float fallbackFireRate = 1f;

    private float nextFireTime;

    private void Update()
    {
        if (GameUIManager.Instance != null)
        {
            if (!GameUIManager.Instance.CanPlayerAct())
                return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();

            float currentFireRate = GetCurrentFireRate();
            float cooldown = 1f / currentFireRate;

            nextFireTime = Time.time + cooldown;
        }
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

        if (GameSFXManager.Instance != null)
        {
            GameSFXManager.Instance.PlayShoot();
        }

        Debug.Log("Tower fired! Damage: " + GetCurrentDamage());
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