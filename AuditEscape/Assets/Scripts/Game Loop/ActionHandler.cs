using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ActionHandler : MonoBehaviour {
    [SerializeField] private SerializedAction[] actions;
    [SerializeField] private ActionEvent[] randomEvents;

    [SerializeField] private PlayerStatsObject playerStats;
    [SerializeField] private ActionData actionPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject auditGame;
    [SerializeField] private PaymentSystem paymentSystem;
    [SerializeField] private ShopManager shop;

    [SerializeField] private AnimationCurve auditCurve;
    [SerializeField] private float auditMultiplier;

    public UnityEvent<SerializedAction, bool> OnActionFinished;

    private SerializedAction currentAction;
    private GameObject actionObject;

    private bool previousWasAudit;
    private bool previousWasShopSale;
    private int nextRandomEvent;
    private int nextWorkerPayment;

    private static readonly SerializedAction AuditAction = new() {
        Title = "Audit",
        Description = "You are getting audited. Watch out!",
    };

    private static readonly ActionEvent ShopSaleEvent = new() {
        Title = "Shop Sale (RE)",
        Description = "A random event has caused the shop to offer a discount."
    };
    
    private static readonly SerializedAction WorkerPaymentAction = new() {
        Title = "Pay your workers!",
        Description = "Your workers want to get paid."
    };

    private static readonly int MoveOutAnimation = Animator.StringToHash("MoveOut");

    private void Start() {
        AuditAction.Percentage = auditMultiplier;
        ScheduleNextRandomEvent();
        ScheduleNextWorkerPayment();

        // Replace the placeholders in the actions
        // Use the ReplaceMultiple function to replace multiple placeholders at once
        for (int i = 0; i < actions.Length; i++) {
            actions[i] = ReplacePlaceholdersInAction(actions[i]);
        }

        // Do the same with the random events
        for (int i = 0; i < randomEvents.Length; i++) {
            randomEvents[i] = ReplacePlaceholdersInAction(randomEvents[i]);
        }

        NextAction();
    }

    public void Continue(bool accepted) {
        OnActionFinished?.Invoke(currentAction, accepted);

        if (currentAction.Equals(AuditAction)) auditGame.SetActive(true);
        if (currentAction.Equals(WorkerPaymentAction)) {
            paymentSystem.SetMaxPayment(playerStats.cleanMoney);
            paymentSystem.gameObject.SetActive(true);
        }
        NextAction();
    }

    private async void NextAction() {
        if (actionObject != null) {
            Destroy(actionObject, 1.1f);

            await System.Threading.Tasks.Task.Delay(100); // Wait for button animation

            actionObject.GetComponent<Animator>().SetTrigger(MoveOutAnimation);
        }

        ActionData obj = Instantiate(actionPrefab, cardParent);

        currentAction = GetAction(playerStats.aggression);
        obj.Init(currentAction, this);
        actionObject = obj.gameObject;
    }
    
    // Get Random Action

    private SerializedAction GetAction(int aggression) {
        if (IsWorkerPaymentScheduled()) {
            ScheduleNextWorkerPayment();
            return WorkerPaymentAction;
        }
        
        if (IsRandomEventScheduled()) {
            ResetShopDiscount();
            ScheduleNextRandomEvent();
            return GetRandomEvent();
        }

        float maxPercentage = CalculateMaxPercentage(aggression);
        float random = Random.Range(0, maxPercentage);

        return ChooseActionBasedOnRandomNumber(random);
    }

    private bool IsRandomEventScheduled() {
        return nextRandomEvent-- <= 0;
    }

    private bool IsWorkerPaymentScheduled() {
        return nextWorkerPayment-- <= 0;
    }

    private void ResetShopDiscount() {
        shop.DiscountMultiplier = 1;
    }

    private void ScheduleNextRandomEvent() {
        nextRandomEvent = Random.Range(8, 14);
    }
    
    private void ScheduleNextWorkerPayment() {
        nextWorkerPayment = Random.Range(9, 11);
    }

    private SerializedAction GetRandomEvent() {
        int index = Random.Range(0, randomEvents.Length + 1);

        if (index == randomEvents.Length && !previousWasShopSale) {
            shop.DiscountMultiplier = 0.7f;
            previousWasShopSale = true;
            return new SerializedAction(ShopSaleEvent);
        }

        previousWasShopSale = false;
        return new SerializedAction(randomEvents[index]);
    }

    private float CalculateMaxPercentage(int aggression) {
        float maxPercentage = AuditAction.Percentage * aggression * auditCurve.Evaluate(aggression / 100f);
        if (previousWasAudit) maxPercentage = 0;
        maxPercentage += actions.Sum(a => a.Percentage);
        return maxPercentage;
    }

    private SerializedAction ChooseActionBasedOnRandomNumber(float random) {
        float countedPercentage = 0;

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

    // Replace Placeholders

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
        action.Description = ReplacePlaceholdersInString(action.Description, action.CleanMoneyAdded.ToString(),
            action.DirtyMoneyAdded.ToString(), action.AggressionGained.ToString(),
            action.WorkerEfficiency.ToString(CultureInfo.InvariantCulture));
        action.Pros = action.Pros.Select(s => ReplacePlaceholdersInString(s, action.CleanMoneyAdded.ToString(),
            action.DirtyMoneyAdded.ToString(), action.AggressionGained.ToString(),
            action.WorkerEfficiency.ToString(CultureInfo.InvariantCulture))).ToArray();
        action.Cons = action.Cons.Select(s => ReplacePlaceholdersInString(s, action.CleanMoneyAdded.ToString(),
            action.DirtyMoneyAdded.ToString(), action.AggressionGained.ToString(),
            action.WorkerEfficiency.ToString(CultureInfo.InvariantCulture))).ToArray();
        return action;
    }

    private static ActionEvent ReplacePlaceholdersInAction(ActionEvent action) {
        return new ActionEvent(ReplacePlaceholdersInAction(new SerializedAction(action)));
    }

    private static string ReplacePlaceholdersInString(string s, string value1, string value2, string value3,
        string value4) {
        return ReplaceMultiple(s, ("{0}", value1), ("{1}", value2), ("{2}", value3), ("{3}", value4));
    }
}