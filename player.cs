using Godot;
using System;

public class player : KinematicBody2D
{
    [Export] public int Gravity;
    [Export] public int JumpSpeed;
    [Export] public int RunSpeed;

    private Vector2 Velocity = new Vector2();
    Vector2 snap = new Vector2();
    private bool Jumping;

    public override void _Ready()
    {

    }

    private void GetInput()
    {
        Velocity.x = 0;
        bool keyJump = Input.IsActionJustPressed("ui_jump");
        bool keyRight = Input.IsActionPressed("ui_right");
        bool keyLeft = Input.IsActionPressed("ui_left");

        if (keyJump && IsOnFloor())
        {
            Jumping = true;
            // larger -ve JumpSpeed number = higher jump
            Velocity.y = JumpSpeed;
        }
        if (keyRight)
        {
            Velocity.x += RunSpeed;
        }
        /* if (keyLeft && IsOnFloor()) will make the player jump straight up */
        if (keyLeft)
        {
            Velocity.x -= RunSpeed;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput();

        Velocity.y += Gravity * delta;
        if (Jumping && IsOnFloor())
        {
            Jumping = false;
            snap = new Vector2(0, 0);
        }
        else
        {
            Jumping = false;
            snap = new Vector2(0, 40);
        }
        
        // Move and slide
        // Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));

        /* Move and slide with snap
        set snap to (0, 0) in order to jump
         */
        Velocity = MoveAndSlideWithSnap(Velocity, snap, Vector2.Up);
    }
}
