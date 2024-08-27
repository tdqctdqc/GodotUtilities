
using System;
using Godot;
namespace GodotUtilities.GameClient;
using GodotUtilities.GameData;
using GodotUtilities.Server;
using MessagePack;

public class CallbackCommand : Message
{
    public Message Inner { get; private set; }
    public int CallbackId { get; private set; }

    public static CallbackCommand Construct(Message inner, 
        Action callback,
        GameClient c)
    {
        var id = c.Callbacks.AddCallback(callback);
        return new CallbackCommand(c.GetPlayerGuid(), inner, id);
    }
    [SerializationConstructor] private CallbackCommand(
        Guid commandingPlayerGuid,
        Message inner,
        int callbackId)
    {
        Inner = inner;
        CallbackId = callbackId;
    }

    public override void Handle()
    {
        Inner.Handle();
    }
}