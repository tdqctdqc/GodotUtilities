using GodotUtilities.GameData;
using GodotUtilities.Server;

namespace GodotUtilities.Logic;

public class LogicKey
{
    private HostLogic _logic;

    public LogicKey(HostLogic logic, Data data)
    {
        _logic = logic;
        Data = data;
    }

    public Data Data { get; private set; }
    public void SendMessage(Message m)
    {
        _logic.HandleMessageFromLogic(m);
    }

    public void SendMessageToClient(ClientMessage m)
    {
        _logic.SendMessageToClient(m);
    }
}