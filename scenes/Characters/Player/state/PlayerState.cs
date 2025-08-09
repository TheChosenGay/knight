using System;
using Godot;
namespace Knight.State.PlayerState;

public interface IPlayerContextProtocol
{
    public CharacterBody2D GetPlayer();
    // 音频和动画相关
    public AnimatedSprite2D GetAnimationPlayer();
    public AudioStreamPlayer2D GetAudioStreamPlayer();
    public void SetCollisionShapeSize(Vector2 size);
    
    // 逻辑判断相关
    public Vector2 GetDirection();
    public bool IsOnFloor();

    // 移动相关
    public Vector2 GetVelocity();
    public void SetVelocity(Vector2 velocity);
    public float GetPlayerGravity();


    
}


[GlobalClass]
public partial class PlayerState : State
{
    [Export]
    protected CharacterBody2D player;
    [Export]
    private String animationName;

    protected IPlayerContextProtocol context;
    protected float gravity;

    public override void _Ready()
    {
        base._Ready();
        if (player is IPlayerContextProtocol)
        {
            context = (IPlayerContextProtocol)player;
        }
    }


    public virtual void PlayAnimation()
    {
        context.GetAnimationPlayer().Play(animationName);
    }
    public virtual void StopAnimation()
    {
        context.GetAnimationPlayer().Stop();
    }


    public override void Enter()
    {
        base.Enter();
        gravity = context.GetPlayerGravity();
        Callable.From(() =>
        {
            PlayAnimation();
        }).CallDeferred();
    }

    public override void Exit()
    {
        base.Exit();
        Callable.From(() =>
        {
            StopAnimation();
        }).CallDeferred();
    }


}
