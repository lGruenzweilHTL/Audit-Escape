using TMPro;
using UnityEngine;

public class DebugView : MonoBehaviour {
    [SerializeField] private PlayerStatsObject player;
    [SerializeField] private GameObject background;
    [SerializeField] private TMP_Text text;

    [Space, SerializeField] private ActionHandler actionHandler;
    [SerializeField] private ShopManager shop;
    [SerializeField] private PlayerData playerData;
    
    private void Update() {
        if (!Input.GetKeyDown(KeyCode.I)) return;
        
        bool shouldBeEnabled = !background.activeSelf;
        ToggleBackground();
        if (shouldBeEnabled) OnBackgroundEnabled();
    }
    
    public void ToggleBackground() => background.SetActive(!background.activeSelf);

    public void OnBackgroundEnabled() {
        text.text = $"Clean money: {player.cleanMoney}\nDirty money: {player.dirtyMoney}\n" +
                    $"Passive money: {player.passiveMoney}\nPassive laundering: {player.passiveLaundering}\n\n" +
                    $"Worker efficiency: {player.workerEfficiency}\nWorker happiness: {player.workerHappiness}\nCombined: {(player.workerEfficiency + player.workerHappiness) / 2}\n\n" +
                    $"Suspicion: {player.aggression}\nWatchlist: {playerData.IsOnWatchlist()}\nAudit difficulty decrease: {player.auditDifficultyDecrease}\nChance of audit: {actionHandler.GetAuditChance(player.aggression)}\n\n" +
                    $"Shop prices: {shop.DiscountMultiplier * 100}%";
    }
}
