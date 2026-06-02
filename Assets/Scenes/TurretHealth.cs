using UnityEngine;
using UnityEngine.SceneManagement;

public class TurretHealth : MonoBehaviour
{
    [Header("Fallback Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Hitbox")]
    public Collider2D hitboxCollider;

    [Header("Death / Game Over")]
    public bool goToMainMenuOnDeath = true;
    public string mainMenuSceneName = "MainMenu";

    [Header("Destroy Settings")]
    public GameObject objectToDestroy;

    private TowerHitEffect hitEffect;
    private bool isDead = false;

    private void Awake()
    {
        hitEffect = GetComponent<TowerHitEffect>();

        if (hitEffect == null)
            hitEffect = GetComponentInChildren<TowerHitEffect>();

        if (hitboxCollider == null)
            hitboxCollider = GetComponentInChildren<Collider2D>();

        if (objectToDestroy == null)
            objectToDestroy = gameObject;
    }

    private void Start()
    {
        if (TowerStats.Instance != null)
        {
            maxHP = TowerStats.Instance.maxHP;
            currentHP = TowerStats.Instance.currentHP;
        }
        else
        {
            currentHP = maxHP;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (damage <= 0)
            return;

        if (GameSFXManager.Instance != null)
        {
            GameSFXManager.Instance.PlayTowerHit();
        }

        if (hitEffect != null)
        {
            hitEffect.PlayHitEffect();
        }

        if (TowerStats.Instance != null)
        {
            TowerStats.Instance.TakeDamage(damage);

            maxHP = TowerStats.Instance.maxHP;
            currentHP = TowerStats.Instance.currentHP;

            Debug.Log("Tower took damage! HP: " + currentHP + " / " + maxHP);

            if (currentHP <= 0)
            {
                Die();
            }

            return;
        }

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log("Tower took damage! HP: " + currentHP + " / " + maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead)
            return;

        if (healAmount <= 0)
            return;

        if (TowerStats.Instance != null)
        {
            TowerStats.Instance.currentHP += healAmount;
            TowerStats.Instance.currentHP = Mathf.Clamp(
                TowerStats.Instance.currentHP,
                0,
                TowerStats.Instance.maxHP
            );

            maxHP = TowerStats.Instance.maxHP;
            currentHP = TowerStats.Instance.currentHP;

            return;
        }

        currentHP += healAmount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Tower destroyed! Returning to Main Menu.");

        Time.timeScale = 1f;

        if (goToMainMenuOnDeath)
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}