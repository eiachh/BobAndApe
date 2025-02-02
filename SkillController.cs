using Godot;
using System;
using System.Collections.Generic;

public partial class SkillController
{
    private SkillController() {}
    /// <summary>
    /// Static singleton instance controller, or returns the existing instance. Needs to be init'ed at least once or the animations won't play.
    /// </summary>
    /// <param name="animPlayer"></param>
    /// <returns></returns>
    public static SkillController InitSkillController(AnimatedSprite2D animPlayer)
    {
        if (instance == null)
            instance = new SkillController();

        instance._animationPlayer = animPlayer;
        return instance;
    }

    private static SkillController instance;

    /// <summary>
    /// NEED TO INIT AT LEAST ONCE.
    /// Returns the singleton instance. 
    /// </summary>
    public static SkillController Instance
    {
        get
        {
            if (instance == null)
                return new SkillController();
            return instance;
        }
        private set => instance = value;
    }

    /// <summary>
    /// Changes the casting speed (applies from the next skill cast)
    /// </summary>
    public float CastSpeedScale { get; set; } = 1.0f;

    /// <summary>
    /// List of all the normal skills
    /// </summary>
    private List<PlayerSkill> Skills { get; set; } = new List<PlayerSkill>();

    /// <summary>
    /// List of all the cancelling skills.
    /// </summary>
    private List<PlayerSkill> CancellingSkills { get; set; } = new List<PlayerSkill>();

    /// <summary>
    /// The skill being played right now.
    /// </summary>
    private PlayerSkill CurrentSkill = null;

    /// <summary>
    /// The timestamp when the currently playing skill was started.
    /// </summary>
    private ulong _currSkillStarted = 0;

    /// <summary>
    /// The animationplayer used to actually play the anims.
    /// </summary>
    private AnimatedSprite2D _animationPlayer;

    /// <summary>
    /// THIS IS NOT A GAME NODE SO THIS HAS TO BE CALLED BY THE PARENTS PROCESS THIS WON'T AUTO PLAY.
    /// Should be called on each frame. 
    /// THIS IS NOT A GAME NODE SO THIS HAS TO BE CALLED BY THE PARENTS PROCESS THIS WON'T AUTO PLAY.
    /// </summary>
    public void _Process(double delta)
    {
        var elapsedSinceCast = Time.GetTicksMsec() - _currSkillStarted;
        HandleSkillEnding(elapsedSinceCast);
        HandleSkillCanceling(elapsedSinceCast);
        HandleSkillCastAndChaining(elapsedSinceCast);
        HandleSkillRelase(elapsedSinceCast);
    }

    /// <summary>
    /// Registers normal skills.
    /// </summary>
    public void RegisterSkill(PlayerSkill skill)
    {
        Skills.Add(skill);
        DoIdle();
    }

    /// <summary>
    /// Skills that can cancel anims like block on zerk. Maybe iframe?
    /// </summary>
    public void RegisterAnimCancellingSkill(PlayerSkill animCancellingSkill)
    {
        CancellingSkills.Add(animCancellingSkill);
    }

    /// <summary>
    /// Transitions into idle state if the skill ended naturally by elapsed time.
    /// </summary>
    private void HandleSkillEnding(ulong elapsedSinceCast)
    {
        if (CurrentSkill?.AnimationLength <= elapsedSinceCast)
        {
            GD.PrintErr($"ANIM END GOING IDLE, ELAPSED: {elapsedSinceCast}");
            DoIdle();
        }
    }

    /// <summary>
    /// Determines if the skill is in a cancellable state and allows the cancelling skill to do so. Currently you cannot define different skills to be able to cancel at diff times.
    /// Maybe another loop could allow dodge to cast whenever ( if we add dodge )
    /// </summary>
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
                    if (CanAnimCancel(CurrentSkill, elapsedSinceCast))
                        StartSkill(cancellingSkill);
                }
            }
        }
    }

    /// <summary>
    /// Handles generic skill casting and chaining generic skills into each other. For now you cannot chain a sharging skill.
    /// You can "chain a cancelling skill but not because of this handle"
    /// </summary>
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
                else if (CurrentSkill != null && CurrentSkill.CanChainTo(skill))
                {
                    GD.Print($"CANNOT CHAIN YET ELAPSED: {elapsedSinceCast}");
                    if (IsInTimeFrameToChain(CurrentSkill, elapsedSinceCast))
                    {
                        GD.PrintErr($"CHAINING INTO{skill.ChainedVersion} CURR ELAPSED: {elapsedSinceCast}");

                        StartSkill(skill.ChainedVersion);
                    }

                }
            }

        }
    }

    /// <summary>
    /// Handls what should happen on skill release. On chargable skills it progresses to the follow up anim that comes after the charge.
    /// On chargable skills that are cancelling it just cancels the anim (its only the zerk block by far).
    /// On other cases nothing happens.
    /// </summary>
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
                DoIdle();
            }
        }
    }

    /// <summary>
    /// Determines wether the given skill is in the timeframe to allow chaining based on the elapsed time.
    /// </summary>
    private bool IsInTimeFrameToChain(PlayerSkill toChainFrom, ulong elapsedSinceCast)
    {
        return toChainFrom.RequiredElapseUntilChain < elapsedSinceCast;
    }

    /// <summary>
    /// Starts to cast a skill. Handles animation, cast speed, timeframe scaling. Sets CurrentSkill to the given, and the CurrSkillStarted for elapsed time calc.
    /// </summary>
    private void StartSkill(PlayerSkill skill)
    {
        skill.CastSpeedModifier = CastSpeedScale;
        _animationPlayer.SpeedScale = CastSpeedScale;
        _currSkillStarted = Time.GetTicksMsec();
        _animationPlayer.Play(skill.AnimationName);
        CurrentSkill = skill;
    }

    /// <summary>
    /// Determines wether the given skill is cancellable at this exact moment. If in cancellable timeframe returns true.
    /// If it is not cancellable because of inappropriate timeframe it applies a punishment delay.
    /// If it is not cancellable because of the punishment it just returns false.
    /// </summary>
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
        if (toCancel.CancelTimeFrame.Item1 < elapsedSinceCast && elapsedSinceCast < toCancel.CancelTimeFrame.Item2)
        {
            GD.Print("Succesfull skil cancel");
            return true;
        }
        GD.PrintErr("Failed to cancel ACTIVATING PUNISHMENT");
        toCancel.ActivateCancelPunishment(elapsedSinceCast);
        return false;
    }

    /// <summary>
    /// Changes state to idle, including anim, current skill.
    /// </summary>
    private void DoIdle()
    {
        CurrentSkill = null;
        _animationPlayer.Play("idle");
        _animationPlayer.SpeedScale = 1.0f;
    }
}
