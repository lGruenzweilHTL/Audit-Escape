using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour {
    [SerializeField] private ActionHandler actionHandler;
    [SerializeField] private UI ui;
    [SerializeField] private PlayerStatsObject stats;
    [Space, SerializeField] private int watchlistThreshold = 50;

    public int Aggression {
        get => stats.aggression;
        private set => stats.aggression = value;
    }

    private void Start() {
        actionHandler.OnActionFinished.AddListener(OnActionFinished);
    }

    private void OnActionFinished(SerializedAction action, bool accepted) {
        if (!accepted) return;

        // Active
        MoveAggression(action.AggressionGained);

        if (action.IsPassive) {
            stats.passiveMoney += action.CleanMoneyAdded;
            stats.passiveLaundering += action.DirtyMoneyAdded;
        }
        else {
            stats.cleanMoney += action.CleanMoneyAdded;
            stats.dirtyMoney += action.DirtyMoneyAdded;
        }

        int dirtyMoney = stats.dirtyMoney,
            launderingPerAction = stats.passiveLaundering * stats.workerEfficiency,
            moneyPerAction = stats.passiveMoney * stats.workerEfficiency;
            
        // Passive
        stats.cleanMoney += moneyPerAction;
            
        int launderedMoneyThisTurn = dirtyMoney - Mathf.Max(0, dirtyMoney - launderingPerAction);
        stats.dirtyMoney -= launderedMoneyThisTurn;
        stats.cleanMoney += launderedMoneyThisTurn;

        ui.UpdateStats(stats.cleanMoney, stats.dirtyMoney, moneyPerAction, launderingPerAction);
    }

    public void MoveAggression(int value) {
        Aggression += value;

        ui.UpdateAggression(Aggression);
        if (Aggression >= 100) SceneManager.LoadScene(2);
    }

    public bool IsOnWatchlist() => Aggression >= watchlistThreshold;
}