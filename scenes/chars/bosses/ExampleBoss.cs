using Godot;
using BobAndApe.Skills;
using System;

public partial class ExampleBoss : Area2D
{
	private BossSkillController _skillController;
    public override void _Ready()
	{
        var animBoss = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_skillController = new BossSkillController(animBoss);

		Skill skill1 = new Skill("skill1", 500);
        Skill skill2 = new Skill("skill2", 1000);

		_skillController.AddSkill(skill1);
		_skillController.AddSkill(skill2);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_skillController._Process(delta);
	}
}
