using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


public class PlayerSkill 
{
    private HashSet<string> _chainsFrom = new();
    private ulong _animationLength;


    public float CastSpeedModifier { get; set; } = 1.0f;
    public string AnimationName { get; set; }
    public ulong AnimationLength 
    {
        get => Convert.ToUInt64((int)Math.Round(_animationLength / CastSpeedModifier));
    }
    public string InputmapName { get; set; }
    public bool IsChargable { get; set; } = false;
    public Tuple<ulong,ulong> CancelTimeFrame{ get; private set; }
    public ulong ActiveCancelPunishmentUntil { get; private set; }
    public ulong DefaultCancelPunishment { get; private set; } = 300;
    // Timeframe from allowed to chain cast next
    public ulong RequiredElapseUntilChain { get; private set; }
    public IReadOnlySet<string> ChainsFrom { get => _chainsFrom; }
    public PlayerSkill ChainedVersion { get; private set; }

    public PlayerSkill(string animName, ulong animLength, string inputMapName)
    {
        AnimationName = animName;
        _animationLength = animLength;
        InputmapName = inputMapName;
    }

    public void CreateSkillChainTo(PlayerSkill chainTo, PlayerSkill chainToSpedUpVersion,ulong requiredElapseUntilChain)
    {
        this.RequiredElapseUntilChain = requiredElapseUntilChain;
        chainTo._chainsFrom.Add(this.AnimationName);
        chainTo.ChainedVersion = chainToSpedUpVersion;
    }

    public void ConvertSkillToCharging(PlayerSkill chainToAfterCharge)
    {
        // Charging skills are not chainable ( for now at least )
        if (this.RequiredElapseUntilChain != 0)
            return;
        this.IsChargable = true;
        this.ChainedVersion = chainToAfterCharge;
    }

    /// <summary>
    /// Converts the skill to cancellable from til the timestamps of elapsed delta time. By default it makes the whole skill cancellable whenever.
    /// </summary>
    /// <param name="cancellableFrom"></param>
    /// <param name="cancellableTill"></param>
    public void MakeSkillCancellable(ulong cancellableFrom = 0, ulong cancellableTill = 0)
    {
        if (cancellableTill == 0 )
            cancellableTill = this.AnimationLength;
        CancelTimeFrame = new Tuple<ulong, ulong>(cancellableFrom, cancellableTill);
    }

    public void ActivateCancelPunishment(ulong elapsedSinceCast)
    {
        ActiveCancelPunishmentUntil = DefaultCancelPunishment + elapsedSinceCast;
    }
}
