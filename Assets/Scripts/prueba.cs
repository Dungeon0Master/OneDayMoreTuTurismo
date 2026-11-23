using UnityEngine;

// SOLO PARA PRUEBAS: simula la elección del menú de arquetipos
[DefaultExecutionOrder(-200)]
public class ArchetypeTestSelector : MonoBehaviour
{
    [SerializeField] private string archetypeId = "Turista";

    private void Awake()
    {
        GameSession.SelectArchetype(archetypeId);
    }
}
