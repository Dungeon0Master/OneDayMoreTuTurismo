using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject[] allScreens;

    private readonly Stack<GameObject> history = new Stack<GameObject>();
    private GameObject current;

    private void Awake()
    {
        // Todas ocultas, y solo se muestra la inicial
        foreach (GameObject screen in allScreens)
            screen.SetActive(false);

        current = startScreen;
        current.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    // Abre una pantalla y guarda la actual para poder volver
    public void Open(GameObject screen)
    {
        if (screen == current) return;

        history.Push(current);
        Show(screen);
    }

    // Abre una pantalla como nueva raíz: ya no se puede volver a la anterior
    public void OpenAsRoot(GameObject screen)
    {
        history.Clear();
        Show(screen);
    }

    public void Back()
    {
        if (history.Count == 0) return;

        Show(history.Pop());
    }

    private void Show(GameObject screen)
    {
        current.SetActive(false);
        current = screen;
        current.SetActive(true);
    }
}