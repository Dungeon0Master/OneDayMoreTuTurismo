using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Colisiones")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private Vector2 offsetPuntoMovimiento;
    [SerializeField] private float radioColision = 0.2f;

    [Header("Referencias del Personaje")]
    [SerializeField] private PlayerEnergy playerEnergy;

    private Vector2 puntoMovimiento;
    private Vector2 input;
    private bool isMoving = false;
    private bool isRunningCurrentStep = false;

    
   [Header("Animación")]
    [SerializeField]private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private static readonly int IdleDownState = Animator.StringToHash("PlayerIdleDown");
    private static readonly int IdleUpState   = Animator.StringToHash("PlayerIdleUP");
    private static readonly int IdleSideState = Animator.StringToHash("PlayerIdleSide");
    private static readonly int MoveDownState = Animator.StringToHash("PlayerMove");
    private static readonly int MoveUpState   = Animator.StringToHash("PlayerMoveUp");
    private static readonly int MoveSideState = Animator.StringToHash("PlayerMoveSides");

    private enum Direccion { Abajo, Arriba, Lado }
    private Direccion direccionActual = Direccion.Abajo;
    private int estadoActual;


    public event Action<bool> OnStepCompleted;

    private void Awake()
    {
        if (playerEnergy == null)
            playerEnergy = GetComponent<PlayerEnergy>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        puntoMovimiento = transform.position;
        ActualizarDireccion(Vector2.down);
    }
private void Update()
{
    if (isMoving)
        Mover();

    // Si terminó la casilla (o ya estaba quieto), puede aceptar un nuevo paso
    if (!isMoving && !(TimeManager.Instance != null && TimeManager.Instance.IsDayOver))
    {
        LeerInput();

        if (input != Vector2.zero)
        {
            ActualizarDireccion(input); // mira hacia donde presionas, incluso si hay pared
            IntentarMover();
        }
    }

    ActualizarAnimacion();
}

    private void ActualizarDireccion(Vector2 dir)
    {
        if (dir.y > 0)      direccionActual = Direccion.Arriba;
        else if (dir.y < 0) direccionActual = Direccion.Abajo;
        else if (dir.x != 0)
        {
            direccionActual = Direccion.Lado;
            spriteRenderer.flipX = dir.x < 0;
        }
    }

    private void ActualizarAnimacion()
    {
        int nuevoEstado;

        switch (direccionActual)
        {
            case Direccion.Arriba:
                nuevoEstado = isMoving ? MoveUpState : IdleUpState;
                break;
            case Direccion.Lado:
                nuevoEstado = isMoving ? MoveSideState : IdleSideState;
                break;
            default:
                nuevoEstado = isMoving ? MoveDownState : IdleDownState;
                break;
        }

        // Solo cambia si el estado es distinto, así la animación no se reinicia
        if (nuevoEstado == estadoActual) return;

        estadoActual = nuevoEstado;
        animator.Play(nuevoEstado);
    }
    private void LeerInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0) y = 0;

        input = new Vector2(x, y);
    }

    private void IntentarMover()
    {
        Vector2 destino = puntoMovimiento + input;

        if (!Physics2D.OverlapCircle(destino + offsetPuntoMovimiento, radioColision, collisionLayer))
        {
            // Evaluamos al inicio del paso si quiere y puede correr[cite: 3]
            bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
            isRunningCurrentStep = wantsToRun && (playerEnergy == null || playerEnergy.CanRun);

            puntoMovimiento = destino;
            isMoving = true;
        }
    }

    private void Mover()
    {
        float velocidad = isRunningCurrentStep ? runSpeed : walkSpeed;

        transform.position = Vector2.MoveTowards(transform.position, puntoMovimiento, velocidad * Time.deltaTime);

        if ((Vector2)transform.position == puntoMovimiento)
        {
            isMoving = false;
            CompletarPaso();
        }
    }

    private void CompletarPaso()
    {
        if (playerEnergy != null)
            playerEnergy.ProcessStep(isRunningCurrentStep);

        if (TimeManager.Instance != null)
            TimeManager.Instance.RegisterStep(isRunningCurrentStep);

        OnStepCompleted?.Invoke(isRunningCurrentStep);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 origen = Application.isPlaying ? puntoMovimiento : (Vector2)transform.position;
        Gizmos.DrawWireSphere(origen + input + offsetPuntoMovimiento, radioColision);
    }
}