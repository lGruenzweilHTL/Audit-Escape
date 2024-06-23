using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    public static UI Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    [SerializeField] private TMP_Text cleanMoney, dirtyMoney, passiveMoney, passiveLaundering;
    [Space, SerializeField] private Slider aggressionSlider;
    [SerializeField] private Image aggressionSliderFill;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color watchlistColor;

    public void UpdateStats(PlayerStatsObject stats) {
        UpdateStats(stats.cleanMoney, stats.dirtyMoney, stats.passiveMoney, stats.passiveLaundering);
        UpdateAggression(stats.aggression);
    }

    public void UpdateStats(int clean, int dirty, int passive, int laundering) {
        cleanMoney.text = clean + "$";
        dirtyMoney.text = dirty + "$";
        passiveMoney.text = passive + "$/action";
        passiveLaundering.text = laundering + "$/action";
    }

    public void UpdateStatsWithBonus(int clean, int dirty, int basePassive, int bonusPassive, int baseLaundering,
        int bonusLaundering) {
        if (bonusPassive == 0 && bonusLaundering == 0) {
            UpdateStats(clean, dirty, basePassive, baseLaundering);
            return;
        }
        
        cleanMoney.text = clean + "$";
        dirtyMoney.text = dirty + "$";

        passiveMoney.text = $"{basePassive}<size=-15><color=\"white\">+{bonusPassive}</size></color> $/action";
        passiveLaundering.text = $"{baseLaundering}<size=-15><color=\"white\">+{bonusLaundering}</size></color> $/action";
    }

    public void UpdateStatsWithBonus(PlayerStatsObject stats) {
        float bonusPassive = stats.passiveMoney * stats.workerHappiness * stats.workerEfficiency - stats.passiveMoney;
        float bonusLaundering = stats.passiveLaundering * stats.workerHappiness * stats.workerEfficiency - stats.passiveLaundering;
        UpdateStatsWithBonus(stats.cleanMoney, stats.dirtyMoney, stats.passiveMoney, (int)bonusPassive,
            stats.passiveLaundering, (int)bonusLaundering);
        UpdateAggression(stats.aggression);
    }

    public void UpdateAggression(int value) {
        aggressionSliderFill.gameObject.SetActive(value > 0);

        if (value <= 50) {
            aggressionSlider.value = Map(value, 0, 50, 0, 80);
            aggressionSliderFill.color = normalColor;
        }
        else {
            aggressionSlider.value = Map(value, 50, 100, 80, 100);
            aggressionSliderFill.color = watchlistColor;
        }
    }

    private static float Map(float s, float a1, float a2, float b1, float b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);
}