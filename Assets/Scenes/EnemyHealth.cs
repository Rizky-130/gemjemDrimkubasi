using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 3;
    private int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log(gameObject.name + " took damage. HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}