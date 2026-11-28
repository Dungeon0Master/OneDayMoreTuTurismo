using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private MenuNavigator navigator;

    [Header("Botones")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button scoresButton;
    [SerializeField] private Button creditsButton;

    [Header("Pantallas destino (vacío = botón desactivado)")]
    [SerializeField] private GameObject newGameScreen;
    [SerializeField] private GameObject scoresScreen;
    [SerializeField] private GameObject creditsScreen;

    [Header("Cargar partida")]
    [Tooltip("Si se asigna, abre esa pantalla de espacios. Si está vacío, carga directo el espacio actual.")]
    [SerializeField] private GameObject loadGameScreen;
    [SerializeField] private string gameSceneName = "Tests";

    private void Awake()
    {
        Bind(newGameButton, newGameScreen);
        Bind(scoresButton, scoresScreen);
        Bind(creditsButton, creditsScreen);

        if (loadGameScreen != null)
            Bind(loadGameButton, loadGameScreen);
        else
            BindDirectLoad();
    }

    private void Bind(Button button, GameObject targetScreen)
    {
        if (targetScreen == null)
        {
            button.interactable = false;   // la pantalla aún no existe
            return;
        }

        button.onClick.AddListener(() => navigator.Open(targetScreen));
    }

    private void BindDirectLoad()
    {
        int slot = GameSession.SaveSlot;

        // Solo se activa si hay un guardado válido (principal o copia de seguridad)
        loadGameButton.interactable = SaveStorage.HasValidSave(slot);

        loadGameButton.onClick.AddListener(() =>
        {
            if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
            {
                Debug.LogError($"[MainMenuScreen] La escena '{gameSceneName}' no existe o no está en Build Settings.");
                return;
            }

            loadGameButton.interactable = false;
            GameSession.RequestLoad(slot);
            SceneManager.LoadScene(gameSceneName);
        });
    }
}