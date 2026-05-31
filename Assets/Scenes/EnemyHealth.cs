using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public int maxHP = 3;
    public int currentHP;

    private bool isDead = false;
    private EnemyHitEffect hitEffect;

    private void Awake()
    {
        hitEffect = GetComponent<EnemyHitEffect>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

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
        isDead = true;

        Debug.Log(gameObject.name + " died.");

        Destroy(gameObject);
    }
}