using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    protected bool isStunned;
    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
        
        // 움직임 제어
        if (TryGetComponent<Enemy>(out var enemy))
        {
            enemy.isStunned = stunned;
        }
    }

}
