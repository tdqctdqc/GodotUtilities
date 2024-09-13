using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.GameData;
using GodotUtilities.Logic;

namespace GodotUtilities.Server;

public class FinishedSyncProcedure : Procedure
{
    public Guid PlayerGuid { get; private set; }
    public FinishedSyncProcedure(Guid playerGuid) : base()
    {
        PlayerGuid = playerGuid;
    }

    public override void Handle(ProcedureKey key)
    {
    }
}