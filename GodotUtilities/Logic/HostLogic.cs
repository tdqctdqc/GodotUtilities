using System.Collections.Concurrent;
using GodotUtilities.GameData;
using GodotUtilities.Server;

namespace GodotUtilities.Logic;

public abstract class HostLogic : ILogic
{
    public Guid HostPlayerGuid { get; private set; }
    public ConcurrentQueue<Command> CommandQueue { get; }
    public HostServer Server { get; private set; }
    public Action<ClientMessage> MessageForLocalClient { get; set; }
    public abstract void Process(double delta);

    private ProcedureKey _pKey;
    protected LogicKey _logicKey;
    protected Data _data;
    public HostLogic(Data data, Guid hostPlayerGuid)
    {
        _data = data;
        _pKey = new ProcedureKey(data);
        _logicKey = new LogicKey(this, data);
        HostPlayerGuid = hostPlayerGuid;
        CommandQueue = new ConcurrentQueue<Command>();
        Server = new HostServer(data.Entities, this);
    }
    
    public void HandleMessageFromServer(Message m)
    {
        if (m is Command c)
        {
            CommandQueue.Enqueue(c);
        }
        else if (m is ClientMessage cm)
        {
            if (cm.ClientGuid != HostPlayerGuid) throw new Exception();
            MessageForLocalClient.Invoke(cm);
        }
        else
        {
            throw new Exception($"message of type {m.GetType()} from server not supported");
        }
    }

    public void HandleMessageFromClient(Message m)
    {
        if (m is Command c)
        {
            CommandQueue.Enqueue(c);
        }
        else throw new Exception($"message of type {m.GetType()} from client not supported");
    }
    public void SendMessageToClient(ClientMessage m)
    {
        if(m.ClientGuid == HostPlayerGuid)
        {
            MessageForLocalClient.Invoke(m);
        }
        else
        {
            Server.SendMessageToClient(m, m.ClientGuid);
        }
    }
    
    public void HandleMessageFromLogic(Message m)
    {
        if (m is Procedure p)
        {
            p.Handle(_pKey);
        }
        else throw new Exception($"message type {m.GetType()} from logic not supported");
        Server.HandleOutgoingMessage(m);
    }
}