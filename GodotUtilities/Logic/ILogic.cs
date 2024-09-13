using GodotUtilities.Server;

namespace GodotUtilities.Logic;

public interface ILogic
{
    void HandleMessageFromServer(Message m);
    void HandleMessageFromClient(Message m);
    Action<ClientMessage> MessageForLocalClient { get; set; }
    void Process(double delta);
}