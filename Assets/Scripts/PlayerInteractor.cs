using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Referencias")]
    [SerializeField] private PlayerMoney playerMoney;
    [SerializeField] private PlayerSatisfaction playerSatisfaction;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerApp playerApp;
    [SerializeField] private PlaceInfoPanel panel;

    private IInteractable current;

    public KeyCode InteractKey => interactKey;
    public PlayerMoney Money => playerMoney;
    public PlayerSatisfaction Satisfaction => playerSatisfaction;
    public PlayerController Controller => playerController;
    public PlayerApp App => playerApp;
    public PlaceInfoPanel Panel => panel;

    private void Awake()
    {
        if (playerMoney == null)
            playerMoney = GetComponent<PlayerMoney>();
        if (playerSatisfaction == null)
            playerSatisfaction = GetComponent<PlayerSatisfaction>();
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        if (playerApp == null)
            playerApp = GetComponent<PlayerApp>();

        if (playerApp == null)
            Debug.LogError("[PlayerInteractor] Falta el componente PlayerApp en el jugador.", this);
    }

    private void Start()
    {
        panel.Hide();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        current = interactable;
        current.OnPlayerEnter(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null || interactable != current) return;

        current.OnPlayerExit(this);
        current = null;
    }

    private void Update()
    {
        if (current == null || !Input.GetKeyDown(interactKey)) return;
        if (TimeManager.Instance != null && TimeManager.Instance.IsDayOver) return;

        current.Interact(this);
    }
}