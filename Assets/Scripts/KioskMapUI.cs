using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KioskMapUI : MonoBehaviour
{
    public static KioskMapUI Instance { get; private set; }

    [Header("Referencias de UI")]
    [SerializeField] private GameObject mapRoot;
    [SerializeField] private RawImage mapImage;                      // es el "Content" del ScrollRect
    [SerializeField] private MapZoomView mapView;                    // NUEVO: zoom y desplazamiento
    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button installAppButton;
    [SerializeField] private PlaceMapIcon iconPrefab;
    [SerializeField] private CategoryIconLibrary categoryIcons;
    [SerializeField] private PlaceDetailPanel detailPanel;

    [Header("Mapa del mundo")]
    [SerializeField] private MapWorldArea worldArea;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private int pixelsPerUnit = 32;                 // NUEVO (reemplaza textureHeight)
    [SerializeField] private int maxTextureSize = 4096;              // NUEVO

    private readonly List<GameObject> spawnedIcons = new List<GameObject>();
    private RenderTexture mapTexture;
    private PlayerInteractor player;

    public bool IsOpen { get; private set; }

    // true si se abrió desde un kiosko, false si fue desde la app.
    // Más adelante: el viaje rápido solo estará disponible en modo kiosko.
    public bool OpenedFromKiosk { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupMapView();

        closeButton.onClick.AddListener(Close);
        installAppButton.onClick.AddListener(OnInstallAppClicked);
        mapRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (mapTexture != null)
        {
            mapTexture.Release();
            Destroy(mapTexture);
        }
    }

    private void Update()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    // Prepara la cámara para que encuadre exactamente el área del mundo
    private void SetupMapView()
    {
        // La textura se dimensiona según el tamaño real del mundo,
        // así un mapa alargado no queda angosto ni borroso al hacer zoom
        float largestSide = Mathf.Max(worldArea.Size.x, worldArea.Size.y) * pixelsPerUnit;
        float scale = Mathf.Min(1f, maxTextureSize / largestSide);

        int width = Mathf.Max(1, Mathf.RoundToInt(worldArea.Size.x * pixelsPerUnit * scale));
        int height = Mathf.Max(1, Mathf.RoundToInt(worldArea.Size.y * pixelsPerUnit * scale));

        mapTexture = new RenderTexture(width, height, 16);
        mapTexture.filterMode = FilterMode.Point;

        mapCamera.orthographic = true;
        mapCamera.orthographicSize = worldArea.Size.y / 2f;
        mapCamera.transform.position = new Vector3(worldArea.Center.x, worldArea.Center.y, -10f);
        mapCamera.targetTexture = mapTexture;
        mapCamera.enabled = false;

        mapImage.texture = mapTexture;

        mapView.Setup(worldArea.Aspect);                             // NUEVO
    }

    public void OpenFromKiosk(PlayerInteractor playerInteractor)
    {
        Open(playerInteractor, true);
    }

    public void OpenFromApp(PlayerInteractor playerInteractor)
    {
        Open(playerInteractor, false);
    }

    private void Open(PlayerInteractor playerInteractor, bool fromKiosk)
    {
        if (IsOpen) return;

        player = playerInteractor;
        IsOpen = true;
        OpenedFromKiosk = fromKiosk;
        player.Controller.InputEnabled = false;

        mapCamera.enabled = true;
        mapRoot.SetActive(true);
        detailPanel.Hide();
        RefreshInstallButton();

        // NUEVO: zoom 1 y vista centrada en el jugador
        mapView.ResetView(WorldToNormalized(player.transform.position));

        BuildPlaceIcons();
        PlacePlayerMarker();
    }

    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;
        mapCamera.enabled = false;
        mapRoot.SetActive(false);
        player.Controller.InputEnabled = true;
    }

    // Solo se ofrece instalar la app estando en un kiosko y si aún no está instalada
    private void RefreshInstallButton()
    {
        installAppButton.gameObject.SetActive(OpenedFromKiosk && !player.App.IsInstalled);
    }

    private void OnInstallAppClicked()
    {
        player.App.Install();
        RefreshInstallButton();
    }

    private void BuildPlaceIcons()
    {
        foreach (GameObject icon in spawnedIcons)
            Destroy(icon);
        spawnedIcons.Clear();

        foreach (Place place in Place.All)
        {
            if (place.Info == null) continue;

            PlaceMapIcon icon = Instantiate(iconPrefab, mapImage.rectTransform);
            icon.Setup(place, GetPinSprite(place), OnPlaceSelected);
            SetMapPosition((RectTransform)icon.transform, place.MapPosition);
            spawnedIcons.Add(icon.gameObject);
        }
    }

    private Sprite GetPinSprite(Place place)
    {
        PlaceCategory[] categories = place.Info.Categories;
        if (categoryIcons == null || categories == null || categories.Length == 0) return null;

        return categoryIcons.Get(categories[0]);
    }

    private void PlacePlayerMarker()
    {
        SetMapPosition(playerMarker, player.transform.position);
        playerMarker.SetAsLastSibling();
    }

    // NUEVO: posición del mundo convertida a un punto del mapa (0 a 1)
    private Vector2 WorldToNormalized(Vector3 worldPosition)
    {
        float x = Mathf.InverseLerp(worldArea.Min.x, worldArea.Max.x, worldPosition.x);
        float y = Mathf.InverseLerp(worldArea.Min.y, worldArea.Max.y, worldPosition.y);
        return new Vector2(x, y);
    }

    private void SetMapPosition(RectTransform target, Vector3 worldPosition)
    {
        target.anchorMin = target.anchorMax = WorldToNormalized(worldPosition);
        target.anchoredPosition = Vector2.zero;
    }

    private void OnPlaceSelected(Place place)
    {
        detailPanel.Show(place.Info);
    }
}