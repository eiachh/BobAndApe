using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyreanIsRetarded.Skills
{
    public class Skill
    {
        public string AnimationName { get; set; }
        public float CastSpeedModifier { get; set; } = 1.0f;
        public ulong AnimationLength
        {
            get => RecalculateWithSpeed(_animationLength);
            private set => _animationLength = value;
        }

        private ulong _animationLength;
        public Skill(string animName, ulong animLength)
        {
            AnimationName = animName;
            _animationLength = animLength;
        }

        protected ulong RecalculateWithSpeed(ulong original)
        {
            if (CastSpeedModifier == 1.0f)
                return original;
            return Convert.ToUInt64((int)Math.Round(original / CastSpeedModifier));
        }
    }
}
