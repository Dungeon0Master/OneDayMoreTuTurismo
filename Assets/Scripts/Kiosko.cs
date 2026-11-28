using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Kiosk : MonoBehaviour, IInteractable
{
    [Header("Aviso (opcional)")]
    [SerializeField] private GameObject promptObject;   // el objeto que se muestra/oculta
    [SerializeField] private TMP_Text promptText;       // el texto (puede ser el mismo objeto)

    private void Awake()
    {
        if (promptObject != null)
            promptObject.SetActive(false);
    }

    public void OnPlayerEnter(PlayerInteractor player)
    {
        if (promptObject == null) return;

        // Muestra la tecla real configurada en el jugador
        if (promptText != null)
            promptText.text = $"[{player.InteractKey}] Usar kiosko";

        promptObject.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractor player)
    {
        if (promptObject != null)
            promptObject.SetActive(false);
    }

    public void Interact(PlayerInteractor player)
    {
        if (KioskMapUI.Instance == null)
        {
            Debug.LogError("[Kiosk] No hay un KioskMapUI en la escena.");
            return;
        }

        KioskMapUI.Instance.OpenFromKiosk(player);
    }
}