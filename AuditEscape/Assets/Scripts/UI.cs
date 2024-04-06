using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private TMP_Text cleanMoney, dirtyMoney, passiveMoney, passiveLaundering;
    [Space, SerializeField] private Slider aggressionSlider;
    [SerializeField] private Image aggressionSliderFill;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color watchlistColor;

    public void UpdateStats(int clean, int dirty, int passive, int laundering)
    {
        cleanMoney.text = clean + "$";
        dirtyMoney.text = dirty + "$";
        passiveMoney.text = passive + "$/action";
        passiveLaundering.text = laundering + "$/action";
    }

    public void UpdateAggression(int value)
    {
        aggressionSliderFill.gameObject.SetActive(value > 0);

        if (value <= 50)
        {
            aggressionSlider.value = Map(value, 0, 50, 0, 80);
            aggressionSliderFill.color = normalColor;
        }
        else
        {
            aggressionSlider.value = Map(value, 50, 100, 80, 100);
            aggressionSliderFill.color = watchlistColor;
        }
    }
    private float Map(float s, float a1, float a2, float b1, float b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);
}