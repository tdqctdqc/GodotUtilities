
using GodotUtilities.GameClient;
using GodotUtilities.GameData;
using GodotUtilities.Logic;
using GodotUtilities.Server;

public class DoClientCallbackMessage : ClientMessage
{
    public int CallbackId { get; private set; }

    public DoClientCallbackMessage(int callbackId, Guid clientGuid) : base(clientGuid)
    {
        CallbackId = callbackId;
    }

    public override void Handle(GameClient c)
    {
        c.Callbacks.CallBack(CallbackId);
    }

}