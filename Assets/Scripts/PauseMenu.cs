using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmPanel;

    [Header("Botones de pausa")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Confirmación")]
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private TextMeshProUGUI confirmButtonLabel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Referencias")]
    [SerializeField] private PlayerController player;
    [SerializeField] private string menuSceneName = "MainMenu";

    private Action pendingConfirmAction;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        resumeButton.onClick.AddListener(Resume);
        saveButton.onClick.AddListener(OnSaveClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(CloseConfirm);

        pausePanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    private void Update()
    {
        // Guardar solo se permite con el personaje quieto (no a mitad de un paso)
        if (IsOpen)
            saveButton.interactable = SaveManager.Instance != null && SaveManager.Instance.CanSaveNow;

        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (IsOpen)
        {
            if (confirmPanel.activeSelf) CloseConfirm();
            else Resume();
        }
        else if (CanOpen())
        {
            Open();
        }
    }

    private bool CanOpen()
    {
        // Esc también cierra el mapa del kiosko: ese mismo Esc no debe abrir la pausa
        KioskMapUI map = KioskMapUI.Instance;
        return map == null || (!map.IsOpen && !map.ClosedThisFrame);
    }

    private void Open()
    {
        IsOpen = true;
        player.InputEnabled = false;
        messageText.text = "";
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        IsOpen = false;
        confirmPanel.SetActive(false);
        pausePanel.SetActive(false);
        player.InputEnabled = true;
    }

    // ---------- Guardar ----------

    private void OnSaveClicked()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            messageText.text = "No se encontró el sistema de guardado.";
            return;
        }

        int slot = GameSession.SaveSlot;

        // Si ahí hay otra partida distinta, se pide confirmación antes de pisarla
        if (manager.WouldOverwriteOtherRun(slot))
        {
            ShowConfirm("Ya existe otra partida guardada en este espacio.\n¿Quieres sobrescribirla?",
                        "Sobrescribir",
                        () => DoSave(slot));
            return;
        }

        DoSave(slot);
    }

    private void DoSave(int slot)
    {
        bool saved = SaveManager.Instance.Save(slot);
        messageText.text = saved ? "Partida guardada" : "No se pudo guardar la partida";
    }

    // ---------- Salir ----------

    private void OnExitClicked()
    {
        ShowConfirm("¿Salir al menú principal?\nSe perderá el progreso que no hayas guardado.",
                    "Salir",
                    ExitToMenu);
    }

    private void ExitToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // ---------- Confirmación (se reutiliza para guardar y para salir) ----------

    private void ShowConfirm(string message, string confirmLabel, Action onConfirm)
    {
        confirmText.text = message;
        confirmButtonLabel.text = confirmLabel;
        pendingConfirmAction = onConfirm;
        confirmPanel.SetActive(true);
    }

    private void OnConfirmClicked()
    {
        Action action = pendingConfirmAction;
        CloseConfirm();
        action?.Invoke();
    }

    private void CloseConfirm()
    {
        pendingConfirmAction = null;
        confirmPanel.SetActive(false);
    }
}