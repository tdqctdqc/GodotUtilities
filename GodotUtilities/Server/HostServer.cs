using Godot;
using GodotUtilities.Serialization;
using GodotUtilities.GameData;
namespace GodotUtilities.Server;

public abstract partial class HostServer : Node
{
    private Entities _entities;
    private List<HostSyncer> _peers;
    private Dictionary<Guid, HostSyncer> _peersByGuid;
    private Serializer _serializer;
    private TcpServer _tcp;
    private int _port = 3306;

    public HostServer(Entities entities)
    {
        _entities = entities;
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
    public void QueueMessage(Message m)
    {
        var bytes = _serializer.Serialize(m);
        for (var i = 0; i < _peers.Count; i++)
        {
            _peers[i].QueuePacket(bytes);
        }
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
    protected abstract void HandleIncomingMessage(Message m);
}