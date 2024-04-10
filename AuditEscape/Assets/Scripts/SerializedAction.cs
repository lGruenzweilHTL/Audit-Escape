[System.Serializable]
public struct SerializedAction
{
    public string Title;
    [UnityEngine.Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\" and the aggression with \"{2}\"")] public string Description;
    [UnityEngine.Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\" and the aggression with \"{2}\"")] public string[] Pros;
    [UnityEngine.Tooltip("Replace the clean money with \"{0}\", the dirty money with \"{1}\" and the aggression with \"{2}\"")] public string[] Cons;
    public int CleanMoneyAdded;
    public int DirtyMoneyAdded;
    public int AggressionGained;
    public bool CanDeny;
    public bool IsPassive;
    public float Percentage;
}