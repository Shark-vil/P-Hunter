using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Player Components")]

    [SerializeField]
    [Tooltip("Main player controller")]
    // Main player controller
    protected private Player PlayerController;

    [SerializeField]
    [Tooltip("Player animator")]
    // Player animator
    protected private Animator PlayerAnimator;

    [Header("Playermodel Skin")]

    [SerializeField]
    [Tooltip("Player model skin meshes")]
    // Player model skin meshes
    protected private SkinnedMeshRenderer[] PlayerMeshes;

    [Header("Player Animator States")]

    [SerializeField]
    [Tooltip("Player is movement state")]
    // Player is movement state
    protected internal bool IsMovement;

    [SerializeField]
    [Tooltip("Player is walk state")]
    // Player is walk state
    protected internal bool IsWalk;

    [SerializeField]
    [Tooltip("Player is sprint state")]
    // Player is sprint state
    protected internal bool IsSprint;

    [SerializeField]
    [Tooltip("Player is jump state")]
    // Player is jump state
    protected internal bool IsJump;

    private void Start()
    {
        //DisableMainMeshes();
    }

    /// <summary>
    /// Locally disables the player’s model if it belongs to him.
    /// </summary>
    private void DisableMainMeshes()
    {
        if (PlayerController.isLocalPlayer)
            foreach (var PlayerMesh in PlayerMeshes)
                PlayerMesh.enabled = false;
    }

    private void LateUpdate()
    {
        if (!PlayerController.isLocalPlayer)
            return;

        if (IsJump)
        {
            SetJump(true);
            return;
        }
        else
            SetJump(false);

        if (IsMovement)
        {
            SetIdle(false);
            SetMovement(true);

            if (IsWalk)
            {
                SetWalk(true);
                SetSprint(false);
            }
            else
            {
                SetSprint(true);
                SetWalk(false);
            }
        }
        else
        {
            SetIdle(true);
            SetMovement(false);
            SetWalk(false);
            SetSprint(false);
        }
    }

    private void SetIdle(bool State)
    {
        PlayerAnimator.SetBool("IsIdle", State);
    }

    private void SetMovement(bool State)
    {
        PlayerAnimator.SetBool("IsMovement", State);
    }

    private void SetWalk(bool State)
    {
        PlayerAnimator.SetBool("IsWalk", State);
    }

    private void SetSprint(bool State)
    {
        PlayerAnimator.SetBool("IsSprint", State);
    }

    private void SetJump(bool State)
    {
        PlayerAnimator.SetBool("IsJump", State);
    }

    /**
     * PUBLIC
     */

    internal void SetIdleState()
    {
        if (PlayerController.IsJump)
            return;

        IsJump = false;
        IsMovement = false;
        IsSprint = false;
        IsWalk = false;
    }

    internal void SetMovementState()
    {
        if (PlayerController.IsJump)
            return;

        IsJump = false;
        IsMovement = true;
    }

    internal void SetSprintState()
    {
        if (PlayerController.IsJump)
            return;

        IsSprint = true;
        IsWalk = false;
    }

    internal void SetWalkState()
    {
        if (PlayerController.IsJump)
            return;

        IsWalk = true;
        IsSprint = false;
    }

    internal void SetJumpState()
    {
        IsJump = true;
    }
}
