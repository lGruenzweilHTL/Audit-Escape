using UnityEngine;
using UnityEngine.Events;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject auditGame;

    public UnityEvent<GameAction, bool> OnActionFinished;

    private GameAction currentAction;
    private GameObject actionObject;

    private void Start()
    {
        NextAction();
    }

    public void Continue(bool accepted)
    {
        OnActionFinished?.Invoke(currentAction, accepted);

        if (currentAction.Type == ActionType.Audit) auditGame.SetActive(true);
        NextAction();
    }
    private void NextAction()
    {
        if (actionObject != null) Destroy(actionObject);

        var obj = Instantiate(actionPrefab, cardParent);

        currentAction = GetWeightedAction(player.aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }

    private GameAction GetWeightedAction(int aggression)
    {
        int value = Random.Range(0, 90 + aggression / 5);

        if (value == 0) return new() { Type = ActionType.LaunderAll, Ammount = 0 };
        if (value <= 30) return new() { Type = ActionType.GetMoney, Ammount = 100 };
        else if (value <= 50) return new() { Type = ActionType.LaunderMoney, Ammount = 1000 };
        else if (value <= 70) return new() { Type = ActionType.GetWorker, Ammount = 10 };
        else if (value <= 90) return new() { Type = ActionType.GetLaunderer, Ammount = 100 };
        else return new() { Type = ActionType.Audit, Ammount = 0 };
    }
}