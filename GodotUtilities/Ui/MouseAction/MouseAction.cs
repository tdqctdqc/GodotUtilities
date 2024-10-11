using Godot;

namespace GodotUtilities.Ui;

public abstract class MouseAction : IMouseAction
{
    protected abstract void MouseDown(InputEventMouse m);
    protected abstract void MouseHeld(InputEventMouse m);
    protected abstract void MouseUp(InputEventMouse m);
    public abstract void Clear();
    private MouseButtonMask _button;
    private bool _mouseDown;
    private MouseAuxButton _aux;
    private Action _defaultReleaseAction;
    private Action _ctrlReleaseAction;
    private Action _shiftReleaseAction;

    protected MouseAction(MouseButtonMask button)
    {
        _button = button;
        _mouseDown = false;
    }
    public void Process(InputEventMouse m)
    {
        var pressed = MaskFit(m);
        if (_mouseDown && pressed)
        {
            MouseHeld(m);
        }
        else if(_mouseDown && pressed == false)
        {
            _mouseDown = false;
            DoAction();
            MouseUp(m);
        }
        else if (_mouseDown == false && pressed)
        {
            SetAuxButton(m);
            _mouseDown = true;
            MouseDown(m);
        }
        ProcessSub(m);
    }

    protected virtual void ProcessSub(InputEventMouse m)
    {
        
    }

    protected void DoAction()
    {
        if (_aux == MouseAuxButton.Ctrl)
        {
            _ctrlReleaseAction?.Invoke();
        }
        else if (_aux == MouseAuxButton.Shift)
        {
            _shiftReleaseAction?.Invoke();
        }
        else if(_aux == MouseAuxButton.Default)
        {
            _defaultReleaseAction?.Invoke();
        }
    }

    private void SetAuxButton(InputEventMouse m)
    {
        if (m.CtrlPressed)
        {
            _aux = MouseAuxButton.Ctrl;
        }
        else if (m.ShiftPressed)
        {
            _aux = MouseAuxButton.Shift;
        }
        else
        {
            _aux = MouseAuxButton.Default;
        }
    }

    public void AddDefaultAction(Action action)
    {
        _defaultReleaseAction += action;
    }
    public void AddCtrlAction(Action action)
    {
        _ctrlReleaseAction += action;
    }
    public void AddShiftAction(Action action)
    {
        _shiftReleaseAction += action;
    }
    public bool MaskFit(InputEventMouse m)
    {
        return (m.ButtonMask & _button) != 0;
    }
}