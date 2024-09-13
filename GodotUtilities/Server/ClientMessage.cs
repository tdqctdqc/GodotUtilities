namespace GodotUtilities.Server;

public abstract class ClientMessage : Message
{
    protected ClientMessage(Guid clientGuid)
    {
        ClientGuid = clientGuid;
    }

    public Guid ClientGuid { get; private set; }
    public abstract void Handle(GameClient.GameClient client);
}