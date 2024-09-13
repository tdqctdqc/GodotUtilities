using GodotUtilities.GameData;

namespace GodotUtilities.Logic;

public class ProcedureKey
{
    public Data Data { get; private set; }

    public ProcedureKey(Data data)
    {
        Data = data;
    }
}