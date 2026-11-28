// Todo sistema que deba guardarse implementa esta interfaz.
// El SaveManager los encuentra solo; no hay que registrarlos en ningún lado.
public interface ISaveable
{
    string SaveKey { get; }          // nombre único y estable de la sección (no lo cambies después)
    int RestoreOrder { get; }        // al cargar, primero los de número menor
    string CaptureState();           // devuelve el estado como JSON
    void RestoreState(string json);  // aplica un estado guardado
}