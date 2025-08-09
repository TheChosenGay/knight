using Godot;

public partial class Player : CharacterBody2D
{
    private float moveSpeed;
    private float jumpSpeed;
    private float gravity;
    private bool isCanJump;
    private bool isShorJumping;
    private float jumpDistance;

    private float accHorDelta;

    private AnimatedSprite2D animationPlayer;
    private CollisionShape2D collisionShape;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimatedSprite2D>("Animation");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        Init();
    }




    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        var dir = GetDirection();

        var velocity = Velocity;

        accHorDelta += (float)delta;
      
        if (isCanJump && dir.Y < 0)
        {
            velocity.Y = dir.Y * jumpSpeed;
            isCanJump = false;
        }

        jumpDistance += -(float)(velocity.Y * delta);
        if (!IsOnFloor() && velocity.Y < 0 && jumpDistance < 120) 
        {
            isShorJumping = !Input.IsActionPressed("jump");
        }

        var horDelta = IsOnFloor() || isShorJumping ? accHorDelta / (float)2.0 : accHorDelta / (float)100.0;
        velocity.X = (dir.X != 0) ? dir.X * moveSpeed : Mathf.Lerp(0, velocity.X, Mathf.Pow(2, -horDelta));
     

        var realGravity = isShorJumping ? gravity * 5 : gravity;
        velocity.Y += realGravity * (float)delta;
        velocity.Y = Mathf.Clamp(velocity.Y, -500, 500);


        Callable.From(() =>
        {
            if (IsOnFloor())
            {
                Velocity = new Vector2(Velocity.X, 0);
                isCanJump = true;
                isShorJumping = false;
                jumpDistance = 0;
                if(Velocity.X == 0) accHorDelta = 0;
            }

            PlayAnimation(velocity);
        }).CallDeferred();

        Velocity = velocity;
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
            animationPlayer.SpeedScale = 2.0f;
            animationPlayer.Play(aniName);
        }
        else
        {
            var animationName = velocity.Y > 0 ? "fall" : "jump";
            float shapeHeight = velocity.Y > 0 ? (float)120.0 : (float)134.0;
            ((RectangleShape2D)collisionShape.Shape).Size = new Vector2((float)40.0, shapeHeight);
            animationPlayer.SpeedScale = (float)0.3;
            animationPlayer.Play(animationName);
        }
    }



    private void Init()
    {
        gravity = 800;
        moveSpeed = 300;
        jumpSpeed = 500;
        jumpDistance = 0;
        accHorDelta = 0;

        isCanJump = IsOnFloor();
        isShorJumping = false;
    }
    private Vector2 GetDirection()
    {
        Vector2 direction = new(0, 0);
        direction.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        if (Input.IsActionJustPressed("jump"))
        {
            direction.Y = -Input.GetActionStrength("jump");
        }
        return direction;
    }
    
}
