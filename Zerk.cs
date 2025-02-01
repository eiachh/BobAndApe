using Godot;
using System;

public partial class Zerk : Area2D
{
    // Called when the node enters the scene tree for the first time.
    private KeksSkillController _skillController;
    public override void _Ready()
	{
        //var animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        //var idleSprite = GetNode<Sprite2D>("Idle");

        //_skillController = SkillController.CreateSkillController(animPlayer, idleSprite);

        //var thunderSprite = GetNode<Sprite2D>("ThunderSprite");
        //PlayerSkill skil1 = new PlayerSkill("thunder",2000, "thunderstrike", thunderSprite);
        //skil1.IsChargable = false;

        //var blockSprite = GetNode<Sprite2D>("BlockSprite");
        //PlayerSkill skilBlock = new PlayerSkill("block", 999999, "block", blockSprite);
        //skilBlock.IsChargable = true;

        //_skillController.RegisterSkill(skil1);
        //_skillController.RegisterAnimCancellingSkill(skilBlock);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //_skillController._Process(delta);
        //var velocity = Vector2.Zero; // The player's movement vector.

        //if (Input.IsActionPressed("move_right"))
        //{
        //    velocity.X += 1;
        //}

        //if (Input.IsActionPressed("move_left"))
        //{
        //    velocity.X -= 1;
        //}

        //if (Input.IsActionPressed("move_down"))
        //{
        //    velocity.Y += 1;
        //}

        //if (Input.IsActionPressed("move_up"))
        //{
        //    velocity.Y -= 1;
        //}
        ////if (Input.IsActionPressed("thunderstrike"))
        ////{
        ////    _animPlayer.Play("thunder");
        ////}
        ////if (Input.IsActionPressed("block"))
        ////{
        ////    //_animPlayer.Play("block");
        ////}

        //if (velocity.Length() > 0)
        //{
        //    velocity = velocity.Normalized() * 200;


        //    Position += velocity * (float)delta;
        //    Position = new Vector2(
        //        x: Mathf.Clamp(Position.X, 0, 1920),
        //        y: Mathf.Clamp(Position.Y, 0, 1080)
        //    );
        //}
    }
}
