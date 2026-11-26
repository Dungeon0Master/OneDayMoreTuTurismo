using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private MenuNavigator navigator;
    [SerializeField] private GameObject mainMenuScreen;

    private void Update()
    {
        // Cualquier tecla o clic (menos Esc) pasa al menú principal
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            navigator.OpenAsRoot(mainMenuScreen);
    }
}