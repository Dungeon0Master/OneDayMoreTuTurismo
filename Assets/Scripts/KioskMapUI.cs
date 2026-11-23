using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KioskMapUI : MonoBehaviour
{
    public static KioskMapUI Instance { get; private set; }

    [Header("Referencias de UI")]
    [SerializeField] private GameObject mapRoot;
    [SerializeField] private RawImage mapImage;          // muestra lo que ve la cámara del mapa
    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private Button closeButton;
    [SerializeField] private PlaceMapIcon iconPrefab;
    [SerializeField] private CategoryIconLibrary categoryIcons;

    [Header("Mapa del mundo")]
    [SerializeField] private MapWorldArea worldArea;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private int textureHeight = 1080;

    private readonly List<GameObject> spawnedIcons = new List<GameObject>();
    private RenderTexture mapTexture;
    private PlayerInteractor player;

    public bool IsOpen { get; private set; }

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
        float aspect = worldArea.Aspect;

        mapTexture = new RenderTexture(Mathf.RoundToInt(textureHeight * aspect), textureHeight, 16);
        mapTexture.filterMode = FilterMode.Point;   // pixel art sin borrosidad

        mapCamera.orthographic = true;
        mapCamera.orthographicSize = worldArea.Size.y / 2f;
        mapCamera.transform.position = new Vector3(worldArea.Center.x, worldArea.Center.y, -10f);
        mapCamera.targetTexture = mapTexture;
        mapCamera.enabled = false;   // solo se activa mientras el mapa está abierto

        mapImage.texture = mapTexture;

        AspectRatioFitter fitter = mapImage.GetComponent<AspectRatioFitter>();
        if (fitter == null)
        {
            Debug.LogError("[KioskMapUI] El objeto del mapa necesita un AspectRatioFitter.", mapImage);
            return;
        }
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = aspect;
    }

    public void Open(PlayerInteractor playerInteractor)
    {
        if (IsOpen) return;

        player = playerInteractor;
        IsOpen = true;
        player.Controller.InputEnabled = false;

        mapCamera.enabled = true;
        mapRoot.SetActive(true);

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
        playerMarker.SetAsLastSibling();   // dibujado encima de los pines
    }

    // Convierte una posición del mundo en una posición dentro del mapa
    private void SetMapPosition(RectTransform target, Vector3 worldPosition)
    {
        float x = Mathf.InverseLerp(worldArea.Min.x, worldArea.Max.x, worldPosition.x);
        float y = Mathf.InverseLerp(worldArea.Min.y, worldArea.Max.y, worldPosition.y);

        target.anchorMin = target.anchorMax = new Vector2(x, y);
        target.anchoredPosition = Vector2.zero;
    }

    private void OnPlaceSelected(Place place)
    {
        // Parte 2: aquí se abre el panel de detalle
        Debug.Log($"[KioskMap] Seleccionado: {place.Info.DisplayName}");
    }
}