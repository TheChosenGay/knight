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
    }

    public override void Exit()
    {
        base.Exit();
    }


    public override void StatePhysicsProcess(double delta)
    {
        base.StatePhysicsProcess(delta);
        var dir = context.GetDirection();
        if (dir == Vector2.Zero)
        {
            var velocity = context.GetVelocity();
            velocity.Y += gravity * (float)delta;
            if (context.IsOnFloor())
            {
                velocity.Y = 0;
            }
            context.SetVelocity(velocity);
            return;
        }

        if (dir.Y < 0)
        {
            EmitSignal(SignalName.StateChange, "Jump");
        }
        else
        {
            if (dir.X != 0)
            {
                EmitSignal(SignalName.StateChange, "Run");
            }
        }

    }



}
