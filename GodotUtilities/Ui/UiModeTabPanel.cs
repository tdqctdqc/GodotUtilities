using Godot;
using GodotUtilities.GameClient;

namespace GodotUtilities.Ui;

public partial class UiModeTabPanel : TabContainer
{
    protected UiMode _mode;
    private MouseMode _noTabMode;
    private List<Control> _tabs;
    private List<MouseMode> _tabModes;
    private List<Func<bool>> _tabEnable;
    private List<Action> _tabDraw;

    
    public UiModeTabPanel(UiMode mode, MouseMode noTabMode = null)
    {
        _mode = mode;
        _noTabMode = noTabMode;
        _tabs = new List<Control>();
        _tabEnable = new List<Func<bool>>();
        _tabDraw = new List<Action>();
        _tabModes = new List<MouseMode>();
        _mode.MouseMode.SettingChanged.SubscribeForNode(v =>
        {
            DrawTabs();
        }, this);
        TreeEntered += DrawTabs;
        VisibilityChanged += () =>
        {
            if (Visible) DrawTabs();
        };
        TabSelected += v =>
        {
            var index = _tabs.IndexOf((Control)GetChildren()[(int)v]);
            if (_mode.MouseMode.Value != _tabModes[index])
            {
                _mode.MouseMode.Set(_tabModes[index]);
            }
        };
    }
    
    protected void AddTab(Control tab, MouseMode mode, Func<bool> enable, 
        Action draw)
    {
        AddChild(tab);
        _tabs.Add(tab);
        _tabModes.Add(mode);
        _tabEnable.Add(enable);
        _tabDraw.Add(draw);
    }
    protected void DrawTabs()
    {
        if (_mode.MouseMode.Value == _noTabMode
            || _mode.MouseMode.Value is null)
        {
            for (var i = 0; i < _tabs.Count; i++)
            {
                SetTabDisabled(i, true);
                _tabs[i].ClearChildren();
            }

            return;
        }

        for (var i = 0; i < _tabs.Count; i++)
        {
            var mode = _tabModes[i];
            var index = _tabs[i].GetIndex();
            if (mode == _mode.MouseMode.Value)
            {
                CurrentTab = index;
                _tabDraw[i]();
            }

            if (_tabEnable[i]() == false)
            {
                SetTabDisabled(index, true);
                _tabs[i].ClearChildren();
            }
            else
            {
                SetTabDisabled(index, false);
            }
        }
    }
}