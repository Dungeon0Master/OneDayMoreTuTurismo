using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArchetypeSelectScreen : MonoBehaviour
{
    [SerializeField] private MenuNavigator navigator;

    [Header("Lista de arquetipos")]
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private ArchetypeOption optionPrefab;

    [Header("Panel de descripción")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Botones")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;

    [Header("Juego")]
    [SerializeField] private string gameSceneName = "Test";

    private readonly List<ArchetypeOption> options = new List<ArchetypeOption>();
    private string selectedId;

    private void Awake()
    {
        BuildOptions();

        startButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(navigator.Back);
    }

    // Al abrir la pantalla siempre hay un arquetipo seleccionado
    private void OnEnable()
    {
        if (options.Count == 0) return;

        Select(string.IsNullOrEmpty(selectedId) ? options[0].Id : selectedId);
    }

    // Una opción por cada arquetipo del catálogo, en el orden en que están escritos
    private void BuildOptions()
    {
        foreach (KeyValuePair<string, ArchetypeInfo> entry in ArchetypeCatalog.All)
        {
            ArchetypeOption option = Instantiate(optionPrefab, optionsContainer);
            option.Setup(entry.Key, entry.Value, Select);
            options.Add(option);
        }
    }

    private void Select(string id)
    {
        if (!ArchetypeCatalog.TryGet(id, out ArchetypeInfo info)) return;

        selectedId = id;

        foreach (ArchetypeOption option in options)
            option.SetSelected(option.Id == id);

        nameText.text = info.DisplayName;
        descriptionText.text = info.Description;
        moneyText.text = $"Dinero inicial: ${info.StartingMoney}";
    }

    private void StartGame()
    {
        if (string.IsNullOrEmpty(selectedId)) return;

        startButton.interactable = false;   // evita clics dobles mientras carga

        GameSession.StartNewRun();
        GameSession.SelectArchetype(selectedId);
        SceneManager.LoadScene(gameSceneName);
    }
}