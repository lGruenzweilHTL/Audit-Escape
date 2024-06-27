using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaymentSystem : MonoBehaviour {
    [SerializeField] private PlayerStatsObject playerStats;
    [SerializeField] private Slider paymentSlider;
    [SerializeField] private TMP_Text sliderValueText;
    private event System.Action OnPaymentConfirmed; 
    private int chosenPay;
    private int maxPay;
    
    public async Task Activate(int maxPayment) {
        SetMaxPayment(maxPayment);
        gameObject.SetActive(true);
        
        // Wait for OnPaymentConfirmed to be invoked
        TaskCompletionSource<bool> tcs = new();
        OnPaymentConfirmed += () => tcs.TrySetResult(true);
        await tcs.Task;
    }

    private void SetDefaultSliderValue() {
        paymentSlider.value = paymentSlider.maxValue / 2;
        ChoosePay(paymentSlider.value);
    }

    private void SetMaxPayment(float max) {
        maxPay = (int)max;
        paymentSlider.maxValue = maxPay;
        SetDefaultSliderValue();
    }

    public void ChoosePay(float value) {
        chosenPay = (int)value;
        sliderValueText.text = chosenPay.ToString();
    }

    public void ConfirmPay() {
        gameObject.SetActive(false);
        
        playerStats.cleanMoney -= chosenPay;
        playerStats.workerHappiness += CalculateHappiness(chosenPay, maxPay);
        
        UI.Instance.UpdateStatsWithBonus(playerStats);
        
        OnPaymentConfirmed?.Invoke();
    }

    private static float CalculateHappiness(int paid, int max) {
        // Calculate the happiness of the workers based on the payment
        // When they are paid half of the max, they remain equally happy
        // When they are paid the max, they gain 1 happiness
        // When they are paid 0, the loose 1 happiness
        return 2f * paid / max - 1f;
    }
}
