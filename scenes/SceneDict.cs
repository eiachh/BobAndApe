using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyreanIsRetarded.scenes
{
    public static class SceneDict
    {
        public enum GameScenes
        {
            LoginMenu,
            ExampleLobby,
            ExampleBossArena
        }

        public static string GameSceneTSCN(GameScenes scene)
        {
            if (Scenes.ContainsKey(scene))
            {
                return Scenes[scene];
            }
            return "SCENE NOT REGISTERED SceneDict";
        }

        private static Dictionary<GameScenes, string> Scenes = new Dictionary<GameScenes, string>
        {
            { GameScenes.LoginMenu, "res://scenes/uis/login_menu.tscn" },
            { GameScenes.ExampleLobby, "res://scenes/uis/example_scene_load.tscn" },
            { GameScenes.ExampleBossArena, "res://scenes/areas/example_area.tscn" },
        };
    }
}
