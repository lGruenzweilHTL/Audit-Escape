using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private ActionHandler actionHandler;
    [SerializeField] private UI ui;

    public int cleanMoney { get; private set; }
    public int dirtyMoney { get; private set; }

    public int moneyPerAction { get; private set; }
    public int launderingPerAction { get; private set; }
    public int aggression { get; private set; }

    private void Start()
    {
        actionHandler.OnActionFinished.AddListener((action, accepted) =>
        {
            if (!accepted || action.Type == ActionType.Audit)
                return;

            // Active
            switch (action.Type)
            {
                case ActionType.GetMoney:
                    cleanMoney += action.Ammount;
                    break;
                case ActionType.LaunderMoney:
                    dirtyMoney += action.Ammount;
                    aggression += 5;
                    break;
                case ActionType.GetWorker:
                    moneyPerAction += action.Ammount;
                    break;
                case ActionType.GetLaunderer:
                    launderingPerAction += action.Ammount;
                    break;
                case ActionType.LaunderAll:
                    cleanMoney = dirtyMoney;
                    dirtyMoney = 0;
                    break;
            }

            // Passive
            int moneyLeftToLaunder = Mathf.Max(0, dirtyMoney - launderingPerAction);
            dirtyMoney -= moneyLeftToLaunder;
            cleanMoney += moneyLeftToLaunder;

            cleanMoney += moneyPerAction;

            ui.UpdateStats(cleanMoney, dirtyMoney, moneyPerAction, launderingPerAction);
            ui.UpdateAggression(aggression);
        });
    }
}