using Godot;
using System;

public partial class Node2d : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var sdads = this;
        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
            ClientSingleton.Socket.SendMessage("fuk you from BOB").Wait();
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

        //var test1 = GetNode<Node2D>("Node2D");
        var asd = GetNode<Node2D>("asd");
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Console.Write("ASD");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * 30;
            animatedSprite2D.Play();

            Position += velocity * (float)delta;
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, 1920),
                y: Mathf.Clamp(Position.Y, 0, 1080)
            );
        }
        else
        {
            animatedSprite2D.Stop();
        }
    }
}
