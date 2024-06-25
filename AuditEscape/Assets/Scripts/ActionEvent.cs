using UnityEngine;

[System.Serializable]
public class ActionEvent {
    public ActionEvent() { }

    public ActionEvent(SerializedAction action) {
        Title = action.Title;
        Description = action.Description;
        Pros = action.Pros;
        Cons = action.Cons;
        CleanMoneyAdded = action.CleanMoneyAdded;
        DirtyMoneyAdded = action.DirtyMoneyAdded;
        AggressionGained = action.AggressionGained;
        WorkerEfficiency = action.WorkerEfficiency;
        IsPassive = action.IsPassive;
    }
    
    public string Title;

    [Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\", the aggression with \"{2}\" and the worker efficiency with \"{3}\""),
     TextArea(3, 8)]
    public string Description;

    [Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\", the aggression with \"{2}\" and the worker efficiency with \"{3}\"")]
    public string[] Pros;

    [Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\", the aggression with \"{2}\" and the worker efficiency with \"{3}\"")]
    public string[] Cons;

    public int CleanMoneyAdded;
    public int DirtyMoneyAdded;
    public int AggressionGained;
    public float WorkerEfficiency;
    public bool IsPassive;
}