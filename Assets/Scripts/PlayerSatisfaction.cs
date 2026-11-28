using System;
using UnityEngine;

public class PlayerSatisfaction : MonoBehaviour, ISaveable
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
    [Serializable]
private class State
{
    public int satisfaction;
}

public string SaveKey => "satisfaction";
public int RestoreOrder => 0;

public string CaptureState()
{
    return JsonUtility.ToJson(new State { satisfaction = currentSatisfaction });
}

public void RestoreState(string json)
{
    State state = JsonUtility.FromJson<State>(json);

    currentSatisfaction = state.satisfaction;
    OnSatisfactionChanged?.Invoke(currentSatisfaction);
}
}