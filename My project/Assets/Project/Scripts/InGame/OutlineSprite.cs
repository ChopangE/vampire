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

    void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = color;
        originalOutlineSize = outlineSize;
        UpdateOutline(true);
    }

    void OnDisable() {
        UpdateOutline(false);
    }

    void Update() {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline) {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
        }

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
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