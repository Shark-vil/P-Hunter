using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStateDefault : IPlayerState
{
    public void Walk(Player player, float horizontal, float vertical)
    {
        PlayerMovementToVelocity(player, horizontal, vertical, player.WalkSpeed);

        if (!player.IsWalk)
        {
            player.IsWalk = true;
            player.IsSprint = false;
        }
    }

    public void Sprint(Player player, float horizontal, float vertical)
    {
        PlayerMovementToVelocity(player, horizontal, vertical, player.SprintSpeed);

        if (!player.IsSprint)
        {
            player.IsSprint = true;
            player.IsWalk = false;
        }
    }

    public void Jump(Player player)
    {
        player.GroundDetectedCooldown = Time.time + 1.5f;
        player.StartCoroutine(JumpDelay(player));
        player.IsJump = true;
    }

    /// <summary>
    /// Moves a player’s physical component with a specific speed and direction.
    /// </summary>
    /// <param name="Horizontal">Horizontal direction</param>
    /// <param name="Vertical">Vertical direction</param>
    /// <param name="Speed">Move speed</param>
    private void PlayerMovementToVelocity(Player player, float Horizontal, float Vertical, float Speed)
    {
        Camera MainCamera = player.MainCamera;
        Rigidbody Phys = player.Phys;

        Vector3 MoveForward = new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z).normalized;
        Vector3 MoveRight = new Vector3(MainCamera.transform.right.x, 0, MainCamera.transform.right.z).normalized;

        Vector3 MoveHorizontal = MoveRight * Horizontal;
        Vector3 MoveVertical = MoveForward * Vertical;

        Vector3 NewVelocity = MoveHorizontal + MoveVertical * Speed * Time.deltaTime;
        NewVelocity.y = Phys.velocity.y;

        Phys.velocity = NewVelocity;

        if (!player.IsMovement)
            player.IsMovement = true;
    }

    /// <summary>
    /// Causes a jump with a delay of 0.5 seconds.
    /// </summary>
    /// <param name="player">Player component</param>
    /// <returns>Coroutine process</returns>
    private IEnumerator JumpDelay(Player player)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            player.Phys.AddForce(player.transform.up * player.JumpPower, ForceMode.Impulse);
            yield break;
        }
    }
}
