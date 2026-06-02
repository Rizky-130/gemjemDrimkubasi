using TMPro;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI Instance;

    public GameObject panel;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI tierText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI hmText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(InventoryItem item)
    {
        panel.SetActive(true);

        nameText.text = item.itemName;
        tierText.text = "Tier :"+item.tier.ToString();
        damageText.text = "Damage :"+item.damage.ToString();
        fireRateText.text = "Fire rate :"+item.fireRate.ToString("0.0");
        hpText.text = " Hp :"+item.hpTurret.ToString();
        hmText.text = "HM :"+item.hm.ToString();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}