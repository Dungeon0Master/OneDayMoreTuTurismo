using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveStorage
{
    public const int CurrentVersion = 1;

    public static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot{slot}.json");
    }

    private static string TempPath(int slot) { return GetPath(slot) + ".tmp"; }
    private static string BackupPath(int slot) { return GetPath(slot) + ".bak"; }

    public static bool Write(int slot, SaveFile file)
    {
        try
        {
            file.version = CurrentVersion;
            file.checksum = ComputeChecksum(file);
            string json = JsonUtility.ToJson(file, true);

            string path = GetPath(slot);
            string temp = TempPath(slot);

            // 1) Se escribe en un archivo temporal, nunca directo sobre el guardado bueno
            File.WriteAllText(temp, json, Encoding.UTF8);

            // 2) Se comprueba que lo escrito se puede leer y es válido
            if (!TryParse(File.ReadAllText(temp, Encoding.UTF8), out _))
            {
                Debug.LogError("[SaveStorage] El archivo temporal quedó inválido; no se tocó el guardado anterior.");
                File.Delete(temp);
                return false;
            }

            // 3) El guardado anterior pasa a ser la copia de seguridad (solo si él mismo es válido)
            if (File.Exists(path) && ReadFile(path) != null)
                File.Copy(path, BackupPath(slot), true);

            // 4) Recién ahora se reemplaza el guardado
            File.Copy(temp, path, true);
            File.Delete(temp);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[SaveStorage] No se pudo guardar: {exception.Message}");
            return false;
        }
    }

    // Devuelve el guardado válido más reciente: el principal o, si está dañado, la copia de seguridad
    public static SaveFile Read(int slot)
    {
        SaveFile file = ReadFile(GetPath(slot));
        if (file != null) return file;

        file = ReadFile(BackupPath(slot));
        if (file != null)
            Debug.LogWarning("[SaveStorage] El guardado principal no es válido; se usó la copia de seguridad.");

        return file;
    }

    public static bool HasValidSave(int slot)
    {
        return Read(slot) != null;
    }

    public static void Delete(int slot)
    {
        foreach (string path in new[] { GetPath(slot), BackupPath(slot), TempPath(slot) })
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static SaveFile ReadFile(string path)
    {
        try
        {
            if (!File.Exists(path)) return null;

            string json = File.ReadAllText(path, Encoding.UTF8);
            return TryParse(json, out SaveFile file) ? file : null;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[SaveStorage] No se pudo leer '{path}': {exception.Message}");
            return null;
        }
    }

    private static bool TryParse(string json, out SaveFile file)
    {
        file = null;

        try
        {
            SaveFile parsed = JsonUtility.FromJson<SaveFile>(json);
            if (parsed == null || parsed.sections == null) return false;

            if (parsed.version < 1 || parsed.version > CurrentVersion)
            {
                Debug.LogWarning($"[SaveStorage] Versión de guardado no soportada: {parsed.version}.");
                return false;
            }

            if (parsed.checksum != ComputeChecksum(parsed))
            {
                Debug.LogWarning("[SaveStorage] El checksum no coincide: el archivo está dañado.");
                return false;
            }

            file = parsed;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string ComputeChecksum(SaveFile file)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(file.version).Append('|')
               .Append(file.savedAtUtc).Append('|')
               .Append(file.runId).Append('|')
               .Append(file.archetypeId);

        foreach (SaveSection section in file.sections)
            builder.Append('|').Append(section.key).Append('=').Append(section.json);

        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));
            return Convert.ToBase64String(hash);
        }
    }
}