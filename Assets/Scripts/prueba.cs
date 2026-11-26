using UnityEngine;

// SOLO PARA PRUEBAS: simula la elección del menú cuando juegas la escena del juego directamente
[DefaultExecutionOrder(-200)]
public class prueba : MonoBehaviour
{
    [SerializeField] private string archetypeId = "Turista";

    private void Awake()
    {
        // Si ya se eligió un arquetipo en el menú, no lo pisa
        if (GameSession.HasSelection) return;

        GameSession.SelectArchetype(archetypeId);
    }
}