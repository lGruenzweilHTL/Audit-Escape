[System.Serializable]
public class SerializedAction : ActionEvent {
    public SerializedAction() { }

    public SerializedAction(ActionEvent baseAction) {
        // Base values
        AggressionGained = baseAction.AggressionGained;
        CleanMoneyAdded = baseAction.CleanMoneyAdded;
        DirtyMoneyAdded = baseAction.DirtyMoneyAdded;
        Description = baseAction.Description;
        Cons = baseAction.Cons;
        Pros = baseAction.Pros;
        Title = baseAction.Title;
        IsPassive = baseAction.IsPassive;

        // Special values
        CanDeny = false;
        Percentage = 0;
    }

    public bool CanDeny;
    public float Percentage;
}