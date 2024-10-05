using Godot;

namespace GodotUtilities.Ui;

public class KeyboardInputMode
{
    private Dictionary<Key, Action<InputEventKey>> _baseActions = new();
    private Dictionary<Key, Action<InputEventKey>> _shiftActions = new();
    private Dictionary<Key, Action<InputEventKey>> _ctrlActions = new();
    private Dictionary<Key, Action<InputEventKey>> _altActions = new();


    protected void AddAction(Key key, Action<InputEventKey> action)
    {
        _baseActions.Add(key, action);
    }
    public void Handle(InputEventKey k)
    {
        var key = k.Keycode;
        if (k.IsCtrlPressed())
        {
            if (_ctrlActions.TryGetValue(key, out var action))
            {
                action(k);
            }
        }
        else if (k.IsShiftPressed())
        {
            if (_shiftActions.TryGetValue(key, out var action))
            {
                action(k);
            }
        }
        else if (k.IsAltPressed())
        {
            if (_altActions.TryGetValue(key, out var action))
            {
                action(k);
            }
        }
        else if (_baseActions.TryGetValue(key, out var action))
        {
            action(k);
        }
    }
}