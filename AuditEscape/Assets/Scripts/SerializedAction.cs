using UnityEngine;

[System.Serializable]
public struct SerializedAction
{
    public string Title;

    [Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\" and the aggression with \"{2}\""), TextArea(3, 8)] 
    public string Description;

    [Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\" and the aggression with \"{2}\"")] 
    public string[] Pros;

    [Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\" and the aggression with \"{2}\"")] 
    public string[] Cons;

    public int CleanMoneyAdded;
    public int DirtyMoneyAdded;
    public int AggressionGained;
    public bool CanDeny;
    public bool IsPassive;
    public float Percentage;
}