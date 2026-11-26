using System;
using UnityEngine;

public class PlayerApp : MonoBehaviour
{
    public bool IsInstalled { get; private set; }

    public event Action OnInstalled;

    public void Install()
    {
        if (IsInstalled) return;

        IsInstalled = true;
        OnInstalled?.Invoke();
    }
}