using UnityEngine;
using UnityEngine.Events;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;

    public UnityEvent<GameAction, bool> OnActionFinished;

    private GameAction currentAction;
    private GameObject actionObject;

    private void Start()
    {
        NextAction();
    }

    public void Continue(bool accepted)
    {
        if (currentAction.Type != ActionType.Audit)
            NextAction();
        else
            Debug.Log("Audit");

        OnActionFinished?.Invoke(currentAction, accepted);
    }
    private void NextAction()
    {
        if (actionObject != null) Destroy(actionObject);

        var obj = Instantiate(actionPrefab, cardParent);

        currentAction = new() { Type = ActionType.GetMoney, Ammount = 20 };
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }
}