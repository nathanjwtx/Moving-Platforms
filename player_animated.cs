using Godot;
using System;

public class player_animated : KinematicBody2D
{
    [Export] public int Gravity;
    [Export] public int JumpSpeed;
    [Export] public int RunSpeed;

    private Vector2 Velocity = new Vector2();
    private Vector2 _Snap = new Vector2();
    private bool Jumping;
    private AnimationPlayer _animation;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    private void GetInput()
    {
        // Velocity.x = 0;
        bool keyJump = Input.IsActionJustPressed("ui_jump");
        bool keyRight = Input.IsActionPressed("ui_right");
        bool keyLeft = Input.IsActionPressed("ui_left");

        bool idle = !keyRight && !keyLeft;

        if (keyJump && IsOnFloor())
        {
            GD.Print("jump");
            Jumping = true;
            Velocity.y = JumpSpeed;
            _animation.Play("jump_up");
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput();

        Velocity.y += Gravity * delta;
        if (Jumping && IsOnFloor())
        {
            Jumping = false;
            _Snap = new Vector2(0, 0);
        }
        else
        {
            Jumping = false;
            _Snap = new Vector2(0, 40);
        }

        Velocity = MoveAndSlideWithSnap(Velocity, _Snap, Vector2.Up);
    }
}
