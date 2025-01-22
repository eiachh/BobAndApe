using System.Text.Json.Serialization;

namespace Package
{
	public abstract class Package<T>
	{
		[JsonPropertyName("name")]
		public abstract string Name { get; }

		[JsonPropertyName("body")]
		public T Body { get; set; }
	}

	// LoginCommand implementation
	public class LoginCommand : Package<LoginBody>
	{
		public override string Name => "LoginCommand";
	}

	// MoveCommand implementation
	public class MoveCommand : Package<MoveBody>
	{
		public override string Name => "MoveCommand";
	}

	public class AttackComand : Package<AttackBody>
	{
		public override string Name => "AttackCommand";
	}

	public class LoginBody
	{
		[JsonPropertyName("username")]
		public string Username { get; set; }
	}

	public class MoveBody
	{
		[JsonPropertyName("movecommandtype")]
		public string MoveCommandType { get; set; } = "sync";

		[JsonPropertyName("posx")]
		public int PosX { get; set; }

		[JsonPropertyName("posy")]
		public int PosY { get; set; }
	}

	public class AttackBody
	{
		[JsonPropertyName("damage")]
		public int damage { get; set; } 

		[JsonPropertyName("damageType")]
		public string damageType { get;set; }
	}
}
