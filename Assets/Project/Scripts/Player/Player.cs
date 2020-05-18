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
    protected private Rigidbody Physics;

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

    private void Start()
    {
        DisableNoMainCamera();
        StateController();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
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
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");

            if (Input.GetButton("Sprint"))
            {
                CmdSprint(Horizontal, Vertical);

                AnimatorController.IsSprint = true;
                AnimatorController.IsWalk = false;
            }
            else
            {
                CmdWalk(Horizontal, Vertical);

                AnimatorController.IsWalk = true;
                AnimatorController.IsSprint = false;
            }

            AnimatorController.IsMovement = true;
        }
        else
        {
            AnimatorController.IsMovement = false;
            AnimatorController.IsSprint = false;
            AnimatorController.IsWalk = false;
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
        NewVelocity.y = Physics.velocity.y;

        Physics.velocity = NewVelocity;

        if (!IsMovement)
            IsMovement = true;
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
