using System.Threading.Tasks;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    [SerializeField] private PlayerStatsObject playerStats;
    [SerializeField] private GameObject shopWindow;

    [Header("Upgrade Thresholds"), SerializeField]
    private int auditDifficultyUpgrade = 1;

    [SerializeField] private float workerEfficiencyUpgrade = 1,
        workerHappinessUpgrade = 1;

    [SerializeField] private int passiveSuspicionUpgrade = -1,
        oneTimeSuspicionReduce = 25;

    public void ToggleShopWindow() {
        UI.Instance.UpdateStatsWithBonus(playerStats);
        bool active = !shopWindow.activeSelf;
        SetShopWindowActive(active);
    }

    private async void SetShopWindowActive(bool active) {
        await Task.Delay(200);
        shopWindow.SetActive(active);
    }

    // Returns: if the purchase was successful
    public bool UpgradeItem(int item, int cost, int currLevel, int maxLevel) {
        if (playerStats.cleanMoney < cost) {
            Debug.Log($"Player tried to buy upgrade {item} for {cost} coins. They had {playerStats.cleanMoney} which is not enough");
            return false;
        }

        // Check if the player has already reached the max level
        if (maxLevel != -1 && currLevel >= maxLevel) {
            Debug.Log($"Player tried to buy upgrade {item} for {cost} coins. They already reached the max level");
            return false;
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
                playerStats.passiveSuspicion = Mathf.Max(playerStats.passiveSuspicion, 1);
                break;
            case 4: // SuspicionDecrease (once)
                playerStats.aggression -= oneTimeSuspicionReduce;
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(item), item, null);
        }

        return true;
    }
}