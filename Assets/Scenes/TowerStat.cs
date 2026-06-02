using System.Collections.Generic;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public static TowerStats Instance;

    [Header("Base Tower Stats")]
    public int baseDamage = 10;
    public float baseFireRate = 1.0f; // Shots per second
    public int baseMaxHP = 100;
    public int baseHM = 0;

    [Header("Current Tower Stats")]
    public int damage;
    public float fireRate;
    public int maxHP;
    public int currentHP;
    public int dreamHormone;

    [Header("Ending Requirement")]
    public int goodEndingMinHM = 50;
    public int goodEndingMaxHM = 100;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        maxHP = baseMaxHP;
        currentHP = maxHP;
        dreamHormone = baseHM;

        DebugStats();
    }

    public void RecalculateStats(List<InventoryItem> activeItems)
    {
        int oldMaxHP = maxHP;

        damage = baseDamage;
        fireRate = baseFireRate;
        maxHP = baseMaxHP;
        dreamHormone = baseHM;

        if (activeItems != null)
        {
            foreach (InventoryItem item in activeItems)
            {
                if (item == null)
                    continue;

                damage += item.stats.damageBonus;
                fireRate += item.stats.fireRateBonus;
                maxHP += item.stats.hpBonus;
                dreamHormone += item.stats.hmBonus;
            }
        }

        // If max HP increases, current HP increases by the difference.
        int hpDifference = maxHP - oldMaxHP;
        currentHP += hpDifference;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        DebugStats();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log("Tower took damage! HP: " + currentHP + " / " + maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Tower destroyed! Bad Ending.");
        Destroy(gameObject);
    }

    public bool IsGoodEnding()
    {
        return dreamHormone > goodEndingMinHM && dreamHormone < goodEndingMaxHM;
    }

    public void DebugStats()
    {
        Debug.Log(
            "Tower Stats | Damage: " + damage +
            " | Fire Rate: " + fireRate +
            " | HP: " + currentHP + "/" + maxHP +
            " | HM: " + dreamHormone
        );
    }
}