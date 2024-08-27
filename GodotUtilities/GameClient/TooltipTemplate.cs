using System;
using System.Collections.Generic;
using System.Linq;
using Godot;


public abstract class TooltipTemplate<T>
{
    protected abstract List<Func<T, Control>> _fastGetters { get; }
    protected abstract List<Func<T, Control>> _slowGetters { get; }
    protected TooltipTemplate()
    {
    }
    public List<Control> GetFastEntries(T t)
    {
        return _fastGetters
            .Select(f => f(t))
            .Where(c => c is not null)
            .ToList();
    }

    public List<Control> GetSlowEntries(T t)
    {
        return _slowGetters.Select(f => f(t))
            .Where(c => c is not null)
            .ToList();
    }

    public VBoxContainer GetFastContainer(T t)
    {
        var fast = GetFastEntries(t);
        if (fast.Count == 0) return null;
        var vbox = new VBoxContainer();
        for (var i = 0; i < fast.Count; i++)
        {
            vbox.AddChild(fast[i]);
        }
        return vbox;
    }
    public VBoxContainer GetSlowContainer(T t)
    {
        var slow = GetSlowEntries(t);
        if (slow.Count == 0) return null;
        var vbox = new VBoxContainer();

        for (var i = 0; i < slow.Count; i++)
        {
            vbox.AddChild(slow[i]);
        }
        return vbox;
    } 
}
