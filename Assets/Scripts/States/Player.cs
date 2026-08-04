using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public PlayerInputSet input { get; private set; }

    private StateMachine stateMachine;

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;

    public float attackVelocityDuration = 0.1f;
    public float comboResetTime = 1f;

    private Coroutine queuedAttackCo;

    [Header("Movement details")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public Vector2 wallJumpForce = new Vector2(6f, 12f);

    [Range(0f, 1f)]
    public float inAirMoveMultiplier = 0.8f;

    [Range(0f, 1f)]
    public float wallSlideSlowMultiplier = 0.4f;

    [Space]
    public float dashDuration = 0.25f;
    public float dashSpeed = 20f;

    private bool facingRight = true;

    public int facingDir { get; private set; } = 1;
    public Vector2 moveInput { get; private set; }

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance = 1.4f;
    [SerializeField] private float wallCheckDistance = 0.4f;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    private void Awake()
    {
        Debug.Log("PLAYER AWAKE RUNNING", gameObject);

        /*
         * The Animator should exist on the child object containing
         * the SpriteRenderer and AC_Player controller.
         */
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (anim == null)
            Debug.LogError("No Animator was found in Player or its children.", gameObject);

        if (rb == null)
            Debug.LogError("No Rigidbody2D was found on the Player.", gameObject);

        stateMachine = new StateMachine();
        input = new PlayerInputSet();

        // These names must exactly match the Animator parameters.
        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");

        jumpState = new Player_JumpState(
            this,
            stateMachine,
            "JumpFall"
        );

        fallState = new Player_FallState(
            this,
            stateMachine,
            "JumpFall"
        );

        wallSlideState = new Player_WallSlideState(
            this,
            stateMachine,
            "WallSlide"
        );

        wallJumpState = new Player_WallJumpState(
            this,
            stateMachine,
            "JumpFall"
        );

        dashState = new Player_DashState(
            this,
            stateMachine,
            "Dash"
        );

        basicAttackState = new Player_BasicAttackState(
            this,
            stateMachine,
            "BasicAttack"
        );

        jumpAttackState = new Player_JumpAttackState(
            this,
            stateMachine,
            "JumpAttack"
        );
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCanceled;

        input.Disable();
    }

    private void Start()
    {
        Debug.Log($"Animator found: {anim}", anim);

        if (anim != null)
        {
            Debug.Log(
                $"Animator Controller: {anim.runtimeAnimatorController}",
                anim
            );
        }

        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    private void OnMovementPerformed(
        UnityEngine.InputSystem.InputAction.CallbackContext context
    )
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(
        UnityEngine.InputSystem.InputAction.CallbackContext context
    )
    {
        moveInput = Vector2.zero;
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();

        stateMachine.ChangeState(basicAttackState);
        queuedAttackCo = null;
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0f && !facingRight)
            Flip();
        else if (xVelocity < 0f && facingRight)
            Flip();
    }

    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);

        facingRight = !facingRight;
        facingDir *= -1;
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            whatIsGround
        );

        if (primaryWallCheck == null || secondaryWallCheck == null)
        {
            wallDetected = false;
            return;
        }

        bool primaryCheck = Physics2D.Raycast(
            primaryWallCheck.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround
        );

        bool secondaryCheck = Physics2D.Raycast(
            secondaryWallCheck.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround
        );

        wallDetected = primaryCheck && secondaryCheck;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundCheckDistance
        );

        if (primaryWallCheck != null)
        {
            Gizmos.DrawLine(
                primaryWallCheck.position,
                primaryWallCheck.position +
                Vector3.right * facingDir * wallCheckDistance
            );
        }

        if (secondaryWallCheck != null)
        {
            Gizmos.DrawLine(
                secondaryWallCheck.position,
                secondaryWallCheck.position +
                Vector3.right * facingDir * wallCheckDistance
            );
        }
    }
}