using System;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Configuración de un dia (en minutos)")]
    [SerializeField] private int startHour = 8;   // 08:00
    [SerializeField] private int endHour = 20;    // 20:00

    private int currentTotalMinutes;
    private int endTotalMinutes;
    private int stepUnitsAccumulator; // 6 unidades = 1 minuto exacto

    // Eventos para que la UI y otros sistemas reaccionen sin acoplamiento
    public event Action<string> OnTimeFormattedChanged;
    public event Action OnDayEnded;

    public bool IsDayOver => currentTotalMinutes >= endTotalMinutes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentTotalMinutes = startHour * 60;
        endTotalMinutes = endHour * 60;
    }

    private void Start()
    {
        NotifyTimeChanged();
    }

    public void RegisterStep(bool isRunning)
    {
        if (IsDayOver) return;

        // Correr: 6 cuadros = 1 min (1 unidad/paso). Caminar: 3 cuadros = 1 min (2 unidades/paso)
        stepUnitsAccumulator += isRunning ? 1 : 2;

        if (stepUnitsAccumulator >= 6)
        {
            int minutesToAdd = stepUnitsAccumulator / 6;
            stepUnitsAccumulator %= 6;
            AdvanceMinutes(minutesToAdd);
        }
    }

    // Método público reutilizable
    public void AdvanceMinutes(int minutes)
    {
        if (IsDayOver || minutes <= 0) return;

        currentTotalMinutes = Mathf.Min(currentTotalMinutes + minutes, endTotalMinutes);
        NotifyTimeChanged();

        if (IsDayOver)
        {
            OnDayEnded?.Invoke();
        }
    }

    private void NotifyTimeChanged()
    {
        int hours = currentTotalMinutes / 60;
        int minutes = currentTotalMinutes % 60;
        string formattedTime = $"{hours:00}:{minutes:00}";
        OnTimeFormattedChanged?.Invoke(formattedTime);
    }
}