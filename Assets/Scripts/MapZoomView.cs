using UnityEngine;
using UnityEngine.UI;

// Zoom y desplazamiento del mapa del kiosko.
//  - Rueda del ratón sobre el mapa: zoom hacia el cursor
//  - Arrastrar el mapa o usar la barra lateral: desplazarse
//  - Botones + y -: zoom hacia el centro del visor
// Va en el mismo objeto que el ScrollRect (MapFrame).
public class MapZoomView : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button zoomInButton;
    [SerializeField] private Button zoomOutButton;

    [Header("Zoom")]
    [SerializeField] private float maxZoom = 4f;
    [SerializeField] private float wheelZoomSpeed = 0.5f;
    [SerializeField] private float buttonZoomStep = 0.5f;

    private RectTransform viewport;
    private RectTransform content;
    private Canvas rootCanvas;
    private float contentAspect = 1f;   // ancho / alto del mapa
    private float zoom = 1f;
    private bool initialized;

    private Camera UICamera =>
        rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

    // Se inicializa al primer uso, porque este objeto puede estar desactivado al arrancar
    private void Init()
    {
        if (initialized) return;
        initialized = true;

        viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
        content = scrollRect.content;
        rootCanvas = scrollRect.GetComponentInParent<Canvas>().rootCanvas;

        // La rueda se usa para el zoom, así que deja de desplazar
        scrollRect.scrollSensitivity = 0f;
        scrollRect.inertia = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        // El contenido se ancla arriba a la izquierda; su tamaño lo controla este script
        content.anchorMin = content.anchorMax = new Vector2(0f, 1f);
        content.pivot = new Vector2(0f, 1f);

        if (zoomInButton != null) zoomInButton.onClick.AddListener(ZoomIn);
        if (zoomOutButton != null) zoomOutButton.onClick.AddListener(ZoomOut);
    }

    // Proporción (ancho / alto) del mapa; la llama KioskMapUI
    public void Setup(float aspect)
    {
        Init();
        contentAspect = aspect;
    }

    // Zoom 1 y vista centrada en un punto del mapa (valores de 0 a 1; y hacia arriba)
    public void ResetView(Vector2 focusNormalized)
    {
        Init();
        Canvas.ForceUpdateCanvases();   // asegura que el visor ya tiene su tamaño

        zoom = 1f;
        ApplyContentSize();

        Vector2 viewSize = viewport.rect.size;
        Vector2 contentSize = content.sizeDelta;

        Vector2 position = new Vector2(
            viewSize.x / 2f - focusNormalized.x * contentSize.x,
            -viewSize.y / 2f + (1f - focusNormalized.y) * contentSize.y);

        content.anchoredPosition = ClampPosition(position);
    }

    public void ZoomIn()
    {
        ZoomAtCenter(zoom + buttonZoomStep);
    }

    public void ZoomOut()
    {
        ZoomAtCenter(zoom - buttonZoomStep);
    }

    private void Update()
    {
        float wheel = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(wheel, 0f)) return;

        Init();
        if (!RectTransformUtility.RectangleContainsScreenPoint(viewport, Input.mousePosition, UICamera)) return;

        ZoomAt(Input.mousePosition, zoom + wheel * wheelZoomSpeed);
    }

    // Zoom 1 = el mapa cubre todo el visor, sin huecos, en ambos ejes
    private void ApplyContentSize()
    {
        Vector2 viewSize = viewport.rect.size;

        float baseWidth = Mathf.Max(viewSize.x, viewSize.y * contentAspect);
        float width = baseWidth * zoom;

        content.sizeDelta = new Vector2(width, width / contentAspect);
    }

    private void ZoomAtCenter(float newZoom)
    {
        Init();

        Vector3 worldCenter = viewport.TransformPoint(viewport.rect.center);
        Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(UICamera, worldCenter);
        ZoomAt(screenCenter, newZoom);
    }

    // Cambia el zoom manteniendo fijo el punto del mapa que está bajo screenPoint
    private void ZoomAt(Vector2 screenPoint, float newZoom)
    {
        newZoom = Mathf.Clamp(newZoom, 1f, maxZoom);
        if (Mathf.Approximately(newZoom, zoom)) return;

        // Punto del mapa (0 a 1) que está bajo el cursor antes del cambio
        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, screenPoint, UICamera, out Vector2 localBefore);
        Vector2 normalized = Rect.PointToNormalized(content.rect, localBefore);

        zoom = newZoom;
        ApplyContentSize();

        // Dónde quedó ese mismo punto tras el cambio; se compensa moviendo el contenido
        Vector2 localAfter = Rect.NormalizedToPoint(content.rect, normalized);
        content.anchoredPosition = ClampPosition(content.anchoredPosition - (localAfter - localBefore));
    }

    // Evita que el mapa deje huecos vacíos en los bordes del visor
    private Vector2 ClampPosition(Vector2 position)
    {
        Vector2 viewSize = viewport.rect.size;
        Vector2 contentSize = content.sizeDelta;

        position.x = Mathf.Clamp(position.x, Mathf.Min(0f, viewSize.x - contentSize.x), 0f);
        position.y = Mathf.Clamp(position.y, 0f, Mathf.Max(0f, contentSize.y - viewSize.y));
        return position;
    }
}