using Godot;
using System;
using Package;
using System.Threading.Tasks;

public partial class Node2d : Node2D
{

	private Vector2 lastSentPosition = Vector2.Zero;
	private Vector2 pos = Vector2.Zero;
	private float timeSinceLastUpdate = 0f;
	private const float updateInterval = 1f / 120f;
	private const float positionThreshold = 2f;
	public bool n = true;




	public override void _Ready()
	{

	}

	public override async void _Process(double delta)
	{
		timeSinceLastUpdate += (float)delta;
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			ClientSocket.SendMessage(PackageFactory.CreateMovePackage(Position));
		}

		if (Input.IsActionPressed("move_left") && n )
		{
			velocity.X -= 1;
			n = false;
			ClientSocket.SendMessage(PackageFactory.CreateLoginPackage("bobby tables"));
			object response = await ClientSocket.ReceiveMessage();
			LoginCommand lcResponse = (LoginCommand)response;
			GD.Print(lcResponse.Body.UserID);
			
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized()*200;
		
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
		if (timeSinceLastUpdate >= updateInterval || HasSignificantPositionChange())
		{
			//ClientSocket.SendMessage(PackageFactory.CreateMovePackage(Position)).Wait();

			// Reset timer
			timeSinceLastUpdate = 0f;
			lastSentPosition = pos;
		}
	}

	private bool HasSignificantPositionChange()
	{
		return Position.DistanceTo(lastSentPosition) >= positionThreshold;
	}
}
