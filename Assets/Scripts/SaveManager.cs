using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Tooltip("Para no guardar a mitad de un paso")]
    [SerializeField] private PlayerController player;

    public bool CanSaveNow
    {
        get { return player == null || !player.IsMoving; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        // Solo si el menú pidió cargar una partida
        if (!GameSession.ConsumePendingLoad()) yield break;

        // Se espera un frame para que todos los objetos terminen de inicializarse
        yield return null;

        if (!Load(GameSession.SaveSlot))
            Debug.LogError("[SaveManager] No se pudo cargar la partida; se continúa con una partida nueva.");
    }

    public bool Save(int slot)
    {
        if (!CanSaveNow)
        {
            Debug.LogWarning("[SaveManager] No se puede guardar a mitad de un paso.");
            return false;
        }

        SaveFile file = new SaveFile
        {
            savedAtUtc = DateTime.UtcNow.ToString("o"),
            runId = GameSession.RunId,
            archetypeId = GameSession.ArchetypeId,
        };

        try
        {
            HashSet<string> usedKeys = new HashSet<string>();

            foreach (ISaveable saveable in FindSaveables())
            {
                if (!usedKeys.Add(saveable.SaveKey))
                {
                    Debug.LogError($"[SaveManager] Hay dos sistemas con la misma SaveKey '{saveable.SaveKey}'. No se guarda.");
                    return false;
                }

                file.sections.Add(new SaveSection { key = saveable.SaveKey, json = saveable.CaptureState() });
            }
        }
        catch (Exception exception)
        {
            // Si un sistema falla al capturarse no se guarda nada a medias
            Debug.LogError($"[SaveManager] Error al capturar el estado: {exception.Message}");
            return false;
        }

        return SaveStorage.Write(slot, file);
    }

    public bool Load(int slot)
    {
        SaveFile file = SaveStorage.Read(slot);
        if (file == null) return false;

        GameSession.RestoreRun(file.runId, file.archetypeId);

        List<ISaveable> saveables = FindSaveables();
        saveables.Sort((a, b) => a.RestoreOrder.CompareTo(b.RestoreOrder));

        foreach (ISaveable saveable in saveables)
        {
            SaveSection section = file.Find(saveable.SaveKey);
            if (section == null)
            {
                Debug.LogWarning($"[SaveManager] El guardado no tiene la sección '{saveable.SaveKey}'; se deja en su valor inicial.");
                continue;
            }

            try
            {
                saveable.RestoreState(section.json);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SaveManager] Error al restaurar '{saveable.SaveKey}': {exception.Message}");
            }
        }

        return true;
    }

    // true si guardar aquí pisaría el guardado de OTRA partida distinta a la actual
    public bool WouldOverwriteOtherRun(int slot)
    {
        SaveFile existing = SaveStorage.Read(slot);
        return existing != null && existing.runId != GameSession.RunId;
    }

    private static List<ISaveable> FindSaveables()
    {
        List<ISaveable> result = new List<ISaveable>();

        foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (behaviour is ISaveable saveable)
                result.Add(saveable);
        }
        return result;
    }

    // ---------- Pruebas desde el Inspector (⋮ del componente) ----------

    [ContextMenu("Test: guardar")]
    private void TestSave()
    {
        int slot = GameSession.SaveSlot;
        Debug.Log(Save(slot) ? $"Guardado en: {SaveStorage.GetPath(slot)}" : "Falló el guardado");
    }

    [ContextMenu("Test: cargar")]
    private void TestLoad()
    {
        Debug.Log(Load(GameSession.SaveSlot) ? "Partida cargada" : "No se pudo cargar");
    }

    [ContextMenu("Test: borrar guardado")]
    private void TestDelete()
    {
        SaveStorage.Delete(GameSession.SaveSlot);
        Debug.Log("Guardado borrado");
    }
}