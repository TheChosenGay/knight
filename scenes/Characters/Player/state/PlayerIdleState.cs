using Godot;
using System;
using System.Net;

namespace Knight.State.PlayerState;


public partial class PlayerIdleState : PlayerState
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void Enter()
    {
        base.Enter();
        // 只要进入idle状态，横向速度必然是0
        context.SetVelocity(new Vector2(0, 0));
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
        velocity.Y += context.GetPlayerGravity() * (float)delta; // 有可能冲出地面的
        context.SetVelocity(velocity);

        Callable.From(() =>
        {

            if (dir.Y < 0 || !context.IsOnFloor())
            {
                EmitSignal(SignalName.StateChange, "Jump");
            }
            else
            {
                if (context.IsDash)
                {
                    EmitSignal(SignalName.StateChange, "Dash");
                }
                else if (dir.X != 0)
                {
                    EmitSignal(SignalName.StateChange, "Run");
                }
            }
        }).CallDeferred();

    }



}
