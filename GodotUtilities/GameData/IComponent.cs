using Godot;

namespace GodotUtilities.GameData;

public interface IComponent
{
    Control GetDisplay(GameClient.GameClient client);
}