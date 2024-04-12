using UnityEngine;
using UnityEngine.Events;

public class ActionHandler : MonoBehaviour
{
    public static SerializedAction AUDIT_ACTION = new()
    {
        Title = "Audit",
        Description = "You are getting audited. Watch out!",
        CleanMoneyAdded = 0,
        DirtyMoneyAdded = 0,
        AggressionGained = 0,
        IsPassive = false,
        CanDeny = false,
        Pros = { },
        Cons = { },
        Percentage = 20 // Multiplier for actual value
    };

    [SerializeField] private SerializedAction[] actions;

    [SerializeField] private PlayerData player;
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject auditGame;

    [SerializeField] private AnimationCurve auditCurve;

    public UnityEvent<SerializedAction, bool> OnActionFinished;

    private SerializedAction currentAction;
    private GameObject actionObject;

    private void Start()
    {
        // Build ActionMap
        for (int i = 0; i < actions.Length; i++)
        {
            SerializedAction a = actions[i];
            a.Description = a.Description.Replace("{0}", a.CleanMoneyAdded.ToString()).Replace("{1}", a.DirtyMoneyAdded.ToString()).Replace("{2}", a.AggressionGained.ToString());
            for (int j = 0; j < a.Pros.Length; j++) a.Pros[j] = a.Pros[j].Replace("{0}", a.CleanMoneyAdded.ToString()).Replace("{1}", a.DirtyMoneyAdded.ToString()).Replace("{2}", a.AggressionGained.ToString());
            for (int j = 0; j < a.Cons.Length; j++) a.Cons[j] = a.Cons[j].Replace("{0}", a.CleanMoneyAdded.ToString()).Replace("{1}", a.DirtyMoneyAdded.ToString()).Replace("{2}", a.AggressionGained.ToString());

            actions[i] = a;
        }

        NextAction();
    }

    [ContextMenu("Start Audit")]
    public void StartAudit()
    {
        auditGame.SetActive(true);
    }

    public void Continue(bool accepted)
    {
        OnActionFinished?.Invoke(currentAction, accepted);

        if (currentAction.Title == "Audit") auditGame.SetActive(true);
        NextAction();
    }
    private async void NextAction()
    {
        if (actionObject != null)
        {
            Destroy(actionObject, 1.1f);

            await System.Threading.Tasks.Task.Delay(100); // Wait for button animation

            actionObject.GetComponent<Animator>().SetTrigger("MoveOut");
        }

        var obj = Instantiate(actionPrefab, cardParent);

        currentAction = GetAction(player.Aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }

    private SerializedAction GetAction(int aggression)
    {
        float maxPercentage = AUDIT_ACTION.Percentage * aggression * auditCurve.Evaluate(aggression / 100f);
        float countedPercentage = 0;

        for (int i = 0; i < actions.Length; i++) maxPercentage += actions[i].Percentage;

        float random = Random.Range(0, maxPercentage);

        for (int i = 0; i < actions.Length; i++)
        {
            SerializedAction currentAction = actions[i];
            float percent = currentAction.Percentage;
            if (random >= countedPercentage && random < countedPercentage + percent) return currentAction;

            countedPercentage += percent;
        }

        return AUDIT_ACTION;
    }
}