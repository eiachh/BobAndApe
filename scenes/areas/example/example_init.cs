using Godot;
using System;

public partial class example_init : Node2D
{
    [Export] public PackedScene NpcScene;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        SpawnNPC(new Vector2(100, 200));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    private void SpawnNPC(Vector2 position)
    {
        if (NpcScene == null)
        {
            GD.PrintErr("NpcScene is not assigned!");
            return;
        }

        Node2D npcInstance = NpcScene.Instantiate<Node2D>(); // Instantiate the NPC
        npcInstance.Position = position; // Set position
        AddChild(npcInstance); // Add to the scene tree
    }
}
