using Godot;
using System;
using BobAndApe.scenes;

public partial class ExampleDungEnter : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.Pressed += OnButtonPressed;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    private void OnButtonPressed()
    {
        var gc = GetNode<GameController>("/root/GameController");
        gc.LoadScene(SceneDict.GameScenes.ExampleBossArena);
    }
}
