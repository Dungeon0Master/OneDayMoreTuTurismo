using UnityEngine;

public static class GameSession
{
    // Si se juega la escena directo en el editor, se usa este arquetipo
    public const string DefaultArchetypeId = "Turista";

    public static string ArchetypeId { get; private set; } = DefaultArchetypeId;

    public static ArchetypeInfo Archetype
    {
        get
        {
            if (ArchetypeCatalog.TryGet(ArchetypeId, out ArchetypeInfo info))
                return info;

            ArchetypeCatalog.TryGet(DefaultArchetypeId, out info);
            return info;
        }
    }

    public static void SelectArchetype(string id)
    {
        if (!ArchetypeCatalog.TryGet(id, out _))
        {
            Debug.LogError($"[GameSession] El arquetipo '{id}' no existe en ArchetypeCatalog.");
            return;
        }
        ArchetypeId = id;
    }
} 