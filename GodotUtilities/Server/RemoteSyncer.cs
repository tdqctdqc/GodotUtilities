using Godot;
using GodotUtilities.Serialization;

namespace GodotUtilities.Server;

public class RemoteSyncer : Syncer
{
    public RemoteSyncer(PacketPeerStream packetStream, Action<Message> handleIncoming, Serializer serializer) 
        : base(packetStream, handleIncoming, serializer)
    {
    }

    public void Send(Message t)
    {
        var bytes = _serializer.Serialize(t);
        PushPacket(bytes);
    }
}