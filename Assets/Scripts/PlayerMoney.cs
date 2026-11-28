using System;
using UnityEngine;

public class PlayerMoney : MonoBehaviour, ISaveable
{
    [Header("Configuración de Dinero")]
  
    private int currentMoney;

    public event Action<int> OnMoneyChanged;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        currentMoney = GameSession.Archetype.StartingMoney;
    }

    private void Start()
    {
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    // Devuelve false si no alcanza; en ese caso no resta nada
    public bool TrySpend(int amount)
    {
        if (amount < 0 || !CanAfford(amount)) return false;

        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;

        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

   [Serializable]
private class State
{
    public int money;
}

public string SaveKey => "money";
public int RestoreOrder => 0;

public string CaptureState()
{
    return JsonUtility.ToJson(new State { money = currentMoney });
}

public void RestoreState(string json)
{
    State state = JsonUtility.FromJson<State>(json);

    currentMoney = Mathf.Max(0, state.money);
    OnMoneyChanged?.Invoke(currentMoney);
}
}