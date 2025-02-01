using Godot;
using System;
using System.Collections.Generic;

public partial class KeksSkillController
{
	private static KeksSkillController instance;
	public static KeksSkillController Instance
	{
		get
		{
			if (instance == null)
				return new KeksSkillController();
			return instance;
		}
		private set => instance = value;
	}

    private KeksSkillController(){}
	public static KeksSkillController CreateSkillController(AnimationPlayer animPlayer, Sprite2D idleSprite)
	{
		if(instance == null)
			instance = new KeksSkillController();

		instance._idleSprite = idleSprite;
        instance._animationPlayer = animPlayer;

        return instance;
	}

    public List<PlayerSkill> Skills { get; set; } = new List<PlayerSkill>();
	public List<PlayerSkill> CancellingSkills { get; set; } = new List<PlayerSkill>();

	private PlayerSkill CurrentSkill = null;
	private ulong CurrSkillStarted = 0;
	private Sprite2D _idleSprite;
	private AnimationPlayer _animationPlayer;


    public void RegisterSkill(PlayerSkill skill)
	{
		Skills.Add(skill);
		DoIdle(null);
    }

	/// <summary>
	/// Skills that can cancel anims like block on zerk. Maybe iframe?
	/// </summary>
	/// <param name="animCancellingSkill"></param>
	public void RegisterAnimCancellingSkill(PlayerSkill animCancellingSkill)
	{
		CancellingSkills.Add(animCancellingSkill);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _Process(double delta)
	{
        var elapsedSinceCast = Time.GetTicksMsec() - CurrSkillStarted;
		HandleSkillEnding(elapsedSinceCast);
		HandleSkillCanceling(elapsedSinceCast);
		HandleSkillCastAndChaining(elapsedSinceCast);
		HandleSkillRelase(elapsedSinceCast);
    }

	private void HandleSkillEnding(ulong elapsedSinceCast)
	{
        if (CurrentSkill?.AnimationLength <= elapsedSinceCast)
        {
            GD.PrintErr($"ANIM END GOING IDLE, ELAPSED: {elapsedSinceCast}");
            DoIdle(CurrentSkill);
        }
    }

	private void HandleSkillCanceling(ulong elapsedSinceCast)
	{
        foreach (PlayerSkill cancellingSkill in CancellingSkills)
        {
            if (Input.IsActionPressed(cancellingSkill.InputmapName))
            {
                if (CurrentSkill == cancellingSkill)
                {
                    continue;
                }
                if (CurrentSkill == null)
                {
                    StartSkill(cancellingSkill);
                }
                else if (CurrentSkill != null)
                {
                    if (CanAnimCancel(CurrentSkill,elapsedSinceCast))
                        StartSkill(cancellingSkill);
                }
            }
        }
    }

	private void HandleSkillCastAndChaining(ulong elapsedSinceCast)
	{
        foreach (PlayerSkill skill in Skills)
        {
            if (Input.IsActionPressed(skill.InputmapName))
            {
                if (CurrentSkill == skill)
                {
                    continue;
                }
                if (CurrentSkill == null)
                {
                    GD.Print($"SKILL FROM IDLE: {skill.AnimationName}");
                    StartSkill(skill);
                }
                else if (CurrentSkill != null && CanChainFromTo(CurrentSkill, skill))
                {
                    GD.Print($"CANNOT CHAIN YET ELAPSED: {elapsedSinceCast}");
                    if (IsInTimeFrameToChain(CurrentSkill, elapsedSinceCast))
                    {
                        GD.PrintErr($"CHAINING INTO{skill.AnimationName} CURR ELAPSED: {elapsedSinceCast}");

                        StartSkill(skill.ChainedVersion);
                    }

                }
            }

        }
    }

	private void HandleSkillRelase(ulong elapsedSinceCharging)
	{
        foreach (PlayerSkill skill in Skills)
        {
            if (Input.IsActionJustReleased(skill.InputmapName) && skill.IsChargable && CurrentSkill == skill)
            {
                if (CurrentSkill.ChainedVersion != null)
                {
                    GD.Print($"CHARGED FOR {elapsedSinceCharging} MS, CHARGED INTO: {CurrentSkill.ChainedVersion.AnimationName}");
                    StartSkill(CurrentSkill.ChainedVersion);
                }
            }
        }
        // THIS COULD BE EXPLOITED BEHAVIOUR IF NO OTHER SKILL WILL BE MADE AS A CANCELLING SKILL BUT BLOCK WHICH HAS NO SECOND ANIM AFTER CHARGE
        foreach (PlayerSkill skill in CancellingSkills)
        {
            if (Input.IsActionJustReleased(skill.InputmapName) && skill.IsChargable && CurrentSkill == skill)
            {
                GD.Print($"Released chargable cancelling skill after: {elapsedSinceCharging}");
                DoIdle(skill);
            }
        }
    }

	private bool CanChainFromTo(PlayerSkill from, PlayerSkill to)
	{
		if (to.ChainsFrom is null)
			return false;

        return to.ChainsFrom.Contains(from.AnimationName);
	}

	private bool IsInTimeFrameToChain(PlayerSkill toChainFrom, ulong elapsed)
	{
        return toChainFrom.RequiredElapseUntilChain < elapsed;
	}

	private void StartSkill(PlayerSkill skill) 
	{
		CurrSkillStarted = Time.GetTicksMsec();
        PlayAnim(CurrentSkill,skill);
        CurrentSkill = skill;
    }

    private bool CanAnimCancel(PlayerSkill toCancel, ulong elapsedSinceCast)
    {
        if (toCancel.CancelTimeFrame is null)
        {
            GD.Print("Not cancellable skill");
            return false;
        }
        if (toCancel.ActiveCancelPunishmentUntil > elapsedSinceCast)
        {
            GD.Print("Cannot skill cancel because PUNISHMENT is ACTIVE");
            return false;
        }
        if( toCancel.CancelTimeFrame.Item1< elapsedSinceCast && elapsedSinceCast < toCancel.CancelTimeFrame.Item2)
        {
            GD.Print("Succesfull skil cancel");
            return true;
        }
        GD.PrintErr("Failed to cancel ACTIVATING PUNISHMENT");
        toCancel.ActivateCancelPunishment(elapsedSinceCast);
        return false;
    }

	private void PlayAnim(PlayerSkill from, PlayerSkill to)
	{
        _idleSprite.Visible = false;
		MakeInvisible(from);


        to.Sprite.Visible = true;

		_animationPlayer.Play(to.AnimationName);
    }
	private void DoIdle(PlayerSkill from)
	{
        MakeInvisible(from);

        CurrentSkill = null;

        _idleSprite.Visible = true;
		_animationPlayer.Play("idle");
	}
	private void MakeInvisible(PlayerSkill skill)
	{
        if (skill != null)
        {
            skill.Sprite.Visible = false;
            if (skill.ChainedVersion != null)
                skill.ChainedVersion.Sprite.Visible = false;
        }
    }
}
