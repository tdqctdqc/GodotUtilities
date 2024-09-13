using GodotUtilities.Logic;

namespace GodotUtilities.Server;

public abstract class Command : Message
{
    public Guid CommandingPlayerGuid { get; private set; }

    public void SetCommandingPlayer(Guid guid)
    {
        CommandingPlayerGuid = guid;
    }

    public abstract void Handle(LogicKey key);
}