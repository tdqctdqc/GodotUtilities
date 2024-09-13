using Godot;
using GodotUtilities.Serialization;
using GodotUtilities.GameData;
using GodotUtilities.Logic;

namespace GodotUtilities.Server;

public  partial class HostServer : Node
{
    private ILogic _logic;
    private Entities _entities;
    private List<HostSyncer> _peers;
    private Dictionary<Guid, HostSyncer> _peersByGuid;
    private Serializer _serializer;
    private TcpServer _tcp;
    private int _port = 3306;

    public HostServer(Entities entities, ILogic logic)
    {
        _entities = entities;
        _logic = logic;
        _peers = new List<HostSyncer>();
        _peersByGuid = new Dictionary<Guid, HostSyncer>();
        _tcp = new TcpServer();
        _tcp.Listen((ushort)_port);
        _serializer = new Serializer();
    }
    public override void _Process(double delta)
    {
        if (_tcp.IsConnectionAvailable())
        {
            GD.Print("connection available");
            var peer = _tcp.TakeConnection();
            var newPlayer = new Guid();
            HandleNewPeer(newPlayer, peer);
        }
    }

    private void HandleNewPeer(Guid newPlayer,
        StreamPeerTcp peer)
    {
        var packet = new PacketPeerStream();
        packet.StreamPeer = peer;
        var syncer = new HostSyncer(packet, 
            HandleIncomingMessage, _serializer);
        GD.Print("started syncing");
        syncer.Sync(_entities.EntitiesById.Values, newPlayer);
        GD.Print("Done syncing");
        _peers.Add(syncer);
        _peersByGuid.Add(newPlayer, syncer);
    }
    public void SendMessageToClient(Message m, Guid clientGuid)
    {
        var bytes = _serializer.Serialize(m);
        _peersByGuid[clientGuid].QueuePacket(bytes);
    }
    public void HandleOutgoingMessage(Message m)
    {
        var bytes = _serializer.Serialize(m);
        for (var j = 0; j < _peers.Count; j++)
        {
            _peers[j].QueuePacket(bytes);
        }
    }
    public void PushPackets()
    {
        _peers.ForEach(p => p.PushPackets());
    }

    protected void HandleIncomingMessage(Message m)
    {
        _logic.HandleMessageFromServer(m);
    }
}