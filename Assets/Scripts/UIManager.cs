using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private Image energyFillImage;
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI satisfactionText;

    [Header("Referencias de Sistemas")]
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private PlayerMoney playerMoney;
    [SerializeField] private PlayerSatisfaction playerSatisfaction;

    private void OnEnable()
    {
        if (playerEnergy != null)
            playerEnergy.OnEnergyNormalizedChanged += UpdateEnergyBar;

        if (playerMoney != null)
            playerMoney.OnMoneyChanged += UpdateMoneyText;

        if (playerSatisfaction != null)
            playerSatisfaction.OnSatisfactionChanged += UpdateSatisfactionText;

        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeFormattedChanged += UpdateClockText;
    }

    private void OnDisable()
    {
        if (playerEnergy != null)
            playerEnergy.OnEnergyNormalizedChanged -= UpdateEnergyBar;

        if (playerMoney != null)
            playerMoney.OnMoneyChanged -= UpdateMoneyText;

        if (playerSatisfaction != null)
            playerSatisfaction.OnSatisfactionChanged -= UpdateSatisfactionText;

        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeFormattedChanged -= UpdateClockText;
    }

    private void UpdateEnergyBar(float normalizedValue)
    {
        if (energyFillImage != null)
            energyFillImage.fillAmount = normalizedValue;
    }

    private void UpdateClockText(string formattedTime)
    {
        if (clockText != null)
            clockText.text = formattedTime;
    }

    private void UpdateMoneyText(int money)
    {
        if (moneyText != null)
            moneyText.text = $"${money}";
    }

    private void UpdateSatisfactionText(int satisfaction)
    {
        if (satisfactionText != null)
            satisfactionText.text = $"Score:\n{satisfaction:00000}";
    }
}