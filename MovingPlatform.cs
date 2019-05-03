using Godot;
using System;

public class MovingPlatform : Path2D
{
    [Export] public int Platform_Speed;
    private PathFollow2D _pathFollow;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetPath();
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        _pathFollow.SetOffset(_pathFollow.GetOffset() + Platform_Speed * delta);
    }

    public void SetPath()
    {
        _pathFollow = GetNode<PathFollow2D>("PathFollow2D");
    }
}
