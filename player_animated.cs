using Godot;
using System;
using static Godot.GD;

public class player_animated : KinematicBody2D
{
    [Export] public int Gravity;
    [Export] public int JumpSpeed;
    [Export] public int RunSpeed;

    private Vector2 Velocity = new Vector2();
    private Vector2 _Snap = new Vector2();
    private bool Jumping;
    private AnimationPlayer _animation;
    private Sprite _sprite;

    public State CurrentState { get; set; }

    public enum State
    {
        IDLE,
        RUN,
        JUMP
    }

    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _sprite = GetNode<Sprite>("Sprite");
        SetCurrentState(State.IDLE);
    }

    private void GetInput()
    {
        // resets Velocity.x each frame to ensure a set movement
        Velocity.x = 0;
        bool keyJump = Input.IsActionJustPressed("ui_jump");
        bool keyRight = Input.IsActionPressed("ui_right");
        bool keyLeft = Input.IsActionPressed("ui_left");

        bool idle = !keyRight && !keyLeft;

        // resets animation to idle when stood on floor not moving
        if (idle && IsOnFloor())
        {
            _animation.Play("idle");
            SetCurrentState(State.IDLE);
        }

        if (keyJump && IsOnFloor())
        {
            SetCurrentState(State.JUMP);
            Velocity.y = JumpSpeed;
            _animation.Play("jump_up");
        }
        if (keyRight)
        {
            // SetCurrentState(State.RUN);
            _animation.Play("run");
            /* FlipH ensures sprite is facing correct direction if only one animation
            not needed if a left and a right animation */
            _sprite.FlipH = false;
            Velocity.x += RunSpeed;
        }
        if (keyLeft)
        {
            _animation.Play("run");
            _sprite.FlipH = true;
            Velocity.x -= RunSpeed;
        }
    }

    public void SetCurrentState(State state)
    {
        CurrentState = state;
        switch (state)
        {
            case State.JUMP:
                _animation.Play("jump_up");
                break;
            case State.IDLE:
                _animation.Play("idle");
                break;
            case State.RUN:
                _animation.Play("run");
                break;
            default:
                break;
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        // set gravity
        Velocity.y += Gravity * delta;

        // GetInput() could be included in this function rather than its own
        GetInput();
        
        // changing value of snap vector to (0, 0) allows player to jump
        if (CurrentState == State.JUMP && IsOnFloor())
        {
            _Snap = new Vector2(0, 0);
        }
        else
        {
            _Snap = new Vector2(0, 40);
        }

        // testing value of Velocity.y determines direction of jump
        if (Velocity.y < 0 && CurrentState == State.JUMP)
        {
            _animation.Play("jump_up");
        }
        else if (Velocity.y > 0 && CurrentState == State.JUMP)
        {
            _animation.Play("jump_down");
        }

        Velocity = MoveAndSlideWithSnap(Velocity, _Snap, Vector2.Up);

        // cancels out the JUMP state/animation once landed on floor after the move
        if (CurrentState == State.JUMP && IsOnFloor())
        {
            SetCurrentState(State.IDLE);
        }

        
    }
}
