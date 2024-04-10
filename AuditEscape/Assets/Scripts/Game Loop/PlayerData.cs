using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private ActionHandler actionHandler;
    [SerializeField] private UI ui;

    public int cleanMoney { get; private set; }
    public int dirtyMoney { get; private set; }

    public int moneyPerAction { get; private set; }
    public int launderingPerAction { get; private set; }
    private int aggression;
    public int Aggression
    {
        get => aggression;
        private set
        {
            aggression = value;
            ui.UpdateAggression(aggression);
            if (aggression >= 100) SceneManager.LoadScene(2);
        }
    }

    private void Start()
    {
        //throw new System.NotImplementedException();
        actionHandler.OnActionFinished.AddListener((action, accepted) =>
        {
            if (!accepted) return;

            // Active
            Aggression += action.AggressionGained;
            if (action.IsPassive)
            {
                moneyPerAction += action.CleanMoneyAdded;
                launderingPerAction += action.DirtyMoneyAdded;
            }
            else
            {
                cleanMoney += action.CleanMoneyAdded;
                dirtyMoney += action.DirtyMoneyAdded;
            }


            // Passive
            int launderedMoneyThisTurn = dirtyMoney - Mathf.Max(0, dirtyMoney - launderingPerAction);
            dirtyMoney -= launderedMoneyThisTurn;
            cleanMoney += launderedMoneyThisTurn;

            cleanMoney += moneyPerAction;

            ui.UpdateStats(cleanMoney, dirtyMoney, moneyPerAction, launderingPerAction);
        });
    }

    public void MoveAggression(int value) => Aggression += value;
    public bool IsOnWatchlist() => Aggression >= 80;
}