using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour {
    [SerializeField] private ActionHandler actionHandler;
    [SerializeField] private PlayerStatsObject stats;
    [Space, SerializeField] private int watchlistThreshold = 50;

    private int Aggression {
        get => stats.aggression;
        set => stats.aggression = value;
    }

    private void Start() {
        actionHandler.OnActionFinished.AddListener(OnActionFinished);
    }

    private void OnActionFinished(SerializedAction action, bool accepted) {
        MoveAggression(stats.passiveSuspicion);
        
        if (!accepted) return;

        // Active
        MoveAggression(action.AggressionGained);
        stats.workerEfficiency += action.WorkerEfficiency;

        if (action.IsPassive) {
            stats.passiveMoney += action.CleanMoneyAdded;
            stats.passiveLaundering += action.DirtyMoneyAdded;
            
            // Clamp the passive money and laundering to positive values only
            stats.passiveMoney = Mathf.Max(0, stats.passiveMoney);
            stats.passiveLaundering = Mathf.Max(0, stats.passiveLaundering);
        }
        else {
            stats.cleanMoney += action.CleanMoneyAdded;
            stats.dirtyMoney += action.DirtyMoneyAdded;
        }

        float avgEfficiency = (stats.workerEfficiency + stats.workerHappiness) / 2;  
        int dirtyMoney = stats.dirtyMoney,
            launderingPerAction = (int)(stats.passiveLaundering * avgEfficiency),
            moneyPerAction = (int)(stats.passiveMoney * avgEfficiency);
            
        // Passive
        stats.cleanMoney += moneyPerAction;
            
        int launderedMoneyThisTurn = dirtyMoney - Mathf.Max(0, dirtyMoney - launderingPerAction);
        stats.dirtyMoney -= launderedMoneyThisTurn;
        stats.cleanMoney += launderedMoneyThisTurn;

        int passiveBonus = moneyPerAction - stats.passiveMoney;
        int launderingBonus = launderingPerAction - stats.passiveLaundering;
        UI.Instance.UpdateStatsWithBonus(stats.cleanMoney, stats.dirtyMoney, stats.passiveMoney, passiveBonus, stats.passiveLaundering, launderingBonus);
    }

    public void MoveAggression(int value) {
        Aggression += value;
        Aggression = Mathf.Clamp(Aggression, 0, 100);

        UI.Instance.UpdateAggression(Aggression);
        if (Aggression >= 100) SceneManager.LoadScene(2);
    }

    public bool IsOnWatchlist() => Aggression >= watchlistThreshold;
}