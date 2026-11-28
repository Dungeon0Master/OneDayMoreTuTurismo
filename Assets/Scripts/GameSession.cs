using System;
using UnityEngine;

public static class GameSession
{
    // Si se juega la escena directo en el editor, se usa este arquetipo
    public const string DefaultArchetypeId = "Turista";

    public static string ArchetypeId { get; private set; } = DefaultArchetypeId;

    // true cuando ya se eligió un arquetipo (desde el menú, una carga o el selector de pruebas)
    public static bool HasSelection { get; private set; }

    // Espacio de guardado en uso (por ahora siempre el 0)
    public static int SaveSlot { get; private set; } = 0;

    // true si el menú pidió cargar una partida al entrar a la escena del juego
    public static bool PendingLoad { get; private set; }

    // Identifica la partida en curso: sirve para no sobrescribir por accidente el guardado de otra partida
    private static string runId;

    public static string RunId
    {
        get
        {
            if (string.IsNullOrEmpty(runId))
                runId = Guid.NewGuid().ToString();
            return runId;
        }
    }

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
        HasSelection = true;
    }

    // Se llama al empezar una partida nueva desde el menú
    public static void StartNewRun()
    {
        runId = Guid.NewGuid().ToString();
        PendingLoad = false;
    }

    // Se llama desde el menú principal antes de cargar la escena del juego
    public static void RequestLoad(int slot)
    {
        SaveSlot = slot;
        PendingLoad = true;
    }

    // La escena del juego consulta (y consume) la petición de carga
    public static bool ConsumePendingLoad()
    {
        bool pending = PendingLoad;
        PendingLoad = false;
        return pending;
    }

    // Se llama al cargar un guardado: recupera la identidad de esa partida
    public static void RestoreRun(string savedRunId, string archetypeId)
    {
        runId = string.IsNullOrEmpty(savedRunId) ? Guid.NewGuid().ToString() : savedRunId;
        SelectArchetype(archetypeId);
    }
}