
using System;
using Godot;
using GodotUtilities.Logic;

namespace GodotUtilities.GameClient;
using GodotUtilities.GameData;
using GodotUtilities.Server;
using MessagePack;

public class CallbackCommand : Command
{
    public Command Inner { get; private set; }
    public int CallbackId { get; private set; }

    public static CallbackCommand Redraw(Command inner, 
        Node n, Action redraw,
        GameClient c)
    {
        var id = c.Callbacks.AddCallback(() =>
        {
            if (GodotObject.IsInstanceValid(n))
            {
                redraw();
            }
        });
        return new CallbackCommand(c.PlayerGuid, inner, id);
    }
    public static CallbackCommand Construct(Command inner, 
        Action callback,
        GameClient c)
    {
        var id = c.Callbacks.AddCallback(callback);
        return new CallbackCommand(c.PlayerGuid, inner, id);
    }
    [SerializationConstructor] private CallbackCommand(
        Guid commandingPlayerGuid,
        Command inner,
        int callbackId)
    {
        Inner = inner;
        CallbackId = callbackId;
    }


    public override void Handle(LogicKey key)
    {
        Inner.Handle(key);
        key.SendMessageToClient(new DoClientCallbackMessage(CallbackId, CommandingPlayerGuid));
    }
}