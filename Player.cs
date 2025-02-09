using Godot;
using System;

namespace PInfo
{
	public class Player
	{
		public static string Name { get; private set; }
		public static string UUID { get; private set; }

        public PlayerSkillController SkillController { get; private set; }

        public Player(string playerName, string playerUUID)
        {
            Name = playerName;
            UUID = playerUUID;
        }
	}
}
