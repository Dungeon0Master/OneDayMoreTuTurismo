using UnityEngine;
using UnityEngine.Tilemaps;

// Define la zona del mundo que muestra el mapa del kiosko.
// Mueve el objeto al centro del mapa y ajusta Size (o usa "Ajustar al Tilemap").
public class MapWorldArea : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(40f, 24f);

    [Header("Opcional: para la opción 'Ajustar al Tilemap'")]
    [SerializeField] private Tilemap tilemapToFit;

    public Vector2 Center => transform.position;
    public Vector2 Size => size;
    public Vector2 Min => Center - size / 2f;
    public Vector2 Max => Center + size / 2f;
    public float Aspect => size.x / size.y;

    private void OnValidate()
    {
        size.x = Mathf.Max(1f, size.x);
        size.y = Mathf.Max(1f, size.y);
    }

    [ContextMenu("Ajustar al Tilemap")]
    private void FitToTilemap()
    {
        if (tilemapToFit == null)
        {
            Debug.LogWarning("[MapWorldArea] Asigna un Tilemap en 'Tilemap To Fit'.", this);
            return;
        }

        Bounds local = tilemapToFit.localBounds;
        Vector3 worldMin = tilemapToFit.transform.TransformPoint(local.min);
        Vector3 worldMax = tilemapToFit.transform.TransformPoint(local.max);

        transform.position = (worldMin + worldMax) / 2f;
        size = worldMax - worldMin;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(transform);
#endif
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 0f));
    }
}