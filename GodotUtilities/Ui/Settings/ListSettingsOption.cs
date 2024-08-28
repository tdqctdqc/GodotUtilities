using Godot;

namespace GodotUtilities.Ui;

public class ListSettingsOption<T> : SettingsOption<T>
{
    public List<T> Options { get; private set; }
    private Dictionary<T, string> _names;
    public ListSettingsOption(string name, List<T> options, 
        List<string> names) 
        : base(name, options.FirstOrDefault())
    {
        Options = options;
        _names = new Dictionary<T, string>();
        for (var i = 0; i < options.Count; i++)
        {
            _names.Add(options[i], names[i]);
        }
    }

    public TOption Choose<TOption>() where TOption : T
    {
        var first = Options.OfType<TOption>().First();
        Set(first);
        return first;
    }

    public void Choose(T t)
    {
        if (Options.Contains(t) == false) throw new Exception();
        Set(t);
    }
    public override Control GetControlInterface()
    {
        var token = new ItemListToken<T>(
            Options,
            t => _names[t],
            false
        );
        token.JustSelected += () =>
        {
            if (token.Selected.Count != 1) return;
            Set(token.Selected.First());
        };
        var list = token.ItemList;
        SettingChanged.SubscribeForNode(t => list.Select(Options.IndexOf(t.newVal)),
            list);
        list.Select(Options.IndexOf(Value));
        list.FocusMode = Control.FocusModeEnum.None;
        list.ExpandFill();
        list.CustomMinimumSize = new Vector2(100f, 100f);
        return list;
    }
    
    public Control GetControlInterfaceIcon(
        Func<T, Texture2D> getTexture,
        int iconHeight)
    {
        var token = new ItemListToken<T>(
            Options,
            m => _names[m],
            getTexture,
            iconHeight,
            false);
        var list = token.ItemList;
        token.JustSelected += () =>
        {
            if (token.Selected.Count != 1) return;
            Set(token.Selected.First());
        };
        SettingChanged.SubscribeForNode(t => list.Select(Options.IndexOf(t.newVal)),
            list);
        list.Select(Options.IndexOf(Value));
        list.FocusMode = Control.FocusModeEnum.None;
        return list;
    }
}