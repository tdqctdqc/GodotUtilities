using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace GodotUtilities.Server;

public class FinishedStateSyncUpdate : Message
{
    public Guid PlayerGuid { get; private set; }
    public FinishedStateSyncUpdate(Guid playerGuid) : base()
    {
        PlayerGuid = playerGuid;
    }

    public override void Handle()
    {
        // key.Data.ClientPlayerData.SetLocalPlayerGuid(PlayerGuid);
    }
}