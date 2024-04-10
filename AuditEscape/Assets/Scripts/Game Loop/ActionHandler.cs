using System.Collections.Generic;
using System.Linq;
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
        Percentage = 0 // Getting assigned dynamically
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

    public Dictionary<string, SerializedAction> ActionMap = new();

    private void Start()
    {
        // Build ActionMap
        for (int i = 0; i < actions.Length; i++)
        {
            SerializedAction a = actions[i];
            a.Description = a.Description.Replace("{0}", a.CleanMoneyAdded.ToString()).Replace("{1}", a.DirtyMoneyAdded.ToString()).Replace("{2}", a.AggressionGained.ToString());
            for (int j = 0; j < a.Pros.Length; j++) a.Pros[j] = a.Pros[j].Replace("{0}", a.CleanMoneyAdded.ToString()).Replace("{1}", a.DirtyMoneyAdded.ToString()).Replace("{2}", a.AggressionGained.ToString());
            for (int j = 0; j < a.Cons.Length; j++) a.Cons[j] = a.Cons[j].Replace("{0}", a.CleanMoneyAdded.ToString()).Replace("{1}", a.DirtyMoneyAdded.ToString()).Replace("{2}", a.AggressionGained.ToString());

            ActionMap.Add(a.Title, a);
        }

        NextAction();
    }

    public void Continue(bool accepted)
    {
        OnActionFinished?.Invoke(currentAction, accepted);

        if (currentAction.Title == "Audit") auditGame.SetActive(true);
        NextAction();
    }
    private void NextAction()
    {
        if (actionObject != null) Destroy(actionObject);

        var obj = Instantiate(actionPrefab, cardParent);

        currentAction = GetAction(player.Aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }

    private SerializedAction GetAction(int aggression)
    {
        AUDIT_ACTION.Percentage = aggression;
        float maxPercentage = AUDIT_ACTION.Percentage;
        float countedPercentage = 0;

        for (int i = 0; i < ActionMap.Count; i++) maxPercentage += ActionMap.Values.ToArray()[i].Percentage;

        float random = Random.Range(0, maxPercentage);

        for (int i = 0; i < ActionMap.Count; i++)
        {
            SerializedAction currentAction = ActionMap.Values.ToArray()[i];
            float percent = currentAction.Percentage;
            if (random >= countedPercentage && random < countedPercentage + percent) return currentAction;

            countedPercentage += percent;
        }

        return AUDIT_ACTION;
    }
}