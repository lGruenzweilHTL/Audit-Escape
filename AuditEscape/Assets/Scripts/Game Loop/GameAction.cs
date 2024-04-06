public struct GameAction
{
    public ActionType Type;

    [UnityEngine.Tooltip("Not used for some types")]
    public int Ammount;
}
public enum ActionType
{
    GetMoney, LaunderMoney, GetWorker, GetLaunderer, LaunderAll, Audit
}