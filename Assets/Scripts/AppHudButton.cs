using UnityEngine;
using UnityEngine.UI;

public class AppHudButton : MonoBehaviour
{
    [SerializeField] private PlayerApp playerApp;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private Button appButton;

    private void Awake()
    {
        appButton.onClick.AddListener(OnAppButtonClicked);
    }

    private void OnEnable()
    {
        playerApp.OnInstalled += ShowButton;
    }

    private void OnDisable()
    {
        playerApp.OnInstalled -= ShowButton;
    }

    private void Start()
    {
        // Oculto hasta que se instale la app
        appButton.gameObject.SetActive(playerApp.IsInstalled);
    }

    private void ShowButton()
    {
        appButton.gameObject.SetActive(true);
    }

    private void OnAppButtonClicked()
    {
        if (TimeManager.Instance != null && TimeManager.Instance.IsDayOver) return;
        if (KioskMapUI.Instance == null) return;

        KioskMapUI.Instance.OpenFromApp(playerInteractor);
    }
}