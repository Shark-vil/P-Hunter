public interface IPlayerStation
{
    void Idle(Player player);
    void Walk(Player player);
    void Sprint(Player player);
    void Jump(Player player);
}

public interface IPlayerNetworkStation
{
    void CmdIdle(Player player);
    void CmdWalk(Player player);
    void CmdSprint(Player player);
    void CmdJump(Player player);
}
