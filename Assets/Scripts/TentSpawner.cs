using System.Collections.Generic;
using UnityEngine;

public class TentSpawner : MonoBehaviour, ISaveable
{
    [Tooltip("Variantes visuales de carpa; se elige una al azar. Cada prefab debe tener un Place.")]
    [SerializeField] private Place[] tentPrefabs;

    [System.Serializable]
    private class TentEntry
    {
        public string zoneId;
        public string eventId;
        public string prefabName;
    }

    [System.Serializable]
    private class State
    {
        public List<TentEntry> tents = new List<TentEntry>();
    }

    private readonly List<TentEntry> entries = new List<TentEntry>();
    private readonly List<GameObject> spawnedTents = new List<GameObject>();

    public string SaveKey => "tents";
    public int RestoreOrder => -10;   // antes que PlacesSaveable: las carpas también son Place

    private void Start()
    {
        SpawnRandomTents();
    }

    private void SpawnRandomTents()
    {
        if (tentPrefabs == null || tentPrefabs.Length == 0)
        {
            Debug.LogError("[TentSpawner] No hay prefabs de carpa asignados.", this);
            return;
        }

        // El guardado necesita distinguir las zonas: avisa si hay nombres repetidos
        HashSet<string> zoneIds = new HashSet<string>();
        foreach (TentZone zone in TentZone.All)
        {
            if (!zoneIds.Add(zone.ZoneId))
                Debug.LogWarning($"[TentSpawner] Hay dos zonas con el mismo ZoneId '{zone.ZoneId}'.", zone);
        }

        // Cada evento sale como máximo una vez por partida
        List<string> availableEvents = PlaceCatalog.GetRandomEventIds();

        // Zonas en orden aleatorio, para que ninguna tenga ventaja si hay menos eventos que zonas
        List<TentZone> zones = new List<TentZone>(TentZone.All);
        for (int i = zones.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (zones[i], zones[j]) = (zones[j], zones[i]);
        }

        foreach (TentZone zone in zones)
        {
            if (availableEvents.Count == 0) break;
            if (Random.value > zone.SpawnChance) continue;

            int eventIndex = Random.Range(0, availableEvents.Count);
            string eventId = availableEvents[eventIndex];
            availableEvents.RemoveAt(eventIndex);

            Place prefab = tentPrefabs[Random.Range(0, tentPrefabs.Length)];
            SpawnTent(zone, prefab, eventId);
        }
    }

    // Lo usan tanto la aparición aleatoria como la restauración de un guardado
    private void SpawnTent(TentZone zone, Place prefab, string eventId)
    {
        Place tent = Instantiate(prefab, zone.transform.position, Quaternion.identity, zone.transform);
        tent.Initialize(eventId);

        spawnedTents.Add(tent.gameObject);
        entries.Add(new TentEntry { zoneId = zone.ZoneId, eventId = eventId, prefabName = prefab.name });
    }

    // ---------- Guardado ----------

    public string CaptureState()
    {
        return JsonUtility.ToJson(new State { tents = entries });
    }

    public void RestoreState(string json)
    {
        State state = JsonUtility.FromJson<State>(json);

        // Se descartan las carpas aleatorias de esta sesión y se reconstruyen las guardadas
        ClearTents();

        foreach (TentEntry entry in state.tents)
        {
            TentZone zone = FindZone(entry.zoneId);
            Place prefab = FindPrefab(entry.prefabName);

            if (zone == null || prefab == null || !PlaceCatalog.TryGet(entry.eventId, out _))
            {
                Debug.LogWarning($"[TentSpawner] No se pudo restaurar la carpa '{entry.eventId}' en la zona '{entry.zoneId}'.");
                continue;
            }

            SpawnTent(zone, prefab, entry.eventId);
        }
    }

    private void ClearTents()
    {
        foreach (GameObject tent in spawnedTents)
        {
            if (tent == null) continue;

            tent.SetActive(false);   // sale de Place.All de inmediato
            Destroy(tent);
        }

        spawnedTents.Clear();
        entries.Clear();
    }

    private static TentZone FindZone(string zoneId)
    {
        foreach (TentZone zone in TentZone.All)
        {
            if (zone.ZoneId == zoneId) return zone;
        }
        return null;
    }

    private Place FindPrefab(string prefabName)
    {
        foreach (Place prefab in tentPrefabs)
        {
            if (prefab.name == prefabName) return prefab;
        }
        return null;
    }
}