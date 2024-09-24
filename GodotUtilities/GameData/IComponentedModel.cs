using System.Collections.Generic;

namespace HexGeneral.Game.Components;

public interface IComponentedModel
{
    List<IModelComponent> Components { get; }
}