using Godot;
using System;

namespace PInfo
{
	public class PlayerInfo
	{
		public static string Name { get; private set; }
		public static string UUID { get; private set; }


		public static void SetPlayerInfo(string playerName, string playerUUID)
		{
			Name = playerName;
			UUID = playerUUID;
		}
	}
}
