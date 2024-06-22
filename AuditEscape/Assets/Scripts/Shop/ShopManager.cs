using System.Threading.Tasks;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    [SerializeField] private PlayerStatsObject playerStats;
    [SerializeField] private GameObject shopWindow;

    [Header("Upgrade Thresholds"), SerializeField]
    private int auditDifficultyUpgrade = 1;
        [SerializeField] private int workerEfficiencyUpgrade = 1,
        workerHappinessUpgrade = 1,
        passiveSuspicionUpgrade = -1;

    public void ToggleShopWindow() {
        UI.Instance.UpdateStats(playerStats);
        bool active = !shopWindow.activeSelf;
        SetShopWindowActive(active);
    }

    private async void SetShopWindowActive(bool active) {
        await Task.Delay(200);
        shopWindow.SetActive(active);
    }

    public void UpgradeItem(int item, int cost) {
        if (playerStats.cleanMoney < cost) {
            Debug.Log($"Player tried to buy upgrade {item} for {cost} coins. They had {playerStats.cleanMoney} which is not enough");
            return;
        }

        playerStats.cleanMoney -= cost;
        
        switch (item) {
            case 0: // AuditDifficulty
                playerStats.auditDifficultyDecrease += auditDifficultyUpgrade;
                break;
            case 1: // WorkerEfficiency
                playerStats.workerEfficiency += workerEfficiencyUpgrade;
                break;
            case 2: // WorkerHappiness
                playerStats.workerHappiness += workerHappinessUpgrade;
                break;
            case 3: // PassiveSuspicion
                playerStats.passiveSuspicion += passiveSuspicionUpgrade;
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(item), item, null);
        }
    }
}
