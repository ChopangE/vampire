using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Tooltip("Radius used for scanning")]
    public float scanRange;
    public LayerMask targetLayer;

    // Non-allocating buffer for OverlapCircleNonAlloc — tune maxTargets in the inspector.
    public Collider2D[] targets;
    public int maxTargets = 32;
    int targetCount = 0;

    // Last computed nearest target
    public Transform nearestTarget;

    void Awake()
    {
        if (targets == null || targets.Length == 0)
            targets = new Collider2D[Mathf.Max(1, maxTargets)];
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive) return;

        // Fill the preallocated array without allocating memory each frame.
        // Note: use LayerMask.value as the API expects an int mask.
        targetCount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position, scanRange, targets, targetLayer.value);

        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;

        // Use squared distances to avoid creating temporaries from sqrt calls.
        float minSqr = float.MaxValue;
        Vector3 myPos = transform.position;

        for (int i = 0; i < targetCount; i++)
        {
            var col = targets[i];
            if (col == null) continue;

            Transform t = col.transform;
            float curSqr = (t.position - myPos).sqrMagnitude;

            if (curSqr < minSqr)
            {
                minSqr = curSqr;
                result = t;
            }
        }

        return result;
    }

    public Transform GetNearstEliteOrBoss()
    {
        Transform result = null;
        float minSqr = float.MaxValue;
        Vector3 myPos = transform.position;

        for (int i = 0; i < targetCount; i++)
        {
            var col = targets[i];
            if (col == null) continue;

            Transform t = col.transform;

            // TryGetComponent with generics avoids allocation and is efficient.
            Boss boss;
            MiddleBoss middleBoss;
            if (t.TryGetComponent<Boss>(out boss) || t.TryGetComponent<MiddleBoss>(out middleBoss))
            {
                float curSqr = (t.position - myPos).sqrMagnitude;
                if (curSqr < minSqr)
                {
                    minSqr = curSqr;
                    result = t;
                }
            }
        }

        return result;
    }
}
