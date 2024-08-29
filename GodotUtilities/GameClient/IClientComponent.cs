using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace GodotUtilities.GameClient;

public interface IClientComponent
{
    Node Node { get; }
    void Connect(GameClient client);
    Action Disconnect { get; set; }
    void Process(float delta);
}
