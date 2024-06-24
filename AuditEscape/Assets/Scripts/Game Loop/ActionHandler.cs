using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Timeline.Actions.MenuPriority;
using Random = UnityEngine.Random;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private SerializedAction[] actions;

    [SerializeField] private PlayerData player;
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject auditGame;

    [SerializeField] private AnimationCurve auditCurve;
    [SerializeField] private float auditMultiplier;

    public UnityEvent<SerializedAction, bool> OnActionFinished;

    private SerializedAction currentAction;
    private GameObject actionObject;

    private bool previousWasAudit = false;

    private static SerializedAction _auditAction = new SerializedAction
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
        _auditAction.Percentage = auditMultiplier;

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

            actionObject.GetComponent<Animator>().SetTrigger(MoveOut);
        }

        var obj = Instantiate(actionPrefab, cardParent);
        
        currentAction = GetAction(player.Aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }

    private SerializedAction GetAction(int aggression)
    {
        float maxPercentage = _auditAction.Percentage * aggression * auditCurve.Evaluate(aggression / 100f);
        if (previousWasAudit) maxPercentage = 0;
        float countedPercentage = 0;
        
        print($"Previous action was audit: {previousWasAudit}");
        print($"Current chance of audit: {maxPercentage}");

        for (int i = 0; i < actions.Length; i++) maxPercentage += actions[i].Percentage;

        float random = Random.Range(0, maxPercentage);

        foreach (SerializedAction currAction in actions) {
            float percent = currAction.Percentage;
            if (random >= countedPercentage && random < countedPercentage + percent)
            {
                print($"Action chosen: {currAction.Title}");
                previousWasAudit = false;
                return currAction;
            }

            countedPercentage += percent;
        }

        print("Audit chosen");
        previousWasAudit = true;
        return _auditAction;
    }


    // TODO: implement replacement process with this function
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