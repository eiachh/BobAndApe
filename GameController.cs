using Godot;
using PInfo;
using System;
using BobAndApe.scenes;

/// <summary>
/// Singleton get with "GetNode<GameController>("/root/GameController");"
/// </summary>
public partial class GameController : Node
{
    private const string _serverIP = "84.3.90.43";
    private const int _serverPort = 7070;

    public Player Player { get; set; }

    public Node CurrentScene { get; private set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		ClientSocket.Init(_serverIP, _serverPort);
		CurrentScene = GetTree().CurrentScene;

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnLoginSuccess(string userName, string uuid)
	{
		Player player = new Player(userName, uuid);
		this.Player = player;

		LoadScene(SceneDict.GameScenes.ExampleLobby);
	}

	public void LoadScene(SceneDict.GameScenes toLoad)
	{
		var sceneTSCN = SceneDict.GameSceneTSCN(toLoad);
        GetTree().ChangeSceneToFile(sceneTSCN);
		CurrentScene = GetTree().CurrentScene;
    }
}
