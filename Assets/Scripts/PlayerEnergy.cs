using System;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Configuración de Energía")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float runCostPerStep = 2f;

    private float currentEnergy;
    private int exhaustedStepsCounter;

    // Envía el valor normalizado (0 a 1) directamente compatible con Image.fillAmount
    public event Action<float> OnEnergyNormalizedChanged;
    public event Action OnExhaustedPenaltyTriggered;

    public bool CanRun => currentEnergy >= runCostPerStep;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    private void Start()
    {
        NotifyEnergyChanged();
    }

    public void ProcessStep(bool isRunning)
    {
        if (isRunning && CanRun)
        {
            ConsumeEnergy(runCostPerStep);
            exhaustedStepsCounter = 0;
        }
        else if (!CanRun)
        {
            //  cada 3 pasos resta satisfacción
            exhaustedStepsCounter++;
            if (exhaustedStepsCounter >= 3)
            {
                exhaustedStepsCounter = 0;
                OnExhaustedPenaltyTriggered?.Invoke();
            }
        }
    }

    public void ConsumeEnergy(float amount)
    {
        currentEnergy = Mathf.Max(0f, currentEnergy - amount);
        NotifyEnergyChanged();
    }

    // Método público reutilizable para comida y descanso
    public void RecoverEnergy(float amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        NotifyEnergyChanged();
    }

    private void NotifyEnergyChanged()
    {
        OnEnergyNormalizedChanged?.Invoke(currentEnergy / maxEnergy);
    }
}