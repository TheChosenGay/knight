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
    private bool isDash;
    private bool isFaceRight;

    private AnimatedSprite2D animationPlayer;
    private CollisionShape2D collisionShape;
    private AudioStreamPlayer2D audioPlayer;
    private StateMachine stateMachine;
    private bool isCouldPlayAudio;
    private Timer audioTimer;

    public bool IsDash { get => isDash; set => isDash = value; }
    public bool IsFaceRight { get => isFaceRight;}    


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

        // 设置碰撞黏着距离
        FloorSnapLength = 0;
    }




    public override void _Process(double delta)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        isDash = Input.IsActionJustPressed("dash");
        UpdateDirection();
    }


    public override void _PhysicsProcess(double delta)
    {
        stateMachine.StateMachinePhysicsProcess(delta);
        MoveAndSlide();
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
        gravity = 1200;
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
        isFaceRight = direction.X > 0 ||(direction.X == 0 && isFaceRight);
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
    public void SetCollisionShapeSize(Vector2 size, Vector2? pos)
    {
        ((RectangleShape2D)collisionShape.Shape).Size = size;
        if (pos != null)
        {
            collisionShape.Position = (Vector2)pos;
        }
    }

    public CollisionShape2D GetCollisionShape()
    {
        return collisionShape;
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
