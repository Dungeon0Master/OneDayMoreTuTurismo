using System.Collections.Generic;

// Define qué datos tiene un lugar. Para agregar un campo nuevo, añádelo aquí.
public enum PlaceCategory { Cultura, Historia, Compras, Monumento, Comida, Tours }

public class PlaceInfo
{
    public string DisplayName;
    public string Description;
    public int Cost;          // dinero que resta
    public int TimeMinutes;  // minutos que resta
    public int Satisfaction;  // puntos que suma
    public PlaceCategory [] Categories; // categoría del lugar
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
            TimeMinutes = 120,
            Satisfaction = 200,
            Categories = new[] { PlaceCategory.Cultura, PlaceCategory.Historia}
        }},
        { "Catedral", new PlaceInfo {
            DisplayName = "Catedral",
            Description = "Monumento histórico del centro.",
            Cost = 0,
            TimeMinutes = 12,
            Satisfaction = 50,
            Categories = new[] { PlaceCategory.Historia}
        }},
        { "Mercado", new PlaceInfo {
            DisplayName = "Mercado",
            Description = "Artesanías y comida local.",
            Cost = 20,
            TimeMinutes = 130,
            Satisfaction = 80,
            Categories = new[] {PlaceCategory.Compras,}
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
}