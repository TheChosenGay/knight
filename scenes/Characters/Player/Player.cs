using System.Drawing;
using Godot;
using Knight.State.PlayerState;
using Knight.State.StateMachine;

public partial class Player : CharacterBody2D, IPlayerContextProtocol
{
    private float moveSpeed;
    private float gravity;

    private float accHorDelta;
    private Vector2 playerDirection;

    private AnimatedSprite2D animationPlayer;
    private CollisionShape2D collisionShape;
    private AudioStreamPlayer2D audioPlayer;
    private StateMachine stateMachine;
    private bool isCouldPlayAudio;
    private Timer audioTimer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimatedSprite2D>("Animation");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
        stateMachine = GetNode<StateMachine>("StateMachine");
        audioTimer = new Timer();
        audioTimer.Timeout += () =>
        {
            isCouldPlayAudio = true;
        };
        AddChild(audioTimer);
        Init();
        stateMachine.StartMachine();
    }




    public override void _Process(double delta)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        UpdateDirection();
    }


    public override void _PhysicsProcess(double delta)
    {
        stateMachine.StateMachinePhysicsProcess(delta);
        // var dir = GetDirection();

        // var velocity = Velocity;

        // accHorDelta += (float)delta;

        // var horDelta = IsOnFloor() ? accHorDelta / (float)2.0 : accHorDelta / (float)100.0;
        // velocity.X = (dir.X != 0) ? dir.X * moveSpeed : Mathf.Lerp(0, velocity.X, Mathf.Pow(2, -horDelta));

        // Callable.From(() =>
        // {
        //     if (IsOnFloor())
        //     {
        //         Velocity = new Vector2(Velocity.X, 0);
        //         if (Velocity.X == 0) accHorDelta = 0;
        //     }

        //     PlayAnimation(velocity);
        // }).CallDeferred();

        // Velocity = velocity;
        MoveAndSlide();

    }

    private void PlayAnimation(Vector2 velocity)
    {
        animationPlayer.SpeedScale = (float)1.0;
        if (velocity.X != 0)
        {
            animationPlayer.FlipH = velocity.X < 0;
        }
        if (IsOnFloor())
        {
            var aniName = Mathf.Abs(velocity.X) > 100 ? "run" : "idle";
            animationPlayer.SpeedScale = aniName == "run" ? 0.1f : 1.0f;
            animationPlayer.SpeedScale = 2.0f;
            animationPlayer.Play(aniName);
            if (aniName == "run")
            {
                PlayAudio();
            }
        }
        else
        {
        }

    }

    private void PlayAudio()
    {
        if (!audioPlayer.Playing && isCouldPlayAudio)
        {
            audioPlayer.Play();
            isCouldPlayAudio = false;
            audioTimer.Start(0.1);
            GD.Print("PlayAudio");
        }
    }


    private void Init()
    {
        gravity = 800;
        moveSpeed = 300;
        accHorDelta = 0;
        isCouldPlayAudio = true;

    }
    private void UpdateDirection()
    {
        Vector2 direction = new(0, 0);
        direction.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        if (Input.IsActionJustPressed("jump"))
        {
            direction.Y = -Input.GetActionStrength("jump");
        }
        playerDirection = direction;
    }


    // MARK: IPlayerContextProtocol

    public CharacterBody2D GetPlayer()
    {
        return this;
    }
    // 音频和动画相关
    public AnimatedSprite2D GetAnimationPlayer()
    {
        return animationPlayer;
    }
    public AudioStreamPlayer2D GetAudioStreamPlayer()
    {
        return audioPlayer;
    }
    public void SetCollisionShapeSize(Vector2 size)
    {
        ((RectangleShape2D)collisionShape.Shape).Size = size;
    }

    // 逻辑判断相关
    public Vector2 GetDirection()
    {
        return playerDirection;
    }

    public float GetPlayerGravity()
    {
        return gravity;
    }

}
