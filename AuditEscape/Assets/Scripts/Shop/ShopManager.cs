using System;
using System.Threading.Tasks;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    private const int UPGRADE_THRESHOLD = 1;

    private enum ShopItem {
        AuditDifficulty, WorkerEfficiency, WorkerHappiness
    }

    [SerializeField] private PlayerStatsObject playerStats;
    [SerializeField] private GameObject shopWindow;

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
        
        switch ((ShopItem)item) {
            case ShopItem.AuditDifficulty:
                playerStats.auditDifficultyDecrease += UPGRADE_THRESHOLD;
                break;
            case ShopItem.WorkerEfficiency:
                playerStats.workerEfficiency += UPGRADE_THRESHOLD;
                break;
            case ShopItem.WorkerHappiness:
                playerStats.workerHappiness += UPGRADE_THRESHOLD;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item), item, null);
        }
    }
}
