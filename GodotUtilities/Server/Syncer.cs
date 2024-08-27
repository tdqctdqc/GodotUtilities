using System.Diagnostics;
using Godot;
using GodotUtilities.Serialization;

namespace GodotUtilities.Server;

public class Syncer
{
    protected Serializer _serializer;
    protected PacketPeerStream _packetStream;
    protected PacketProtocol _protocol;
    protected bool _listening;
    private Stopwatch _sw;
    private Action<Message> _handleIncoming;


    protected Syncer(PacketPeerStream packetStream,
        Action<Message> handleIncoming, Serializer serializer)
    {
        _serializer = serializer;
        _handleIncoming = handleIncoming;
        _sw = new Stopwatch();
        _packetStream = packetStream;
        _protocol = new PacketProtocol(0);
        _protocol.MessageArrived += b =>
        {
            var m = _serializer.Deserialize<Message>(b);
            _handleIncoming(m);
        };
        Task.Run(Listen);
    }
    
    private int _packetsReceived = 0;
    private async void Listen()
    {
        _listening = true;
        while (true)
        {
            if (_listening)
            {
                var count = _packetStream.GetAvailablePacketCount();
                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        _protocol.DataReceived(_packetStream.GetPacket());
                        _packetsReceived++;
                    }
                }
            }
        }
    }
    protected void PushPacket(byte[] bytes)
    {
        if (bytes.Length > short.MaxValue - 1) throw new Exception($"packet too big at {bytes.Length} bytes");
        var lengthBytes = BitConverter.GetBytes(Convert.ToInt16(bytes.Length));
        
        var e1 = _packetStream.PutPacket(lengthBytes);
        var e2 = _packetStream.PutPacket(bytes);
    }
}