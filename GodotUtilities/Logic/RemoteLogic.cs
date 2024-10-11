using GodotUtilities.Server;

namespace GodotUtilities.Logic;

public class RemoteLogic : ILogic
{
    
    public void HandleMessageFromServer(Message m)
    {
        throw new NotImplementedException();
    }

    public void HandleMessageFromClient(Message m)
    {
        throw new NotImplementedException();
    }

    public Action<ClientMessage> MessageForLocalClient { get; set; }
    public void Process(double delta)
    {
        throw new NotImplementedException();
    }
}