using System.Collections.Generic;
using UnityEngine;

public class ArchetypeInfo
{
    public string DisplayName;
    public string Description;
    public int StartingMoney;
    public Dictionary<PlaceCategory, float> Multipliers;
    // Más adelante: objetivo especial (De negocios, De excursión, Turista)

    // Si el lugar tiene varias categorías, se usa la que más le guste al arquetipo
    public float GetMultiplier(PlaceCategory[] categories)
    {
        if (categories == null || categories.Length == 0) return 1f;

        float best = float.MinValue;
        foreach (PlaceCategory category in categories)
        {
            float value = 1f;
            if (Multipliers != null && Multipliers.TryGetValue(category, out float found))
                value = found;

            if (value > best) best = value;
        }
        return best;
    }
        public int CalculateSatisfaction(PlaceInfo info)
    {
        return Mathf.RoundToInt(info.Satisfaction * GetMultiplier(info.Categories));
    }
}

public static class ArchetypeCatalog
{
    private static readonly Dictionary<string, ArchetypeInfo> archetypes = new Dictionary<string, ArchetypeInfo>
    {
        { "Aventurero", new ArchetypeInfo {
            DisplayName = "Aventurero",
            Description = "Experiencias emocionantes y lugares nuevos.",
            StartingMoney = 100,
            Multipliers = new Dictionary<PlaceCategory, float> {
                { PlaceCategory.Tours, 2f },
            },
        }},
        { "Fashion", new ArchetypeInfo {
            DisplayName = "Fashion",
            Description = "Compras de alta calidad; gasta rápido.",
            StartingMoney = 150,
            Multipliers = new Dictionary<PlaceCategory, float> {
                { PlaceCategory.Compras, 2f },
            },
        }},
        { "Culto", new ArchetypeInfo {
            DisplayName = "Culto",
            Description = "Museos, teatros y galerías, a ritmo pausado.",
            StartingMoney = 100,
            Multipliers = new Dictionary<PlaceCategory, float> {
                { PlaceCategory.Cultura, 2f },
            },
        }},
        { "Turista", new ArchetypeInfo {
            DisplayName = "Turista",
            Description = "Visitar la mayor cantidad de lugares.",
            StartingMoney = 100,
            Multipliers = new Dictionary<PlaceCategory, float>(),
        }},
        { "Historiador", new ArchetypeInfo {
            DisplayName = "Historiador",
            Description = "Historia y, en segundo lugar, cultura.",
            StartingMoney = 100,
            Multipliers = new Dictionary<PlaceCategory, float> {
                { PlaceCategory.Historia, 2f },
                { PlaceCategory.Monumento, 2f },
                { PlaceCategory.Cultura, 1.5f },
            },
        }},
        { "Extranjero", new ArchetypeInfo {
            DisplayName = "Extranjero",
            Description = "Elementos de la cultura del país.",
            StartingMoney = 100,
            Multipliers = new Dictionary<PlaceCategory, float> {
                { PlaceCategory.Comida, 2f },
                { PlaceCategory.Cultura, 1.5f },
            },
        }},
    };

    public static bool TryGet(string id, out ArchetypeInfo info)
    {
        if (string.IsNullOrEmpty(id))
        {
            info = null;
            return false;
        }
        return archetypes.TryGetValue(id, out info);
    }
    // Todos los arquetipos, para armar el menú de selección
     public static IEnumerable<KeyValuePair<string, ArchetypeInfo>> All => archetypes;
}