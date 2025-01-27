using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Package
{
	public class PackageTypeMap
	{
		public static readonly Dictionary<string, Type> packageTypeMap = new()
		{
			{ "LoginCommand" , typeof(LoginCommand) },
			{ "MoveCommand" , typeof(MoveCommand) },
			{ "AttackCommand", typeof(AttackCommand) }
		};

	}

	public abstract class Package<T>
	{
		[JsonPropertyName("name")]
		public abstract string Name { get; }

		[JsonPropertyName("body")]
		public T Body { get; set; }
	}

	public class LoginCommand : Package<LoginBody>
	{
		public override string Name => "LoginCommand";
	}


	public class MoveCommand : Package<MoveBody>
	{
		public override string Name => "MoveCommand";
	}

	public class AttackCommand : Package<AttackBody>
	{
		public override string Name => "AttackCommand";
	}

	public class LoginBody
	{
		[JsonPropertyName("namerequest")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Username { get; set; }

		[JsonPropertyName("receiveduserid")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? UserID { get; set; }
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
		public string Target { get; set; }

		[JsonPropertyName("damage")]
		public int Damage { get; set; }

		[JsonPropertyName("damageType")]
		public string DamageType { get; set; }
	}
}
