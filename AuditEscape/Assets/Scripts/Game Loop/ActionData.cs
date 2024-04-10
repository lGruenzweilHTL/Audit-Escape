using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionData : MonoBehaviour
{
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text prosCons;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button denyButton;

    public void Init(SerializedAction action, ActionHandler actionHandler)
    {
        if (!action.CanDeny) denyButton.gameObject.SetActive(false);

        header.text = action.Title;
        description.text = action.Description;

        string prosCons = "";
        if (action.Pros != null && action.Pros.Length > 0)
        {
            prosCons += "<color=\"green\">";
            foreach (string pro in action.Pros) prosCons += pro + '\n';
        }
        if (action.Cons != null && action.Cons.Length > 0)
        {
            prosCons += "<color=\"red\">";
            foreach (string con in action.Cons) prosCons += con + '\n';
        }
        this.prosCons.text = prosCons;

        acceptButton.onClick.AddListener(() => actionHandler.Continue(true));
        denyButton.onClick.AddListener(() => actionHandler.Continue(false));
    }
}