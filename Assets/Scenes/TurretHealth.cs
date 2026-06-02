using UnityEngine;

public class TurretHealth : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        if (TowerStats.Instance == null)
        {
            Debug.LogError("TowerStats.Instance not found!");
            return;
        }

        TowerStats.Instance.TakeDamage(damage);
    }
}