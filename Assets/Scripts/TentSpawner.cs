using System.Collections.Generic;
using UnityEngine;

// Al empezar la partida decide en qué zonas aparecen carpas y qué evento tiene cada una.
public class TentSpawner : MonoBehaviour
{
    [Tooltip("Variantes visuales de carpa; se elige una al azar. Cada prefab debe tener un Place.")]
    [SerializeField] private Place[] tentPrefabs;

    private void Start()
    {
        SpawnTents();
    }

    private void SpawnTents()
    {
        if (tentPrefabs == null || tentPrefabs.Length == 0)
        {
            Debug.LogError("[TentSpawner] No hay prefabs de carpa asignados.", this);
            return;
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
            Place tent = Instantiate(prefab, zone.transform.position, Quaternion.identity, zone.transform);
            tent.Initialize(eventId);
        }
    }
}