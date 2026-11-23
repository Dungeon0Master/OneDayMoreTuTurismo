using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Kiosk : MonoBehaviour, IInteractable
{
    // Sin aviso por ahora; después se puede mostrar "[E] Usar kiosko"
    public void OnPlayerEnter(PlayerInteractor player) { }
    public void OnPlayerExit(PlayerInteractor player) { }

    public void Interact(PlayerInteractor player)
    {
        if (KioskMapUI.Instance == null)
        {
            Debug.LogError("[Kiosk] No hay un KioskMapUI en la escena.");
            return;
        }

        KioskMapUI.Instance.Open(player);
    }
}