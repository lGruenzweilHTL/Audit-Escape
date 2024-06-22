using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Data/Player Stats", fileName = "PlayerStats")]
public class PlayerStatsObject : ScriptableObject {
    [Tooltip("The money you have earned cleanly")] 
    public int cleanMoney = 0;

    [Tooltip("The money you have earned with questionable methods")]
    public int dirtyMoney = 0;

    [Tooltip("The money you gain passively per action")]
    public int passiveMoney = 0;

    [Tooltip("The money you launder every action")]
    public int passiveLaundering = 0;
    
    [Tooltip("The aggression the IRS has towards you (0..100)")]
    public int aggression = 0;
    
    [Header("Shop items")]
    [Tooltip("The efficiency of your workers")]
    public int workerEfficiency = 1;

    [Tooltip("How happy your workers are")]
    public float workerHappiness = 1;

    [Tooltip("The relative decrease of audit difficulty")]
    public float auditDifficultyDecrease = 0;

    [Tooltip("The suspicion you get every action (even if it was denied)")]
    public int passiveSuspicion = 2;
}