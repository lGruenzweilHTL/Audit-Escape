using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Timeline.Actions.MenuPriority;
using Random = UnityEngine.Random;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private SerializedAction[] actions;
    [SerializeField] private ActionEvent[] randomEvents;

    [SerializeField] private PlayerData player;
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject auditGame;

    [SerializeField] private AnimationCurve auditCurve;
    [SerializeField] private float auditMultiplier;

    public UnityEvent<SerializedAction, bool> OnActionFinished;

    private SerializedAction currentAction;
    private GameObject actionObject;

    private bool previousWasAudit;
    private int nextRandomEvent;

    private static readonly SerializedAction AuditAction = new()
    {
        Title = "Audit",
        Description = "You are getting audited. Watch out!",
        CleanMoneyAdded = 0,
        DirtyMoneyAdded = 0,
        AggressionGained = 0,
        IsPassive = false,
        CanDeny = false,
    };

    private static readonly int MoveOut = Animator.StringToHash("MoveOut");

    private void Start()
    {
        AuditAction.Percentage = auditMultiplier;
        nextRandomEvent = Random.Range(8, 16);
        
        // Replace the placeholders in the actions
        // Use the ReplaceMultiple function to replace multiple placeholders at once
        foreach (SerializedAction action in actions)
        {
            action.Description = ReplaceMultiple(action.Description, ("{0}", action.CleanMoneyAdded.ToString()), ("{1}", action.DirtyMoneyAdded.ToString()), ("{2}", action.AggressionGained.ToString()));
            action.Pros = action.Pros.Select(s => ReplaceMultiple(s, ("{0}", action.CleanMoneyAdded.ToString()), ("{1}", action.DirtyMoneyAdded.ToString()), ("{2}", action.AggressionGained.ToString()))).ToArray();
            action.Cons = action.Cons.Select(s => ReplaceMultiple(s, ("{0}", action.CleanMoneyAdded.ToString()), ("{1}", action.DirtyMoneyAdded.ToString()), ("{2}", action.AggressionGained.ToString()))).ToArray();
        }
        // Do the same with the random events
        foreach (ActionEvent action in randomEvents)
        {
            action.Description = ReplaceMultiple(action.Description, ("{0}", action.CleanMoneyAdded.ToString()), ("{1}", action.DirtyMoneyAdded.ToString()), ("{2}", action.AggressionGained.ToString()));
            action.Pros = action.Pros.Select(s => ReplaceMultiple(s, ("{0}", action.CleanMoneyAdded.ToString()), ("{1}", action.DirtyMoneyAdded.ToString()), ("{2}", action.AggressionGained.ToString()))).ToArray();
            action.Cons = action.Cons.Select(s => ReplaceMultiple(s, ("{0}", action.CleanMoneyAdded.ToString()), ("{1}", action.DirtyMoneyAdded.ToString()), ("{2}", action.AggressionGained.ToString()))).ToArray();
        }

        NextAction();
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

            actionObject.GetComponent<Animator>().SetTrigger(MoveOut);
        }

        ActionData obj = Instantiate(actionPrefab, cardParent);
        
        currentAction = GetAction(player.Aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }

    private SerializedAction GetAction(int aggression)
    {
        // if a random event is scheduled, choose one from the array
        if (nextRandomEvent-- == 0)
        {
            nextRandomEvent = Random.Range(8, 16);
            return new SerializedAction(randomEvents[Random.Range(0, randomEvents.Length)]);
        }
        
        float maxPercentage = AuditAction.Percentage * aggression * auditCurve.Evaluate(aggression / 100f);
        if (previousWasAudit) maxPercentage = 0;
        float countedPercentage = 0;

        maxPercentage += actions.Sum(a => a.Percentage);

        float random = Random.Range(0, maxPercentage);

        // Choose an action based on the random number
        foreach (SerializedAction currAction in actions) {
            float chance = currAction.Percentage;
            if (random >= countedPercentage && random < countedPercentage + chance)
            {
                previousWasAudit = false;
                return currAction;
            }

            countedPercentage += chance;
        }
        
        previousWasAudit = true;
        return AuditAction;
    }

    // A more efficient function to replace multiple placeholders in a string
    private static string ReplaceMultiple(string s, params (string, string)[] replaceData) {
        StringBuilder result = new(s);

        foreach (var replacement in replaceData)
        {
            int index;
            while ((index = result.ToString().IndexOf(replacement.Item1, System.StringComparison.Ordinal)) != -1)
            {
                result.Remove(index, replacement.Item1.Length);
                result.Insert(index, replacement.Item2);
            }
        }

        return result.ToString();

    }
}