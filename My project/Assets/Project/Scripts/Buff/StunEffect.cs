using System.Collections;
using System.Collections.Generic;
using ModiBuff.Core;
using UnityEngine;

public class StunEffect : IEffect
{
    public void Effect(IUnit target, IUnit source)
    {
        // 스턴 상태 적용
        if (target is EnemyUnit enemyUnit)
        {
            enemyUnit.SetStunned(true);
        }
    }
}