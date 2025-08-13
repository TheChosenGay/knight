using System;
using Godot;


namespace Knight.State.PlayerState;

public partial class PlayerJumpState : PlayerState
{
    private float jumpSpeed;
    private bool isCanSecondJump;
    private bool isShorJumping;
    private float jumpDistance;
    private float jumpMoveSpeed; // 起跳后，左右移动的速度
    private float accJumpDelta; // jump后累计时长

    public override void _Ready()
    {
        base._Ready();
        jumpSpeed = 600;
        jumpMoveSpeed = 300;
        jumpDistance = 0;
        accJumpDelta = 0;
    }


    public override void Enter()
    {
        base.Enter();

        isCanSecondJump = true;
        jumpDistance = 0;
        context.SetVelocity(new Vector2(0, -jumpSpeed));
    }



    public override void Exit()
    {
        base.Exit();
        isCanSecondJump = true;
        isShorJumping = false;
        jumpDistance = 0;
    }

    public override void StatePhysicsProcess(double delta)
    {
        base.StatePhysicsProcess(delta);
        var dir = context.GetDirection();
        var velocity = context.GetVelocity();

        var jumpDelta = isShorJumping ? accJumpDelta / (float)2.0 : accJumpDelta / (float)100.0;
        velocity.X = (dir.X != 0) ? dir.X * jumpMoveSpeed : Mathf.Lerp(0, velocity.X, Mathf.Pow(2, -jumpDelta));

        jumpDistance += -(float)(velocity.Y * delta);

        if (!context.IsOnFloor() && velocity.Y < 0 && jumpDistance < 100)
        { // 刚跳起阶段，判断是否为短跳
            isShorJumping = !Input.IsActionPressed("jump");
        }


        var realGravity = isShorJumping ? gravity * 5 : gravity;
        velocity.Y += realGravity * (float)delta;

        if (isCanSecondJump && !isShorJumping && !context.IsOnFloor() && jumpDistance > 50 && Input.IsActionJustPressed("jump")) {
            // 二段跳
            isCanSecondJump = false;
            velocity.Y += -jumpSpeed;
        }
        velocity.Y = Mathf.Clamp(velocity.Y, -500, 500);


        context.SetVelocity(velocity);

        Callable.From(() =>
        {
            PlayAnimationAndAudio(velocity);
            JudgeState(velocity);
        }).CallDeferred();
    }

    private void JudgeState(Vector2 velocity)
    {
        if (context.IsOnFloor())
        {
            if (velocity.X == 0)
            {
                EmitSignal(SignalName.StateChange, "Idle");
            }
            else if (velocity.X != 0)
            {
                EmitSignal(SignalName.StateChange, "Run");
            }
        }
    }

    public void PlayAnimationAndAudio(Vector2 velocity)
    {
        var animationName = velocity.Y > 0 ? "fall" : "jump";
        float shapeHeight = velocity.Y > 0 ? (float)120.0 : (float)134.0;
        context.SetCollisionShapeSize(new Vector2((float)40.0, shapeHeight), null);
        context.GetAnimationPlayer().SpeedScale = (float)0.3;
        context.GetAnimationPlayer().FlipH = !context.IsFaceRight;
        context.GetAnimationPlayer().Play(animationName);
        if (animationName == "jump" && jumpDistance < 20 && velocity.Y < 0)
        {
            context.GetAudioStreamPlayer();
        }

    }

}
