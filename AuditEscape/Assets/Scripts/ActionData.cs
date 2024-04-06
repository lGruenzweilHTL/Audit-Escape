using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionData : MonoBehaviour
{
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button denyButton;

    public void Init(GameAction action, ActionHandler actionHandler)
    {
        if (action.Type == ActionType.Audit)
            denyButton.gameObject.SetActive(false);

        header.text = action.Type switch
        {
            ActionType.GetMoney => "Earn money",
            ActionType.LaunderMoney => "Launder money",
            ActionType.LaunderAll => "Launder money",
            ActionType.GetWorker => "Hire worker",
            ActionType.GetLaunderer => "Hire worker",
            ActionType.Audit => "AUDIT",
            _ => ""
        };
        description.text = action.Type switch
        {
            ActionType.GetMoney => $"Earn {action.Ammount}$ immediately",
            ActionType.LaunderMoney => $"Earn {action.Ammount}$ by laundering money",
            ActionType.LaunderAll => $"Immediately launder all your money",
            ActionType.GetWorker => $"Hire a worker that will make you {action.Ammount}$ of clean money per action",
            ActionType.GetLaunderer => $"Hire a worker that will launder {action.Ammount}$ per action",
            ActionType.Audit => "You are getting audited.\nWatch out!",
            _ => ""
        };

        acceptButton.onClick.AddListener(() => actionHandler.Continue(true));
        denyButton.onClick.AddListener(() => actionHandler.Continue(false));
    }
}