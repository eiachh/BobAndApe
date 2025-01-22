using Godot;
using Package;

public static class PackageFactory
{
	public static Package<LoginBody> CreateLoginPackage(string username)
	{
		return new LoginCommand
		{
			Body = new LoginBody
			{
				Username = username
			}
		};
	}

	public static Package<MoveBody> CreateMovePackage(Vector2 pos) 
	{
		return new MoveCommand
		{
			Body = new MoveBody
			{
				MoveCommandType = "sync",
				PosX = (int)pos.X,
				PosY = (int)pos.Y
			}
		};
	}
}
