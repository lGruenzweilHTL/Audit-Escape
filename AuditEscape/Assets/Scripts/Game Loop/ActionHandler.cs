using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Timeline.Actions.MenuPriority;
using Random = UnityEngine.Random;

public class ActionHandler : MonoBehaviour {
    [SerializeField] private SerializedAction[] actions;
    [SerializeField] private ActionEvent[] randomEvents;

    [SerializeField] private PlayerData player;
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject auditGame;
    [SerializeField] private ShopManager shop;

    [SerializeField] private AnimationCurve auditCurve;
    [SerializeField] private float auditMultiplier;

    public UnityEvent<SerializedAction, bool> OnActionFinished;

    private SerializedAction currentAction;
    private GameObject actionObject;

    private bool previousWasAudit;
    private bool previousWasShopSale;
    private int nextRandomEvent;

    private static readonly SerializedAction AuditAction = new() {
        Title = "Audit",
        Description = "You are getting audited. Watch out!",
    };

    private static readonly ActionEvent ShopSaleEvent = new() {
        Title = "Shop Sale (RE)",
        Description = "A random event has caused the shop to offer a discount."
    };

    private static readonly int MoveOut = Animator.StringToHash("MoveOut");

    private void Start() {
        AuditAction.Percentage = auditMultiplier;
        nextRandomEvent = Random.Range(8, 16);

        // Replace the placeholders in the actions
        // Use the ReplaceMultiple function to replace multiple placeholders at once
        for (int i = 0; i < actions.Length; i++) {
            actions[i] = ReplacePlaceholdersInAction(actions[i]);
        }

        // Do the same with the random events
        for (int i = 0; i < randomEvents.Length; i++) {
            randomEvents[i] = new(ReplacePlaceholdersInAction(new(randomEvents[i])));
        }

        NextAction();
    }

    public void Continue(bool accepted) {
        OnActionFinished?.Invoke(currentAction, accepted);

        if (currentAction.Title == "Audit") auditGame.SetActive(true);
        NextAction();
    }

    private async void NextAction() {
        if (actionObject != null) {
            Destroy(actionObject, 1.1f);

            await System.Threading.Tasks.Task.Delay(100); // Wait for button animation

            actionObject.GetComponent<Animator>().SetTrigger(MoveOut);
        }

        ActionData obj = Instantiate(actionPrefab, cardParent);

        currentAction = GetAction(player.Aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }

    private SerializedAction GetAction(int aggression) {
        // if a random event is scheduled, choose one from the array
        if (nextRandomEvent-- == 0) {
            // Every random events resets the shop discount
            shop.DiscountMultiplier = 1;
            
            nextRandomEvent = Random.Range(8, 14); // choose time for next random event
            int index = Random.Range(0, randomEvents.Length + 1);

            // If last index is chosen, return the shop sale event
            if (index == randomEvents.Length && !previousWasShopSale) {
                shop.DiscountMultiplier = 0.7f;
                previousWasShopSale = true;
                return new SerializedAction(ShopSaleEvent);
            }
            
            // Else return the randomly chosen event
            previousWasShopSale = false;
            return new SerializedAction(randomEvents[index]);
        }

        float maxPercentage = AuditAction.Percentage * aggression * auditCurve.Evaluate(aggression / 100f);
        if (previousWasAudit) maxPercentage = 0;
        float countedPercentage = 0;

        maxPercentage += actions.Sum(a => a.Percentage);

        float random = Random.Range(0, maxPercentage);

        // Choose an action based on the random number
        foreach (SerializedAction currAction in actions) {
            float chance = currAction.Percentage;
            if (random >= countedPercentage && random < countedPercentage + chance) {
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

        foreach (var replacement in replaceData) {
            int index;
            while ((index = result.ToString().IndexOf(replacement.Item1, System.StringComparison.Ordinal)) != -1) {
                result.Remove(index, replacement.Item1.Length);
                result.Insert(index, replacement.Item2);
            }
        }

        return result.ToString();
    }

    private static SerializedAction ReplacePlaceholdersInAction(SerializedAction action) {
        action.Description = ReplacePlaceholdersInString(action.Description, action.CleanMoneyAdded.ToString(), action.DirtyMoneyAdded.ToString(), action.AggressionGained.ToString(), action.WorkerEfficiency.ToString(CultureInfo.InvariantCulture));
        action.Pros = action.Pros.Select(s => ReplacePlaceholdersInString(s, action.CleanMoneyAdded.ToString(), action.DirtyMoneyAdded.ToString(), action.AggressionGained.ToString(), action.WorkerEfficiency.ToString(CultureInfo.InvariantCulture))).ToArray();
        action.Cons = action.Cons.Select(s => ReplacePlaceholdersInString(s, action.CleanMoneyAdded.ToString(), action.DirtyMoneyAdded.ToString(), action.AggressionGained.ToString(), action.WorkerEfficiency.ToString(CultureInfo.InvariantCulture))).ToArray();
        return action;
    }

    private static string ReplacePlaceholdersInString(string s, string value1, string value2, string value3, string value4) {
        return ReplaceMultiple(s, ("{0}", value1), ("{1}", value2), ("{2}", value3), ("{3}", value4));
    }
}