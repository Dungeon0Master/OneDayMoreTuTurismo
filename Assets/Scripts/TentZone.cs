using System.Collections.Generic;
using UnityEngine;

// Marca una zona donde puede aparecer una carpa. El TentSpawner decide si aparece o no.
public class TentZone : MonoBehaviour
{
    public static readonly List<TentZone> All = new List<TentZone>();

    [Range(0f, 1f)]
    [SerializeField] private float spawnChance = 0.5f;

    public float SpawnChance => spawnChance;

    private void OnEnable()
    {
        if (!All.Contains(this)) All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    // Cuadro naranja en la Scene para ver dónde están las zonas
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.9f);
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}