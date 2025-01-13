Shader "Sprites/SpriteShadow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 1
        [MaterialToggle] _WaveEffect ("Wave Effect", Float) = 0
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2
        _WaveAmount ("Wave Amount", Range(0, 1)) = 0.1
        [MaterialToggle] _GlitchEffect ("Glitch Effect", Float) = 0
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1
        _GlitchThickness ("Glitch Thickness", Range(0.01, 0.5)) = 0.02
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float _GrayscaleAmount;
            float _WaveEffect;
            float _WaveSpeed;
            float _WaveAmount;
            float _GlitchEffect;
            float _GlitchIntensity;
            float _GlitchThickness;

            // 간단한 랜덤 함수
            float random(float2 st) {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                
                // 웨이브 효과 계산
                if (_WaveEffect > 0)
                {
                    float wave = sin(_Time.y * _WaveSpeed + uv.y * 10) * _WaveAmount;
                    uv.x += wave;
                }

                // 글리치 효과 계산
                if (_GlitchEffect > 0)
                {
                    float time = floor(_Time.y * 10) / 10;
                    float randomValue = random(float2(time, 2));
                    float glitchLine = step(1 - _GlitchThickness, random(float2(uv.y, time)));
                    float offset = (random(float2(uv.y, time)) - 0.5) * _GlitchIntensity;
                    
                    uv.x += offset * glitchLine * _GlitchEffect;
                }
                
                // 텍스처 샘플링
                fixed4 c = SampleSpriteTexture(uv) * IN.color;
                
                // 그레이스케일 계산
                float gray = dot(c.rgb, float3(0.299, 0.587, 0.114));
                c.rgb = lerp(c.rgb, float3(gray, gray, gray), _GrayscaleAmount);
                
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}