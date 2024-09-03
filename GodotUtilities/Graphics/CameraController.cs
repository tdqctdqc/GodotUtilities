using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.DataStructures.ShapingFunctions;
using GodotUtilities.GameClient;


namespace GodotUtilities.Graphics;

public partial class CameraController : Camera2D, IClientComponent
{
    private float _zoomLevel;
    private float _maxZoom = 100f, _minZoom = 1f;
    private float _zoomIncr = .1f;
    private float _scrollSpeed = 500f;
    public Node Node => this;

    public CameraController()
    {
        _zoomLevel = Zoom.X;
    }

    
    private void UpdateZoom(bool zoomIn)
    {
        var zoomDelta = _zoomIncr * _zoomLevel * (zoomIn ? 1f : -1f);
        _zoomLevel += zoomDelta;
        _zoomLevel = Mathf.Clamp(_zoomLevel, _minZoom, _maxZoom);
        Zoom = Vector2.One * _zoomLevel;
    }

    public void Connect(GameClient.GameClient client)
    {
        client.AddChild(this);
        MakeCurrent();
    }

    public Action Disconnect { get; set; }
    public void Process(float delta)
    {
        var mult = 1f;
        if (Godot.Input.IsKeyPressed( Godot.Key.Shift)) mult = 3f;
        if(Godot.Input.IsKeyPressed(Godot.Key.W))
        {
            Position += Vector2.Up * delta / Zoom * _scrollSpeed * mult;
        }
        if(Godot.Input.IsKeyPressed(Godot.Key.S))
        {
            Position += Vector2.Down * delta / Zoom * _scrollSpeed * mult;
        }
        if(Godot.Input.IsKeyPressed(Godot.Key.A))
        {
            Position += Vector2.Left * delta / Zoom * _scrollSpeed * mult;
        }
        if(Godot.Input.IsKeyPressed(Godot.Key.D))
        {
            Position += Vector2.Right * delta / Zoom * _scrollSpeed * mult;
        }
        
        if(Godot.Input.IsKeyPressed(Godot.Key.Z))
        {
            UpdateZoom(true);
        }
        if(Godot.Input.IsKeyPressed(Godot.Key.X))
        {
            UpdateZoom(false);
        }
    }
}
