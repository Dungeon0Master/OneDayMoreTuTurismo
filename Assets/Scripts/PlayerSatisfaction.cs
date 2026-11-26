using System;
using UnityEngine;

public class PlayerSatisfaction : MonoBehaviour
{
    private int currentSatisfaction;

    public event Action<int> OnSatisfactionChanged;

    public int CurrentSatisfaction => currentSatisfaction;

    private void Start()
    {
        OnSatisfactionChanged?.Invoke(currentSatisfaction);
    }

    // Acepta valores negativos: servirá para la penalización por quedarse sin energía
    public void Add(int amount)
    {
        currentSatisfaction += amount;
        OnSatisfactionChanged?.Invoke(currentSatisfaction);
    }

    // Suma los puntos de un lugar aplicando el multiplicador del arquetipo.
    // Devuelve cuántos puntos ganó realmente.
    public int AddFromPlace(PlaceInfo info)
    {
        int gained = GameSession.Archetype.CalculateSatisfaction(info);
        Add(gained);
        return gained;
    }
}