using TMPro;
using UnityEngine;

public class PlaceInfoPanel : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Seguimiento")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    private RectTransform panelRect;
    private RectTransform canvasRect;
    private Canvas rootCanvas;
    private Camera mainCamera;

    private void Awake()
    {
        panelRect = (RectTransform)transform;
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        canvasRect = (RectTransform)rootCanvas.transform;
        mainCamera = Camera.main;

        // El borde inferior central del panel queda sobre el jugador
        panelRect.pivot = new Vector2(0.5f, 0f);
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    public void Show(PlaceInfo info, string message)
    {
        gameObject.SetActive(true);

        nameText.text = info.DisplayName;
        descriptionText.text = info.Description;
        costText.text = info.Cost > 0 ? $"Costo: -${info.Cost}" : "Gratis";
        timeText.text = info.TimeMinutes > 0 ? $"Tiempo: -{FormatTime(info.TimeMinutes)}" : "Sin gasto de tiempo";
        messageText.text = message;

        UpdatePosition();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdatePosition()
    {
        if (followTarget == null || mainCamera == null) return;

        Vector3 screenPoint = mainCamera.WorldToScreenPoint(followTarget.position + worldOffset);
        Camera uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPoint, uiCamera, out Vector3 worldPoint))
            panelRect.position = worldPoint;
    }

    private static string FormatTime(int minutes)
    {
        int h = minutes / 60;
        int m = minutes % 60;

        if (h == 0) return $"{m} min";
        if (m == 0) return $"{h} h";
        return $"{h} h {m} min";
    }
}