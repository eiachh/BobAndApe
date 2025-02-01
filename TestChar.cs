using Godot;
using System;

public partial class TestChar : Area2D
{
    private KeksSkillController _skillController;
    public override void _Ready()
	{
        var animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        var idleSprite = GetNode<Sprite2D>("idle");

        _skillController = KeksSkillController.CreateSkillController(animPlayer, idleSprite);

        // CHAINABLE SKILL
        var chainSourceSprite = GetNode<Sprite2D>("chain-source");
        PlayerSkill chainSourceSkill = new PlayerSkill("chain-source", 1500, "skill_2", chainSourceSprite);
        var chainIntoSprite = GetNode<Sprite2D>("chained-into");
        PlayerSkill chainedIntoSkill_chained = new PlayerSkill("chained-into", 1000, "NONE", chainIntoSprite);
        var notChainInto = GetNode<Sprite2D>("not-chained-into");
        PlayerSkill chainedIntoSkill_original = new PlayerSkill("not-chained-into", 2000, "skill_3", notChainInto);
        chainSourceSkill.CreateSkillChainTo(chainedIntoSkill_original, chainedIntoSkill_chained, 1000);
        // CHAINABLE SKILL

        // CHARGABLE SKILL
        var chargingSprite = GetNode<Sprite2D>("charging");
        PlayerSkill chargingSkill1 = new PlayerSkill("charging", 999999, "skill_1", chargingSprite);
        chargingSkill1.MakeSkillCancellable();

        var chargeIntoSprite = GetNode<Sprite2D>("charge-into");
        PlayerSkill chargeIntoSkill = new PlayerSkill("charge-into", 500, "NONE", chargeIntoSprite);
        chargingSkill1.ConvertSkillToCharging(chargeIntoSkill);
        // CHARGABLE SKILL

        // ANIM CANCELLING BLOCK
        var blockSprite = GetNode<Sprite2D>("block");
        PlayerSkill skilBlock = new PlayerSkill("block", 999999, "block", blockSprite);
        skilBlock.IsChargable = true;
        // ANIM CANCELLING BLOCK

        _skillController.RegisterSkill(chainSourceSkill);
        _skillController.RegisterSkill(chainedIntoSkill_original);
        _skillController.RegisterSkill(chargingSkill1);


        _skillController.RegisterAnimCancellingSkill(skilBlock);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        _skillController._Process(delta);

        var velocity = Vector2.Zero;
        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
        }
        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * 200;


            Position += velocity * (float)delta;
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, 1920),
                y: Mathf.Clamp(Position.Y, 0, 1080)
            );
        }
    }
}
