using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobAndApe.Skills
{
    public class BossSkillController
    {
        public float CastSpeedScale { get; set; } = 1.0f;

        private List<Skill> _skills { get; set; } = new List<Skill>();
        private AnimatedSprite2D _animationBoss = null;
        private Skill _chainInto = null;

        private Skill _currentSkill = null;
        private ulong _currSkillStarted = 0;
        public BossSkillController(AnimatedSprite2D animBoss)
        {
            _animationBoss = animBoss;
            DoIdle();
        }
        public void AddSkill(Skill skill)
        {
            _skills.Add(skill);
        }

        public void _Process(double delta)
        {
            var elapsedSinceCast = Time.GetTicksMsec() - _currSkillStarted;
            HandleSkillEnding(elapsedSinceCast);
        }
        public void StartSkill(Skill skill)
        {
            skill.CastSpeedModifier = CastSpeedScale;
            _animationBoss.SpeedScale = CastSpeedScale;
            _currSkillStarted = Time.GetTicksMsec();
            _animationBoss.Play(skill.AnimationName);
            _currentSkill = skill;
        }

        private void HandleSkillEnding(ulong elapsedSinceCast)
        {
            if (_currentSkill?.AnimationLength <= elapsedSinceCast)
            {
                if (_chainInto != null)
                {
                    GD.PrintErr($"BOSS ANIM END CHAINING INTO: {_chainInto}, ELAPSED: {elapsedSinceCast}");
                    StartSkill(_chainInto);
                }
                GD.PrintErr($"BOSS ANIM END GOING IDLE, ELAPSED: {elapsedSinceCast}");
                DoIdle();
            }
        }

        private void DoIdle()
        {
            _currentSkill = null;
            _animationBoss.Play("idle");
            _animationBoss.SpeedScale = 1.0f;
        }
    }
}
