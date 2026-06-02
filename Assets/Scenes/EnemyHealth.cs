using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public int maxHP = 3;
    public int currentHP;

    [Header("Drop Settings")]
    [Range(0f, 100f)]
    public float dropChance = 100f;

    private bool isDead = false;
    private EnemyHitEffect hitEffect;

    private void Awake()
    {
        hitEffect = GetComponent<EnemyHitEffect>();

        if (hitEffect == null)
            hitEffect = GetComponentInChildren<EnemyHitEffect>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (damage <= 0)
            return;

        currentHP -= damage;

        if (GameSFXManager.Instance != null)
        {
            GameSFXManager.Instance.PlayEnemyHit();
        }

        if (hitEffect != null)
        {
            hitEffect.PlayHitEffect();
        }

        Debug.Log(gameObject.name + " took damage! HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " died.");

        TryDropHMBlock();

        Destroy(gameObject);
    }

    private void TryDropHMBlock()
    {
        float roll = Random.Range(0f, 100f);

        if (roll > dropChance)
        {
            Debug.Log(gameObject.name + " dropped nothing.");
            return;
        }

        if (DropManager.Instance == null)
        {
            Debug.LogWarning("DropManager.Instance not found. Enemy cannot drop HM block.");
            return;
        }

        int currentWave = GetCurrentWave();

        DropManager.Instance.DropHMBlock(transform.position, currentWave);
    }

    private int GetCurrentWave()
    {
        WaveSpawner waveSpawner = FindObjectOfType<WaveSpawner>();

        if (waveSpawner == null)
        {
            Debug.LogWarning("WaveSpawner not found. Defaulting wave to 1.");
            return 1;
        }

        return waveSpawner.currWave;
    }
}