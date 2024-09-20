using GodotUtilities.Logic;
using GodotUtilities.Server;

namespace GodotUtilities.GameClient;

public class DoProcedureCommand(Procedure procedure) : Command
{
    public Procedure Procedure { get; private set; } = procedure;

    public override void Handle(LogicKey key)
    {
        key.SendMessage(Procedure);
    }
}