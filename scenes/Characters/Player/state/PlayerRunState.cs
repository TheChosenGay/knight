using Godot;

using Knight.State.PlayerState;

public partial class PlayerRunState : PlayerState
{


    private float moveSpeed;
    private float accHorDelta;

    public override void _Ready()
    {
        base._Ready();
        moveSpeed = 400;
        accHorDelta = 0;
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

        accHorDelta += (float)delta;

        velocity.X = (dir.X != 0) ? dir.X * moveSpeed : Mathf.Lerp(0, velocity.X, Mathf.Pow(2, -accHorDelta));
        velocity.Y += gravity * (float)delta;

        context.SetVelocity(velocity);

        PlayAnimationAndAudio(velocity);
        JudgeState(dir);
    }

    public override void StateProcess(double delta)
    {
        base.StateProcess(delta);
    }


    private void PlayAnimationAndAudio(Vector2 velocity)
    {
        AnimatedSprite2D animationPlayer = context.GetAnimationPlayer();
        animationPlayer.SpeedScale = (float)1.5;
        if (context.GetVelocity().X != 0)
        {
            animationPlayer.FlipH = velocity.X < 0;
        }
        PlayAnimation();
    }


    private void JudgeState(Vector2 dir)
    {
        if (dir.Y < 0)
        {
            EmitSignal(SignalName.StateChange, "Jump");
        }
        else if (context.IsDash)
        { 
            EmitSignal(SignalName.StateChange, "Dash");
        }
        if (context.GetVelocity().X == 0)
        {
            EmitSignal(SignalName.StateChange, "Idle");
        }

    }


}
