using UnityEngine;

public class TurretHealth : MonoBehaviour
{
    [Header("Turret Health")]
    public int maxHP = 20;
    public int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log("I took damage! HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Tower destroyed! Good job dumbass.");
        Destroy(gameObject);
    }
}