using System;
using Godot;


namespace Knight.State.PlayerState;

public partial class PlayerJumpState : PlayerState
{
    private float jumpSpeed;
    private bool isCanJump;
    private bool isShorJumping;
    private float jumpDistance;
    private float jumpMoveSpeed; // 起跳后，左右移动的速度
    private float accJumpDelta; // jump后累计时长

    public override void _Ready()
    {
        base._Ready();
        jumpSpeed = 500;
        jumpDistance = 0;
        accJumpDelta = 0;
        jumpMoveSpeed = 300;
    }


    public override void Enter()
    {
        base.Enter();

    }



    public override void Exit()
    {
        base.Exit();
    }

    public override void StatePhysicsProcess(double delta)
    {
        base.StatePhysicsProcess(delta);
        var dir = context.GetDirection();
        var velocity = context.GetVelocity();

        if (isCanJump && dir.Y < 0)
        {
            velocity.Y = dir.Y * jumpSpeed;
            isCanJump = false;
        }
        var jumpDelta = isShorJumping ? accJumpDelta / (float)2.0 : accJumpDelta / (float)100.0;
        velocity.X = (dir.X != 0) ? dir.X * jumpMoveSpeed : Mathf.Lerp(0, velocity.X, Mathf.Pow(2, -jumpDelta));

        jumpDistance += -(float)(velocity.Y * delta);

        if (!context.IsOnFloor() && velocity.Y < 0 && jumpDistance < 120)
        {
            isShorJumping = !Input.IsActionPressed("jump");
        }

        var realGravity = isShorJumping ? gravity * 5 : gravity;
        velocity.Y += realGravity * (float)delta;
        velocity.Y = Mathf.Clamp(velocity.Y, -500, 500);

        Callable.From(() =>
        {
            isCanJump = true;
            isShorJumping = false;
            jumpDistance = 0;
            PlayAnimationAndAudio(velocity);
            JudgeState(dir);

        }).CallDeferred();
        context.SetVelocity(velocity);
    }

    private void JudgeState(Vector2 dir)
    {

        if (context.IsOnFloor())
            {
            if (dir == Vector2.Zero)
            {
                EmitSignal(SignalName.StateChange, "Idle");
            }
            else
            {
                EmitSignal(SignalName.StateChange, "Run");
            }
        }
    }

    public void PlayAnimationAndAudio(Vector2 velocity)
    {
        var animationName = velocity.Y > 0 ? "fall" : "jump";
        float shapeHeight = velocity.Y > 0 ? (float)120.0 : (float)134.0;
        context.SetCollisionShapeSize(new Vector2((float)40.0, shapeHeight));
        context.GetAnimationPlayer().SpeedScale = (float)0.3;
        context.GetAnimationPlayer().Play(animationName);
        if (animationName == "jump" && jumpDistance < 20 && velocity.Y < 0)
        {
            context.GetAudioStreamPlayer();
        }

    }

}
