/// <summary>
/// Player State Interface.
/// </summary>
public interface IPlayerState
{
    /// <summary>
    /// Called when the player is walking.
    /// </summary>
    /// <param name="player">Player component</param>
    /// <param name="horizontal">Horizontal direction</param>
    /// <param name="vertical">Vertical direction</param>
    void Walk(Player player, float horizontal, float vertical);

    /// <summary>
    /// Called when the player is sprint.
    /// </summary>
    /// <param name="player">Player component</param>
    /// <param name="horizontal">Horizontal direction</param>
    /// <param name="vertical">Vertical direction</param>
    void Sprint(Player player, float horizontal, float vertical);

    /// <summary>
    /// Called when a player jump.
    /// </summary>
    /// <param name="player">Player component</param>
    void Jump(Player player);
}