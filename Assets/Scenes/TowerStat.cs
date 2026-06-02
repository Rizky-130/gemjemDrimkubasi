using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public static TowerStats Instance;

    [Header("Base Tower Stats")]
    public int baseDamage = 10;
    public float baseFireRate = 1.0f;
    public int baseMaxHP = 100;
    public int baseHM = 0;

    [Header("Current Final Stats")]
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
        ResetToBaseStats();
        RefreshStatsFromInventory();
    }

    public void ResetToBaseStats()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        maxHP = baseMaxHP;
        currentHP = maxHP;
        dreamHormone = baseHM;
    }

    public void RefreshStatsFromInventory()
    {
        int oldMaxHP = maxHP;

        int bonusDamage = 0;
        float bonusFireRate = 0f;
        int bonusHP = 0;
        int bonusHM = 0;

        if (InventoryStats.Instance != null)
        {
            bonusDamage = InventoryStats.Instance.totalDamage;
            bonusFireRate = InventoryStats.Instance.totalFireRate;
            bonusHP = InventoryStats.Instance.totalHP;
            bonusHM = InventoryStats.Instance.totalHM;
        }

        damage = baseDamage + bonusDamage;
        fireRate = baseFireRate + bonusFireRate;
        maxHP = baseMaxHP + bonusHP;
        dreamHormone = baseHM + bonusHM;

        if (maxHP != oldMaxHP)
        {
            int hpDifference = maxHP - oldMaxHP;
            currentHP += hpDifference;
        }

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log(
            $"Tower Final Stats | DMG:{damage} | HP:{currentHP}/{maxHP} | FR:{fireRate} | HM:{dreamHormone}"
        );
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
}