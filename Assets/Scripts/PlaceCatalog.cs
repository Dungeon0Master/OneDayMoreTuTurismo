using System.Collections.Generic;

// Define qué datos tiene un lugar. Para agregar un campo nuevo, añádelo aquí.
public enum PlaceCategory { Cultura, Historia, Compras, Monumento, Comida, Tours }

public class PlaceInfo
{
    public string DisplayName;
    public string Description;
    public int Cost;          // dinero que resta
    public int Energy; // energía que resta o suma
    public int TimeMinutes;  // minutos que resta
    public int Satisfaction;  // puntos que suma
    public PlaceCategory [] Categories; // categoría del lugar
    public bool IsRandomEvent;   // true = puede salir en una carpa aleatoria
}

// Catálogo de lugares. La clave es el Id que se escribe en el componente Place.
public static class PlaceCatalog
{
    private static readonly Dictionary<string, PlaceInfo> places = new Dictionary<string, PlaceInfo>
    {
        { "Museo", new PlaceInfo {
            DisplayName = "Museo",
            Description = "Exposiciones de arte e historia de la región.",
            Cost = 30,
            Energy = -10,
            TimeMinutes = 120,
            Satisfaction = 200,
            Categories = new[] { PlaceCategory.Cultura, PlaceCategory.Historia}
        }},
        { "Catedral", new PlaceInfo {
            DisplayName = "Catedral",
            Description = "Monumento histórico del centro.",
            Cost = 0,
            Energy = 0,
            TimeMinutes = 12,
            Satisfaction = 50,
            Categories = new[] { PlaceCategory.Historia}
        }},
        { "Mercado", new PlaceInfo {
            DisplayName = "Mercado",
            Description = "Artesanías y comida local.",
            Cost = 20,
            Energy = 0,
            TimeMinutes = 130,
            Satisfaction = 80,
            Categories = new[] {PlaceCategory.Compras,}
        }},
        { "MercadoArtesanos", new PlaceInfo {
            DisplayName = "Mercado de artesanos",
            Description = "Artesanías hechas a mano por talleres locales.",
            Cost = 25,
            Energy = 0,
            TimeMinutes = 45,
            Satisfaction = 120,
            Categories = new[] { PlaceCategory.Cultura, PlaceCategory.Compras },
            IsRandomEvent = true,
        }},
        { "FeriaCarneEnJugo", new PlaceInfo {
            DisplayName = "Feria de la carne en su jugo",
            Description = "Un clásico de la gastronomía local.",
            Cost = 40,
            Energy = 100,
            TimeMinutes = 45,
            Satisfaction = 90,
            Categories = new[] { PlaceCategory.Comida },
            IsRandomEvent = true,
        }},
        { "ExpoAntiguedades", new PlaceInfo {
            DisplayName = "Exposición de antigüedades",
            Description = "Piezas históricas de colección.",
            Cost = 15,
            Energy = 0,
            TimeMinutes = 60,
            Satisfaction = 100,
            Categories = new[] { PlaceCategory.Historia },
            IsRandomEvent = true,
        }},
    };

    public static bool TryGet(string id, out PlaceInfo info)
    {
        if (string.IsNullOrEmpty(id))
        {
            info = null;
            return false;
        }
        return places.TryGetValue(id, out info);
    }
    // Ids de todos los eventos que pueden aparecer en carpas aleatorias
    public static List<string> GetRandomEventIds()
    {
        List<string> ids = new List<string>();

        foreach (KeyValuePair<string, PlaceInfo> entry in places)
        {
            if (entry.Value.IsRandomEvent)
                ids.Add(entry.Key);
        }
        return ids;
    }
}