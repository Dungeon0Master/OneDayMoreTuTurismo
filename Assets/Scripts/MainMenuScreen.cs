using UnityEngine;
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
    [SerializeField] private GameObject loadGameScreen;
    [SerializeField] private GameObject scoresScreen;
    [SerializeField] private GameObject creditsScreen;

    private void Awake()
    {
        Bind(newGameButton, newGameScreen);
        Bind(loadGameButton, loadGameScreen);
        Bind(scoresButton, scoresScreen);
        Bind(creditsButton, creditsScreen);
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
}