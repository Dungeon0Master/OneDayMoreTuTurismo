using System.Collections.Generic;
using UnityEngine;

public enum PlaceUseResult { Success, NotEnoughMoney, NotEnoughTime, AlreadyUsed }

[RequireComponent(typeof(Collider2D))]
public class Place : MonoBehaviour, IInteractable
{
    // Todos los lugares activos de la escena (el mapa del kiosko los usa)
    public static readonly List<Place> All = new List<Place>();

    public string Id => placeId;
    public bool Used => used;

    public void MarkUsed()
    {
        used = true;
    }
    [Header("Datos")]
    [SerializeField, Delayed] private string placeId;
    [SerializeField] private bool singleUse = true;

    [Header("Mapa")]
    [Tooltip("Opcional: punto donde aparece el pin en el mapa. Si está vacío usa este objeto.")]
    [SerializeField] private Transform mapAnchor;

    private PlaceInfo info;
    private bool used;

    public PlaceInfo Info => info;
    public bool CanUse => !(singleUse && used);
    public Vector3 MapPosition => mapAnchor != null ? mapAnchor.position : transform.position;

private void Awake()
{
    // Las carpas se crean sin Id y lo reciben después con Initialize()
    if (!string.IsNullOrEmpty(placeId))
        ResolveInfo();
}

// Para lugares que se crean durante el juego (carpas aleatorias)
public void Initialize(string id)
{
    placeId = id;
    ResolveInfo();
}

private void ResolveInfo()
{
    if (!PlaceCatalog.TryGet(placeId, out info))
        Debug.LogError($"[Place] '{name}' tiene el Id '{placeId}', que no existe en PlaceCatalog.", this);
}

    private void OnEnable()
    {
        if (!All.Contains(this)) All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(placeId) && !PlaceCatalog.TryGet(placeId, out _))
            Debug.LogWarning($"[Place] El Id '{placeId}' no existe en PlaceCatalog.", this);
    }

    // ---------- IInteractable ----------

    public void OnPlayerEnter(PlayerInteractor player)
    {
        if (info == null) return;

        string hint = CanUse ? $"[{player.InteractKey}] Visitar" : "Ya visitado";
        player.Panel.Show(info, hint);
    }

    public void OnPlayerExit(PlayerInteractor player)
    {
        player.Panel.Hide();
    }

    public void Interact(PlayerInteractor player)
    {
        if (info == null) return;

       PlaceUseResult result = TryUse(player.Money, player.Satisfaction, player.Energy, out int gained);

        string message;
        switch (result)
        {
            case PlaceUseResult.Success:        message = $"¡Visitado! +{gained} de satisfacción"; break;
            case PlaceUseResult.NotEnoughMoney: message = "No tienes suficiente dinero"; break;
            case PlaceUseResult.NotEnoughTime:  message = "No te alcanza el tiempo del día"; break;
            default:                            message = "Ya visitaste este lugar"; break;
        }

        player.Panel.Show(info, message);
    }

    // ---------- Lógica de uso ----------

    public PlaceUseResult TryUse(PlayerMoney money, PlayerSatisfaction satisfaction, PlayerEnergy energy, out int satisfactionGained)
        {
            satisfactionGained = 0;

            if (!CanUse) return PlaceUseResult.AlreadyUsed;

            if (TimeManager.Instance != null && !TimeManager.Instance.HasTimeFor(info.TimeMinutes))
                return PlaceUseResult.NotEnoughTime;

            if (!money.TrySpend(info.Cost)) return PlaceUseResult.NotEnoughMoney;

            satisfactionGained = satisfaction.AddFromPlace(info);
            ApplyEnergy(energy);

            if (TimeManager.Instance != null)
                TimeManager.Instance.AdvanceMinutes(info.TimeMinutes);

            used = true;
            return PlaceUseResult.Success;
        }

    private void ApplyEnergy(PlayerEnergy energy)
    {
        if (energy == null || info.Energy == 0) return;

        if (info.Energy > 0)
            energy.RecoverEnergy(info.Energy);
        else
            energy.ConsumeEnergy(-info.Energy);
    }
}