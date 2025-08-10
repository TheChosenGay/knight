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

        Callable.From(() =>
        {

            if (dir.Y < 0 || !context.IsOnFloor())
            {
                context.SetVelocity(new Vector2(0, 16));
                EmitSignal(SignalName.StateChange, "Jump");
            }
            else
            {
                if (dir.X != 0)
                {
                    EmitSignal(SignalName.StateChange, "Run");
                }
            }
        }).CallDeferred();

    }



}
