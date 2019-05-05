using System;
using Godot;

namespace MovingPlatforms
{
    public class player_animated : KinematicBody2D
    {
        [Export] public int Gravity;
        [Export] public int JumpSpeed;
        [Export] public int RunSpeed;
        [Export] public int Acceleration;

        private Vector2 _velocity;
        private Vector2 _snap;
        private bool _friction;
        private AnimationPlayer _animation;
        private Sprite _sprite;

        private State CurrentState { get; set; }

        private string Anim { get; set; }

        public enum State
        {
            IDLE,
            RUN,
            JUMP
        }

        public override void _Ready()
        {
            // set values of _animation and _sprite once when scene loaded
            _animation = GetNode<AnimationPlayer>("AnimationPlayer");
            _sprite = GetNode<Sprite>("Sprite");

            /* removing line below makes player jump animation run if starts in the air
            else player will fall running the idle animation */
            // SetCurrentState(State.IDLE);
        }

        private void GetInput()
        {
            /* resets _velocity.x each frame to ensure a set movement if using
            _velocity.x += RunSpeed rather than Math.Min(...)
            */
            // Velocity.x = 0;
            bool keyJump = Input.IsActionJustPressed("ui_jump");
            bool keyRight = Input.IsActionPressed("ui_right");
            bool keyLeft = Input.IsActionPressed("ui_left");

            bool idle = !keyRight && !keyLeft;
        
            if (idle && IsOnFloor()) // resets animation to idle when stood on floor not moving
            {
                SetCurrentState(State.IDLE);
                /* resets _velocity.x when not moving
                if left at the top of the method, _velocity.x never gets above Acceleration as it gets reset
                */
                // _velocity.x = 0;

                // Lerp slows down over a period of time. In this instance essentially decelerating to 0
                _velocity.x = Mathf.Lerp(_velocity.x, 0, 0.2f);
            }
            else if (keyRight)
            {
                SetCurrentState(State.RUN);
                /* FlipH ensures sprite is facing correct direction if only one animation
                not needed if a left and a right animation */
                _sprite.FlipH = false;
                _velocity.x = Math.Min(_velocity.x + Acceleration, RunSpeed);
            }
            else if (keyLeft)
            {
                SetCurrentState(State.RUN);
                _sprite.FlipH = true;
                _velocity.x = Math.Max(_velocity.x - Acceleration, -RunSpeed);
            }

            // controls jumping
            if (IsOnFloor())
            {
                if (keyJump)
                {
                    SetCurrentState(State.JUMP);
                    _velocity.y = JumpSpeed;
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
                    if (_velocity.y < 0)
                    {
                        Anim = "jump_up";
                    }
                    else if (_velocity.y > 0)
                    {
                        Anim = "jump_down";
                    }
                    break;
                case State.IDLE:
                    Anim = "idle";
                    break;
                case State.RUN:
                    Anim = "run";
                    break;
            }
            _animation.Play(Anim);
        }
    
        public override void _PhysicsProcess(float delta)
        {
            // reset player if falls off a platform
            if (_velocity.y > 1000)
            {
                Position = new Vector2(140, 290);
            }

            _velocity.y += Gravity * delta;

            // GetInput() functionality could be included in this function rather than its own
            GetInput();
        
            // changing value of snap vector to (0, 0) allows player to jump
            if (CurrentState == State.JUMP && IsOnFloor())
            {
                _snap = new Vector2(0, 0);
            }
            else
            {
                _snap = new Vector2(0, 40);
            }

            // testing value of _velocity.y determines direction of jump
            if (_velocity.y != 0 && CurrentState == State.JUMP)
            {
                SetCurrentState(State.JUMP);
            }

            _velocity = MoveAndSlideWithSnap(_velocity, _snap, Vector2.Up);

            // cancels out the JUMP state/animation once landed on floor after the move
            if (CurrentState == State.JUMP && IsOnFloor())
            {
                SetCurrentState(State.IDLE);
            }
        }
    }
}
