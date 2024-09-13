using GodotUtilities.Logic;

namespace GodotUtilities.Server;

public abstract class Procedure : Message
{
    public abstract void Handle(ProcedureKey key);
}