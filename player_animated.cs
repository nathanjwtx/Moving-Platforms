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

    private string _anim { get; set; }

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
            SetCurrentState(State.IDLE);
        }
        else if (keyRight)
        {
            SetCurrentState(State.RUN);
            /* FlipH ensures sprite is facing correct direction if only one animation
            not needed if a left and a right animation */
            _sprite.FlipH = false;
            Velocity.x += RunSpeed;
        }
        else if (keyLeft)
        {
                SetCurrentState(State.RUN);
                _sprite.FlipH = true;
                Velocity.x -= RunSpeed;

        }

        if (IsOnFloor())
        {
            if (keyJump)
            {
                SetCurrentState(State.JUMP);
                Velocity.y = JumpSpeed;
                Velocity.x -= RunSpeed;
            }
        }
        else
        {
            SetCurrentState(State.JUMP);
        }
    }

    public void SetCurrentState(State state)
    {
        CurrentState = state;
        switch (state)
        {
            case State.JUMP:
                if (Velocity.y < 0)
                {
                    _anim = "jump_up";
                }
                else if (Velocity.y > 0)
                {
                    _anim = "jump_down";
                }
                // _animation.Play("jump_up");
                break;
            case State.IDLE:
                _anim = "idle";
                // _animation.Play("idle");
                break;
            case State.RUN:
                _anim = "run";
                // _animation.Play("run");
                break;
            default:
                break;
        }
        _animation.Play(_anim);
    }
    public override void _PhysicsProcess(float delta)
    {

        // Print(CurrentState);
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
        if (Velocity.y != 0 && CurrentState == State.JUMP)
        {
            SetCurrentState(State.JUMP);
        }

        Velocity = MoveAndSlideWithSnap(Velocity, _Snap, Vector2.Up);

        // cancels out the JUMP state/animation once landed on floor after the move
        if (CurrentState == State.JUMP && IsOnFloor())
        {
            SetCurrentState(State.IDLE);
        }

        
    }
}
