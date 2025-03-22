Shader "TextMeshPro/TMPVFX" {

Properties {
	_FaceTex			("Face Texture", 2D) = "white" {}
	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
	_FaceColor		    ("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	_OutlineColor	    ("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
	_OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_Bevel				("Bevel", Range(0,1)) = 0.5
	_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
	_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
	_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
	_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0

	_LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
	_SpecularColor	    ("Specular", Color) = (1,1,1,1)
	_SpecularPower		("Specular", Range(0,4)) = 2.0
	_Reflectivity		("Reflectivity", Range(5.0,15.0)) = 10
	_Diffuse			("Diffuse", Range(0,1)) = 0.5
	_Ambient			("Ambient", Range(1,0)) = 0.5

	_BumpMap 			("Normal map", 2D) = "bump" {}
	_BumpOutline		("Bump Outline", Range(0,1)) = 0
	_BumpFace			("Bump Face", Range(0,1)) = 0

	_ReflectFaceColor	("Reflection Color", Color) = (0,0,0,1)
	_ReflectOutlineColor("Reflection Color", Color) = (0,0,0,1)
	_Cube 				("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
	_EnvMatrixRotation	("Texture Rotation", vector) = (0, 0, 0, 0)

	_UnderlayColor	    ("Border Color", Color) = (0,0,0, 0.5)
	_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness	("Border Softness", Range(0,1)) = 0

	_GlowColor		    ("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_MaskCoord			("Mask Coordinates", vector) = (0, 0, 32767, 32767)
	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15

	// VFX Color Settings
	// CRT
	_ScanlineOnOff ("Scanline On/Off", Int) = 0
	[Toggle] _MonochromeOnOff ("Monochrome On/Off", Float) = 0
	[Toggle] _FlickeringOnOff ("Flickering On/Off", Float) = 0
	_FlickeringStrength ("Flickering Strength", Range(0, 0.01)) = 0.003
	_FlickeringCycle ("Flickering Cycle", Range(0, 10)) = 1.0
	
	[Toggle] _SlippageOnOff ("Slippage On/Off", Float) = 0
	[Toggle] _SlippageNoiseOnOff ("Slippage Noise On/Off", Float) = 0
	_SlippageStrength ("Slippage Strength", Range(0, 0.1)) = 0.1
	_SlippageInterval ("Slippage Interval", Range(0, 10)) = 1.0
	_SlippageScrollSpeed ("Slippage Scroll Speed", Range(0, 10)) = 1.0
	_SlippageSize ("Slippage Size", Range(0, 1)) = 0.1

	[Toggle] _OldChromaticAberrationOnOff ("Old Chromatic Aberration On/Off", Float) = 0
	_OldChromaticAberrationStrength ("Old Chromatic Aberration Strength", Range(0, 1)) = 0.005
	[Toggle] _ChromaticAberrationOnOff ("Chromatic Aberration On/Off", Float) = 0
	_ChromaticAberrationStrength ("Chromatic Aberration Strength", Range(0, 0.1)) = 0.003
	[Toggle] _MultipleGhostOnOff ("Multiple Ghost On/Off", Float) = 0
	_MultipleGhostStrength ("Multiple Ghost Strength", Range(0, 1)) = 0.1
	_GhostColor ("Ghost Color", Color) = (1,1,1,1)
	_MainTextWeight ("Main Text Weight", Range(0, 1)) = 0.8
	_Ghost1Weight ("Ghost 1 Weight", Range(0, 1)) = 0.15
	_Ghost2Weight ("Ghost 2 Weight", Range(0, 1)) = 0.05

	[Toggle] _WhiteNoiseOnOff ("White Noise On/Off", Float) = 0
	_WhiteNoiseStrength ("White Noise Strength", Range(0, 1)) = 1

	_RGBSplitLeftColor ("RGB Split Left Color", Color) = (1,0,1,1)
	_RGBSplitRightColor ("RGB Split Right Color", Color) = (0,1,1,1)
	_RGBSplitOffset ("RGB Split Offset", Float) = 0.002

	// VFX Effect properties
	[Toggle] _VerticalGlitchOnOff ("Vertical Glitch On/Off", Float) = 0
	_GlitchIntensity ("Glitch Intensity", Float) = 0.5
	_GlitchFrequency ("Glitch Frequency", Float) = 1
	_LagAmount ("Lag Amount", Range(0, 1)) = 0.1
	_GlitchExtent ("Glitch Extent", Float) = 0.1
	_GlitchSpeed ("Glitch Speed", Float) = 1.0

	// VFX Drawer properties
	_EditorDrawers("Editor Drawers", Float) = 1

	// VFX Toggle Properties 
	[Toggle(GLITCH_ON)] _GLITCHEnabled ("Glitch Enabled", Float) = 0
	[Toggle(CRT_ON)] _CRTEnabled ("CRT Enabled", Float) = 0
	[Toggle(RGBSPLIT_ON)] _RGBSPLITEnabled ("RGB Split Enabled", Float) = 0

}

SubShader {
	Tags
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ BEVEL_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
		#pragma shader_feature __ GLOW_ON
		#pragma shader_feature __ GLITCH_ON
		#pragma shader_feature __ CRT_ON
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ RGBSPLIT_ON


		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"
		#include "TMPro.cginc"

		struct vertex_t
		{
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4	position		: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float4	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};

		struct pixel_t
		{
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4	position		: SV_POSITION;
			fixed4	color			: COLOR;
			float2	atlas			: TEXCOORD0;
			float4	param			: TEXCOORD1;
			float4	mask			: TEXCOORD2;
			float3	viewDir			: TEXCOORD3;

		    #if (UNDERLAY_ON || UNDERLAY_INNER)
			float4	texcoord2		: TEXCOORD4;
			fixed4	underlayColor	: COLOR1;
		    #endif
			float4 textures			: TEXCOORD5;
			float2 originalUV       : TEXCOORD6;
		};

		// Used by Unity internally to handle Texture Tiling and Offset.
		float4 _FaceTex_ST;
		float4 _OutlineTex_ST;

		float _UIMaskSoftnessX;
		float _UIMaskSoftnessY;
		int _UIVertexColorAlwaysGammaSpace;

		float _VerticalGlitchOnOff;
		float _GlitchIntensity;
		float _GlitchFrequency;
		float _LagAmount;
		float _GlitchExtent;
		float _GlitchSpeed;

		int _ScanlineOnOff;
		int _MonochromeOnOff;
		float _FlickeringOnOff;
		float _FlickeringStrength;
		float _FlickeringCycle;
		float _SlippageOnOff;
		float _SlippageNoiseOnOff;
		float _SlippageStrength;
		float _SlippageInterval;
		float _SlippageScrollSpeed;
		float _SlippageSize;
		float _OldChromaticAberrationOnOff;
		float _OldChromaticAberrationStrength;
		float _ChromaticAberrationOnOff;
		float _ChromaticAberrationStrength;
		float _MultipleGhostOnOff;
		float _MultipleGhostStrength;
		float _WhiteNoiseOnOff;
		float _WhiteNoiseStrength;

		float4 _GhostColor;

		float _MainTextWeight;
		float _Ghost1Weight;
		float _Ghost2Weight;

		float2 _CharacterCenter;
		float4 _RGBSplitLeftColor;
		float4 _RGBSplitRightColor;
		float _RGBSplitOffset;

		float _GLITCHEnabled;
		float _CRTEnabled;
		float _RGBSPLITEnabled;

		// These Random functions are from kimyir's SciFi Noise Glitch shader.
		#define RM 39482.17593
		#define RD1 7.8671
		#define RD2 3.3419
		#define RD3 5.8912
		
		float GetRandom(float x);
		float Random11(float seed);

		float2 Random12(float seed);

		float noise(float2 p);
		float2 glitch(float2 uv, float time);
		float2 VerticalGlitch(pixel_t input);

		pixel_t VertShader(vertex_t input)
		{
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input,output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float bold = step(input.texcoord0.w, 0);

			float4 vert = input.position;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord0.w) * _GradientScale * (_Sharpness + 1);
			if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float bias =(.5 - weight) + (.5 / scale);

			float alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _OutlineSoftness * _ScaleRatioA);

		    #if GLOW_ON
			alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
		    #endif

			alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;

		    #if (UNDERLAY_ON || UNDERLAY_INNER)
			float4 underlayColor = _UnderlayColor;
			underlayColor.rgb *= underlayColor.a;

			float bScale = scale;
			bScale /= 1 + ((_UnderlaySoftness*_ScaleRatioC) * bScale);
			float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 bOffset = float2(x, y);
		    #endif

			// Generate UV for the Masking Texture
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Support for texture tiling and offset
			float2 textureUV = input.texcoord1;
			float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
			float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);

			output.position = vPosition;
			output.color = input.color;
			output.atlas =	input.texcoord0;
			output.param =	float4(alphaClip, scale, bias, weight);
			output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));
			output.viewDir =	mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz);
			#if (UNDERLAY_ON || UNDERLAY_INNER)
			output.texcoord2 = float4(input.texcoord0 + bOffset, bScale, bBias);
			output.underlayColor =	underlayColor;
			#endif
			output.textures = float4(faceUV, outlineUV);
			output.originalUV = input.texcoord0.xy;
			_CharacterCenter = (input.texcoord0.xy + input.texcoord1.xy) * 0.5;

			return output;
		}

		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);

			float2 centeredUV = input.originalUV;

			// Replace keyword checks with property checks
			if (_GLITCHEnabled == 1) {
				if(_VerticalGlitchOnOff == 0) {
					float2 glitchedUV = glitch(centeredUV, _Time.y);
					centeredUV = glitchedUV;
				} else {
					float2 glitchedUV = VerticalGlitch(input);
					centeredUV = glitchedUV;
				}
			}

			if (_CRTEnabled == 1) {
				// White Noise (글자 모양대로 노이즈 추가)
				if(_WhiteNoiseOnOff == 1) {
					float originalAlpha = tex2D(_MainTex, input.originalUV).a;
					float n = noise(input.originalUV + _Time.y);
					
					n = lerp(1.0, n, _WhiteNoiseStrength);
					return float4(n, n, n, 1) * originalAlpha * _FaceColor;
				}

				// Flickering effect
				if(_FlickeringOnOff == 1) {
					float flickeringNoise = GetRandom(_Time.y);
					float flickeringMask = pow(abs(sin(input.originalUV.y * _FlickeringCycle + _Time.y * 2.0)), 2);
					float flickerAmount = flickeringNoise * flickeringMask * _FlickeringStrength;
					centeredUV.x = centeredUV.x + flickerAmount;
				}

		    	// Slippage effect
				if(_SlippageOnOff == 1) {
					float scrollSpeed = _Time.x * _SlippageScrollSpeed;
					float slippageMask = pow(abs(sin(input.originalUV.y * _SlippageInterval + scrollSpeed)), _SlippageSize);
					float stepMask = round(sin(input.originalUV.y * _SlippageInterval + scrollSpeed - 1));

					centeredUV.x = centeredUV.x + (_SlippageNoiseOnOff * _SlippageStrength * slippageMask * stepMask) * _SlippageOnOff; 
				}
			}

			float c = tex2D(_MainTex, centeredUV).a;

			float shouldClip = 0;
			if (_OldChromaticAberrationOnOff == 1 || _ChromaticAberrationOnOff == 1 || _MultipleGhostOnOff == 1
			|| _FlickeringOnOff == 1) 
			{
				shouldClip = 1;
			}
			#ifdef UNDERLAY_ON
				shouldClip = 0;
			#endif

			if (shouldClip)
			{
				clip(c - input.param.x);
			}

			float	scale	= input.param.y;
			float	bias	= input.param.z;
			float	weight	= input.param.w;
			float	sd = (bias - c) * scale;

			float outline = 0;
			float softness = 0;
			
			#if OUTLINE_ON
				outline = (_OutlineWidth * _ScaleRatioA) * scale;
				softness = (_OutlineSoftness * _ScaleRatioA) * scale;
			#endif

			half4 faceColor = _FaceColor;
			half4 outlineColor = _OutlineColor;

			faceColor.rgb *= input.color.rgb;

			faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
			outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

			faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

		    #if BEVEL_ON
			float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
			float3 n = GetSurfaceNormal(input.atlas, weight, dxy);

			float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
			bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
			n = normalize(n- bump);

			float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

			float3 col = GetSpecular(n, light);
			faceColor.rgb += col*faceColor.a;
			faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
			faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);

			fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
			faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
		    #endif

		    #if UNDERLAY_ON
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1 - faceColor.a);
		    #endif

		    #if UNDERLAY_INNER
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * (1 - saturate(d - input.texcoord2.w)) * saturate(1 - sd) * (1 - faceColor.a);
		    #endif

		    #if GLOW_ON
			float4 glowColor = GetGlowColor(sd, scale);
			faceColor.rgb += glowColor.rgb * glowColor.a;
		    #endif

			// Alternative implementation to UnityGet2DClipping with support for softness.
		    #if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			faceColor *= m.x * m.y;
		    #endif

		    #if UNITY_UI_ALPHACLIP
			clip(faceColor.a - 0.001);
		    #endif

			if (_CRTEnabled == 1) {
				float2 originUV = input.originalUV;

				// Chromatic Aberration
				if(_OldChromaticAberrationOnOff == 1) {
					float red = tex2D(_MainTex, float2(originUV.x - _OldChromaticAberrationStrength, originUV.y)).a;
					float green = tex2D(_MainTex, originUV).a;
					float blue = tex2D(_MainTex, float2(originUV.x + _OldChromaticAberrationStrength, originUV.y)).a;
					
					faceColor.r = lerp(faceColor.r, red * _FaceColor.r, _OldChromaticAberrationOnOff);
					faceColor.g = lerp(faceColor.g, green * _FaceColor.g, _OldChromaticAberrationOnOff);
					faceColor.b = lerp(faceColor.b, blue * _FaceColor.b, _OldChromaticAberrationOnOff);
				}
				if(_ChromaticAberrationOnOff == 1) {
					// Red channel - shifted right
					float4 r = tex2D(_MainTex, originUV + float2(_ChromaticAberrationStrength, 0));
					// Blue channel - shifted left
					float4 b = tex2D(_MainTex, originUV + float2(-_ChromaticAberrationStrength, 0));
					
					faceColor = float4(
					r.a + faceColor.r,           // Red
					faceColor.g,                  // Green (원본 유지)
					b.a + faceColor.b,           // Blue
					faceColor.a // Alpha
					);
				}

				// Multiple Ghost
				if(_MultipleGhostOnOff == 1) {
					float ghost1Alpha = tex2D(_MainTex, originUV - float2(_MultipleGhostStrength, 0)).a;
					float ghost2Alpha = tex2D(_MainTex, originUV - float2(_MultipleGhostStrength * 2, 0)).a;
					
					faceColor.rgb = faceColor.rgb * _MainTextWeight + 
								(_GhostColor.rgb * ghost1Alpha * _Ghost1Weight) +
								(_GhostColor.rgb * ghost2Alpha * _Ghost2Weight);
				}
			
				// Scanline effect
				float scanline = sin((input.originalUV.y + _Time.x) * 800.0) * 0.04;
				faceColor.rgb -= scanline * _ScanlineOnOff;

				// Monochrome effect
				if (_MonochromeOnOff == 1)
				{
					faceColor.rgb = 0.299f * faceColor.r + 0.587f * faceColor.g + 0.114f * faceColor.b;
				}
			}
			if (_RGBSPLITEnabled == 1) {
				// Original character's SDF value
				float originalC = c;

				// SDF value shifted to the left
				float leftC = tex2D(_MainTex, input.originalUV + float2(_RGBSplitOffset, 0)).a;

				// SDF value shifted to the right
				float rightC = tex2D(_MainTex, input.originalUV + float2(-_RGBSplitOffset, 0)).a;

				// Calculate color effects - Strictly control alpha values
				float4 leftColor = _RGBSplitLeftColor * step(0.5, leftC);
				float4 rightColor = _RGBSplitRightColor * step(0.5, rightC);

				// Final color blending - Changed to additive blending
				faceColor.rgb += (leftColor.rgb * leftColor.a + rightColor.rgb * rightColor.a) * (1 - faceColor.a);
				faceColor.a = max(max(leftColor.a, rightColor.a), faceColor.a);
			}
							
  		    return faceColor * input.color.a;
		}

		float GetRandom(float x)
		{
			return frac(sin(dot(x, float2(12.9898, 78.233))) * 43758.5453);
		}

		float Random11(float seed)
		{
			return frac(sin(dot(float2(RD1, seed), float2(seed, RD2))) * RM);
		}

		float2 Random12(float seed)
		{
		    return float2(
		        frac(sin(dot(float2(RD1, seed), float2(seed, RD2))) * RM),
		        frac(sin(dot(float2(seed, RD2), float2(RD3, seed))) * RM)
		    );
		}
		float noise(float2 p) {
			return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453123);
		}
		
		
		float2 glitch(float2 uv, float time) {
			float refreshTime = floor(time * _GlitchSpeed * 100);
			float2 p = floor(uv * _GlitchFrequency);
			float n = noise(p + refreshTime);
			
			float2 glitchOffset = float2(0, 0);
			if (n > 1.0 - _GlitchIntensity) {
				float glitchAmount = (n - (1.0 - _GlitchIntensity)) / _GlitchIntensity;
				glitchOffset.x = (noise(p + refreshTime * 1.3) - 0.5) * 2.0;
				glitchOffset.y = (noise(p + refreshTime * 1.7) - 0.5) * 2.0;
				
				glitchOffset *= _LagAmount * glitchAmount;
			}
			
			return uv + glitchOffset * _GlitchExtent;
		}
		float2 VerticalGlitch(pixel_t input) {
			float2 uvcoord = input.atlas;
			
			// 글리치 효과를 위한 새로운 코드
			float range = abs(sin(_Time.y * _GlitchSpeed * 1)) * _ScreenParams.y;
			float range2 = abs(sin(_Time.y * _GlitchSpeed * 2)) * _ScreenParams.y;
			float range3 = abs(sin(_Time.y * (_GlitchSpeed * 10) * 10)) * _ScreenParams.y;

			// _GlitchExtent로 글리치 영역 조절
			float glitchArea1 = 10 * _GlitchExtent;
			float glitchArea2 = 15 * _GlitchExtent;
			float glitchArea3 = 20 * _GlitchExtent;

			if ((input.position.y < range && input.position.y > range - glitchArea1))
			{
				uvcoord.x = uvcoord.x + (0.02 * _LagAmount);
			}
			if ((input.position.y < range2 - glitchArea1 && input.position.y > range2 - (glitchArea1 + glitchArea2)))
			{
				float2 squares = Random12(input.position.y);
				uvcoord.xy = uvcoord.xy + (squares * 0.05 * _LagAmount);
			}
			if ((input.position.y < range3 && input.position.y > range3 - glitchArea3))
			{
				uvcoord.x = uvcoord.x + (0.01 * _LagAmount);   
			}
			return uvcoord;
		}
		ENDCG
	}
}

Fallback "TextMeshPro/Mobile/Distance Field"
CustomEditor "StellaRabbitStudio.TMPVFXDrawer"
}
