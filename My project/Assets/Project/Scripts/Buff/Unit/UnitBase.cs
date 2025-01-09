using System.Collections;
using System.Collections.Generic;
using ModiBuff.Core;
using UnityEngine;

public class UnitBase : MonoBehaviour, IUnit
{
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
}