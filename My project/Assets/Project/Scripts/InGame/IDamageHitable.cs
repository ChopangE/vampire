using System;
using UnityEngine;

namespace InGame
{
    public interface IDamageHitable
    {
        public event Action<float> OnHit;
        void Dead();
    }
}
