using System;
using UnityEngine;

public class PlayerApp : MonoBehaviour, ISaveable
{
    public bool IsInstalled { get; private set; }

    public event Action OnInstalled;

    public void Install()
    {
        if (IsInstalled) return;

        IsInstalled = true;
        OnInstalled?.Invoke();
    }
    [Serializable]
private class State
{
    public bool installed;
}

public string SaveKey => "app";
public int RestoreOrder => 0;

public string CaptureState()
{
    return JsonUtility.ToJson(new State { installed = IsInstalled });
}

public void RestoreState(string json)
{
    State state = JsonUtility.FromJson<State>(json);

    // Install() dispara el evento, así el botón del HUD aparece solo
    if (state.installed)
        Install();
}
}