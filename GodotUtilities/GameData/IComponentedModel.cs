using System.Collections.Generic;

namespace GodotUtilities.GameData;


public interface IComponentedModel
{
    ModelComponentHolder Components { get; }
}