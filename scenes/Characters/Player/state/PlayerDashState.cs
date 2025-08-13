using Godot;
using System;
using Knight.State.PlayerState;

public partial class PlayerDashState : PlayerState
{
    private float dashSpeed = 1000f; // 冲刺速度
    private bool dashRight = true;
    private Vector2 oldCollisionShapeSize;
    private Vector2 oldCollisionShapePosition;
    // 生命周期函数

    public override void StatePhysicsProcess(double delta)
    {
        base.StatePhysicsProcess(delta);
        var velocity = context.GetVelocity();
        var dashDir = dashRight ? 1 : -1; 
        velocity.Y += context.GetPlayerGravity() * (float)delta; // 有可能冲出地面的
        context.SetVelocity(new Vector2(dashDir * dashSpeed, velocity.Y));

        Callable.From(() =>
        {
            JudgeState();
        }).CallDeferred();
    }

    public override void StateProcess(double delta)
    {
        base.StateProcess(delta);
    }


    public override void Enter()
    {
        context.GetAnimationPlayer().SpeedScale = 0.3f;
        base.Enter();
        dashRight = context.IsFaceRight;
        oldCollisionShapePosition = context.GetCollisionShape().Position;
        oldCollisionShapeSize = ((RectangleShape2D)context.GetCollisionShape().Shape).Size;
        context.SetCollisionShapeSize(new Vector2(63f, 121.5f), new Vector2(32.5f, -71.5f));
    }


    public override void Exit()
    {
        base.Exit();
        context.IsDash = false;
        context.SetVelocity(Vector2.Zero);
        context.SetCollisionShapeSize(oldCollisionShapeSize, oldCollisionShapePosition);
    }


    private void JudgeState()
    {
        if (!context.GetAnimationPlayer().IsPlaying())
        {
            EmitSignal(SignalName.StateChange, "Idle");
        }
    }

}
