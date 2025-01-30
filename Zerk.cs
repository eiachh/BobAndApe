using Godot;
using System;

public partial class Zerk : Area2D
{
    // Called when the node enters the scene tree for the first time.
    private AnimationPlayer _animPlayer;
    public override void _Ready()
	{
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
        }
        if (Input.IsActionPressed("thunderstrike"))
        {
            _animPlayer.Play("thunder");
        }
        if (Input.IsActionPressed("block"))
        {
            _animPlayer.Play("block");
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * 200;


            Position += velocity * (float)delta;
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, 1920),
                y: Mathf.Clamp(Position.Y, 0, 1080)
            );
        }
    }
}
