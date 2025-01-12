using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    protected bool isStunned;
    public void SetStunned(bool stunned, float stunTime)
    {
        isStunned = stunned;
        if (stunTime > 0)
        {
            StartCoroutine(StunCoroutine(stunTime));
        }

        // 움직임 제어
        if (TryGetComponent<Enemy>(out var enemy))
        {
            enemy.isStunned = stunned;
        }
    }

    private IEnumerator StunCoroutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

}
