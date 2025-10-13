using System;
using UnityEngine;


[ExecuteInEditMode]
public class OutlineSprite : MonoBehaviour {
    public Color color = Color.white;
    private readonly Color stunColor = Color.yellow;
    private Color originalColor;
    private bool isStunned = false;

    [Range(0, 16)]
    public int outlineSize = 1;
    private int originalOutlineSize;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;

    // cache property IDs to avoid string lookups
    private static readonly int PropOutline = Shader.PropertyToID("_Outline");
    private static readonly int PropOutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int PropOutlineSize = Shader.PropertyToID("_OutlineSize");

    // last applied values to avoid redundant SetPropertyBlock calls
    private Color lastColor = new Color(float.NaN, 0, 0, 0);
    private int lastOutlineSize = -1;
    private bool lastOutlineEnabled = false;

    void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        if (mpb == null) mpb = new MaterialPropertyBlock();

        originalColor = color;
        originalOutlineSize = outlineSize;

        // Force initial update
        lastColor = new Color(float.NaN, 0, 0, 0);
        lastOutlineSize = -1;
        lastOutlineEnabled = !lastOutlineEnabled;
        UpdateOutline(true);
    }

    void OnDisable() {
        UpdateOutline(false);
    }

    void Update() {
        // Only update if something changed to avoid per-frame work and allocations
        bool outlineEnabled = outlineSize > 0;
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        if (!lastColor.Equals(color) || lastOutlineSize != outlineSize || lastOutlineEnabled != outlineEnabled)
        {
            UpdateOutline(outlineEnabled);
        }
    }

    void UpdateOutline(bool outline) {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
        }

        // ensure MPB is allocated and reuse it to avoid allocations
        if (mpb == null) mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);

        float outlineVal = outline ? 1f : 0f;
        mpb.SetFloat(PropOutline, outlineVal);
        mpb.SetColor(PropOutlineColor, color);
        mpb.SetFloat(PropOutlineSize, outline ? outlineSize : 0);

        spriteRenderer.SetPropertyBlock(mpb);

        // cache last applied values
        lastColor = color;
        lastOutlineSize = outlineSize;
        lastOutlineEnabled = outline;
    }

    public void SetColor(Color32 newColor)
    {
        color = newColor;
    }

    public void ActiveOutline(bool active) {
        outlineSize = active ? 1 : 0;
    }

    public void SetStunned(bool stunned) {
        isStunned = stunned;
        if (stunned) {
            color = stunColor;
            outlineSize = 1;
        } else {
            color = originalColor;
            outlineSize = 0;
        }
        UpdateOutline(true);
    }
}