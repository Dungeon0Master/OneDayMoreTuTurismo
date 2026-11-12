using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private Image energyFillImage;
    [SerializeField] private TextMeshProUGUI clockText;

    [Header("Referencias de Sistemas")]
    [SerializeField] private PlayerEnergy playerEnergy;

    private void OnEnable()
    {
        if (playerEnergy != null)
            playerEnergy.OnEnergyNormalizedChanged += UpdateEnergyBar;

        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeFormattedChanged += UpdateClockText;
    }

    private void Start()
    {
        // Asegura suscripción si TimeManager se inicializó en Awake
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeFormattedChanged -= UpdateClockText;
            TimeManager.Instance.OnTimeFormattedChanged += UpdateClockText;
        }
    }

    private void OnDisable()
    {
        if (playerEnergy != null)
            playerEnergy.OnEnergyNormalizedChanged -= UpdateEnergyBar;

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
}