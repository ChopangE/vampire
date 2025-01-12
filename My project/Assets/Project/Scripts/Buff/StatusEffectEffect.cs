using System.Collections;
using System.Collections.Generic;
using ModiBuff.Core;
using UnityEngine;

namespace Buff
{
    public class StatusEffectEffect : IEffect
    {
        private readonly StatusEffectType _statusEffectType;
        private readonly float _duration;

        public StatusEffectEffect(StatusEffectType statusEffectType, float duration)
        {
            _statusEffectType = statusEffectType;
            _duration = duration;
        }

        public void Effect(IUnit target, IUnit source)
        {
            Debug.Log($"Applied {_statusEffectType} to {target} for " + _duration + " seconds");
            // ((ISingleStatusEffectOwner)target).StatusEffectController
            //     .ChangeStatusEffect(_statusEffectType, _duration);

        }
    }
}