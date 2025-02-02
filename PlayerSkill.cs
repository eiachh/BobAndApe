using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


public class PlayerSkill 
{
    private HashSet<string> _chainsFrom = new();
    private ulong _animationLength;
    private ulong _requiredElapseUntilChain;
    private ulong _activeCancelPunishmentUntil;
    private ulong _defaultCancelPunishment=300;
    private Tuple<ulong, ulong> _cancelTimeFrame;


    public float CastSpeedModifier { get; set; } = 1.0f;
    public string AnimationName { get; set; }
    public ulong AnimationLength 
    {
        get => RecalculateWithSpeed(_animationLength);
        private set => _animationLength = value;
    }
    public string InputmapName { get; set; }
    public bool IsChargable { get; set; } = false;
    public Tuple<ulong,ulong> CancelTimeFrame
    {
        get => new Tuple<ulong, ulong>(RecalculateWithSpeed(_cancelTimeFrame.Item1), RecalculateWithSpeed(_cancelTimeFrame.Item2));
        private set => _cancelTimeFrame = value;
    }
    public ulong ActiveCancelPunishmentUntil 
    {
        get => RecalculateWithSpeed(_activeCancelPunishmentUntil);
        private set => _activeCancelPunishmentUntil = value;
    }
    public ulong DefaultCancelPunishment 
    {
        get => RecalculateWithSpeed(_defaultCancelPunishment);
        private set => _defaultCancelPunishment = value;
    }
    // Timeframe from allowed to chain cast next
    public ulong RequiredElapseUntilChain 
    {
        get => RecalculateWithSpeed(_requiredElapseUntilChain);
        private set => _requiredElapseUntilChain = value; 
    }
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
    public void ResetCancelPunishment()
    {
        ActiveCancelPunishmentUntil = 0;
    }
    public bool CanChainTo(PlayerSkill to)
    {
        if (to.ChainsFrom is null)
            return false;

        return to.ChainsFrom.Contains(this.AnimationName);
    }
    private ulong RecalculateWithSpeed(ulong original)
    {
        if (CastSpeedModifier == 1.0f)
            return original;
        return Convert.ToUInt64((int)Math.Round(original / CastSpeedModifier));
    }
}
