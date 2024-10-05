using Godot;
using GodotUtilities.GameClient;
using GodotUtilities.Logic;

namespace GodotUtilities.GameData;

public interface IEntityComponent : IComponent
{
    void TurnTick(ProcedureKey key);
    void Added(EntityComponentHolder holder, Data data);
    void Removed(EntityComponentHolder holder, Data data);
}