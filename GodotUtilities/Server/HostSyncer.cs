using Godot;
using GodotUtilities.GameData;
using GodotUtilities.Serialization;

namespace GodotUtilities.Server;

public class HostSyncer : Syncer
{
    private Queue<byte[]> _peerQueue;
    public HostSyncer(PacketPeerStream packetStream, 
        Action<Message> handleIncoming, Serializer serializer) 
        : base(packetStream, handleIncoming, serializer)
    {
        _peerQueue = new Queue<byte[]>();
    }
    public void QueuePacket(byte[] packet)
    {
        _peerQueue.Enqueue(packet);
    }
    public void PushPackets()
    {
        var count = _peerQueue.Count;
        for (var i = 0; i < count; i++)
        {
            PushPacket(_peerQueue.Dequeue());
        }
    }
    public void Sync(IEnumerable<Entity> entities, Guid newPlayer)
    {
        foreach (var e in entities)
        {
            var u = new EntityCreationMessage(e);
            QueuePacket(_serializer.Serialize(u));
        }
        
        var done = new FinishedStateSyncUpdate(newPlayer);
        var bytes = _serializer.Serialize(done);
        QueuePacket(bytes);
        PushPackets();
    }
}