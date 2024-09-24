using Godot;
using GodotUtilities.GameClient;
using GodotUtilities.Logic;

namespace HexGeneral.Game.Components;

public interface IEntityComponent
{
    Control GetDisplay(GameClient client);
    void TurnTick(ProcedureKey key);
    void Added(ProcedureKey key);
    void Removed(ProcedureKey key);
}