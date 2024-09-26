using System.Collections.Generic;

namespace GodotUtilities.GameData;

public interface IComponentedEntity 
{
    int Id { get; }
    EntityComponentHolder Components { get; }
}