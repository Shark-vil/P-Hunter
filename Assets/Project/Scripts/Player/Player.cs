using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : NetworkBehaviour
{
    [Header("Player Components")]

    [SerializeField]
    [Tooltip("Player main camera")]
    // Player main camera
    protected private Camera MainCamera;

    [SerializeField]
    [Tooltip("Player rigidbody component")]
    // Player rigidbody component
    protected private Rigidbody Phys;

    [SerializeField]
    [Tooltip("Player network identity")]
    // Player network identity
    protected private NetworkIdentity identity;

    [SerializeField]
    [Tooltip("Player animator controller")]
    // Player animator controller
    protected private PlayerAnimatorController AnimatorController;

    [Header("Player Characteristics")]

    [SerializeField]
    [Tooltip("Player walking speed")]
    // Player walking speed
    protected internal float WalkSpeed;

    [SerializeField]
    [Tooltip("Player sprint speed")]
    // Player sprint speed
    protected internal float SprintSpeed;

    [SerializeField]
    [Tooltip("Player jump power")]
    // Player jump power
    protected internal float JumpPower;

    [Header("Player States")]

    [SyncVar]
    [SerializeField]
    [Tooltip("Player is movement state")]
    // Player is movement state
    protected internal bool IsMovement;

    [SyncVar]
    [SerializeField]
    [Tooltip("Player is walk state")]
    // Player is walk state
    protected internal bool IsWalk;

    [SyncVar]
    [SerializeField]
    [Tooltip("Player is sprint state")]
    // Player is sprint state
    protected internal bool IsSprint;

    [SerializeField]
    [Tooltip("Ground layers")]
    // Ground layers
    protected internal LayerMask LayerGround;
    [SyncVar]
    [SerializeField]
    [Tooltip("Player is ground state")]
    // Player is ground state
    protected internal bool IsGround;

    [SyncVar]
    [SerializeField]
    [Tooltip("Player is jump state")]
    // Player is jump state
    protected internal bool IsJump;

    private float GroundDetectedCooldown;

    private void Start()
    {
        DisableNoMainCamera();

        if (isServer)
        {
            StateController();
        }

        if (isClient && !isServer)
        {
            Phys.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            PlayerMovement();
        }
    }

    private void Update()
    {
        if (isServer)
        {
            GroundDetected();
        }
    }

    /// <summary>
    /// Disable a player’s camera if it doesn’t belong to him.
    /// </summary>
    private void DisableNoMainCamera()
    {
        if (!identity.isLocalPlayer)
            MainCamera.enabled = false;
    }

    /// <summary>
    /// Registers a helper to track player states.
    /// </summary>
    private void StateController()
    {
        StartCoroutine(IsMovementState());
    }

    /// <summary>
    /// Raises the player’s move event on the server if the corresponding keys are pressed.
    /// </summary>
    private void PlayerMovement()
    {
        if (!IsJump && Input.GetButtonDown("Jump"))
        {
            CmdJump();
        }

        if (IsJump)
            AnimatorController.SetJumpState();

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");

            if (Input.GetButton("Sprint"))
            {
                CmdSprint(Horizontal, Vertical);
                AnimatorController.SetSprintState();
            }
            else
            {
                CmdWalk(Horizontal, Vertical);
                AnimatorController.SetWalkState();
            }

            AnimatorController.SetMovementState();
        }
        else
        {
            AnimatorController.SetIdleState();
        }
    }

    /// <summary>
    /// Performs player walk  on the server side.
    /// </summary>
    /// <param name="Horizontal">Horizontal direction</param>
    /// <param name="Vertical">Vertical direction</param>
    [Command]
    private void CmdWalk(float Horizontal, float Vertical)
    {
        PlayerMovementToVelocity(Horizontal, Vertical, WalkSpeed);

        if (!IsWalk)
        {
            IsWalk = true;
            IsSprint = false;
        }
    }

    /// <summary>
    /// Performs player sprint on the server side.
    /// </summary>
    /// <param name="Horizontal">Horizontal direction</param>
    /// <param name="Vertical">Vertical direction</param>
    [Command]
    private void CmdSprint(float Horizontal, float Vertical)
    {
        PlayerMovementToVelocity(Horizontal, Vertical, SprintSpeed);

        if (!IsSprint)
        {
            IsSprint = true;
            IsWalk = false;
        }
    }

    [Command]
    private void CmdJump()
    {
        GroundDetectedCooldown = Time.time + 1.5f;

        StartCoroutine(JumpDelay());

        IsJump = true;
    }

    private IEnumerator JumpDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Phys.AddForce(transform.up * JumpPower, ForceMode.Impulse);
            yield break;
        }
    }

    /// <summary>
    /// Moves a player’s physical component with a specific speed and direction.
    /// </summary>
    /// <param name="Horizontal">Horizontal direction</param>
    /// <param name="Vertical">Vertical direction</param>
    /// <param name="Speed">Move speed</param>
    private void PlayerMovementToVelocity(float Horizontal, float Vertical, float Speed)
    {
        Vector3 MoveForward = new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z).normalized;
        Vector3 MoveRight = new Vector3(MainCamera.transform.right.x, 0, MainCamera.transform.right.z).normalized;

        Vector3 MoveHorizontal = MoveRight * Horizontal;
        Vector3 MoveVertical = MoveForward * Vertical;

        Vector3 NewVelocity = MoveHorizontal + MoveVertical * Speed * Time.deltaTime;
        NewVelocity.y = Phys.velocity.y;

        Phys.velocity = NewVelocity;

        if (!IsMovement)
            IsMovement = true;
    }

    /// <summary>
    /// Checks if the player is on the ground or not.
    /// </summary>
    private void GroundDetected()
    {
        if (GroundDetectedCooldown > Time.time)
        {
            IsGround = false;
            IsJump = true;
            return;
        }

        RaycastHit hit;

        Vector3 StartPos = transform.position + new Vector3(0, 0.3f);
        Vector3 Direction = -Vector3.up;
        float Lenght = 0.5f;
 
        if (Physics.Raycast(StartPos, Direction, out hit, Lenght, LayerGround))
        {
            IsJump = false;
            IsGround = true;
            Debug.DrawRay(StartPos, Direction * hit.distance, Color.green);
        }
        else
        {
            IsGround = false;
            Debug.DrawRay(StartPos, Direction * Lenght, Color.yellow);
        }
    }

    /// <summary>
    /// Watch state of a player’s movement.
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsMovementState()
    {
        Vector3 PlayerPosition = transform.position.normalized;

        while (true)
        {
            Vector3 SelectPlayerPosition = transform.position.normalized;

            if (PlayerPosition != SelectPlayerPosition)
            {
                PlayerPosition = SelectPlayerPosition;
            }
            else
            {
                if (IsMovement)
                {
                    IsMovement = false;
                    IsWalk = false;
                    IsSprint = false;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
