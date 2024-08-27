using GodotUtilities.DataStructures;

namespace GodotUtilities.GameData;

public class SaveFile
{
    public Entities Entities { get; private set; }
    public IdDispenser IdDispenser { get; private set; }

    public SaveFile(Entities entities, IdDispenser idDispenser)
    {
        Entities = entities;
        IdDispenser = idDispenser;
    }
}