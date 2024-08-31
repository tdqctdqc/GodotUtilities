using Godot;

namespace GodotUtilities.Input;

public static class InputExt
{
    public static int GetNumKeyPressed()
    {
        if (Godot.Input.IsKeyPressed(Key.Key0)) return 0;
        if (Godot.Input.IsKeyPressed(Key.Key1)) return 1;
        if (Godot.Input.IsKeyPressed(Key.Key2)) return 2;
        if (Godot.Input.IsKeyPressed(Key.Key3)) return 3;
        if (Godot.Input.IsKeyPressed(Key.Key4)) return 4;
        if (Godot.Input.IsKeyPressed(Key.Key5)) return 5;
        if (Godot.Input.IsKeyPressed(Key.Key6)) return 6;
        if (Godot.Input.IsKeyPressed(Key.Key7)) return 7;
        if (Godot.Input.IsKeyPressed(Key.Key8)) return 8;
        if (Godot.Input.IsKeyPressed(Key.Key9)) return 9;
        return -1;
    }
}