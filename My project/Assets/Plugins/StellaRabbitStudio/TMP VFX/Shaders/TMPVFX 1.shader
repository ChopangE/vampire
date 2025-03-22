Shader "TextMeshPro/TMPVFX1" {

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

	// VFX Drawer properties
	_EditorDrawers("Editor Drawers", Float) = 1

	[Header(Screen Noise Settings)]
	[Toggle] _PAPER_NOISEEnabled("Paper Noise On/Off", Float) = 0
	[Toggle] _UseOriginalTextureColor ("Use Original Texture Color", Float) = 0
	[NoScaleOffset] _PaperNoiseTexture("Paper Noise Texture", 2D) = "white" {}
	_NoiseScale("Noise Scale", Float) = 2
	[Enum(UV Space,0,Screen Space,1)] _NoiseUVSource("Noise UV Source", Float) = 0
	_NoiseOffset("Noise Offset", Range(0, 1)) = 0
	[IntRange] _NoiseFramerate("Noise Framerate", Range(0, 120)) = 8

	[Toggle] _HANDDRAWNEnabled ("Hand Drawn On/Off", Float) = 0
	[Toggle] _HandDrawnRandomEnabled ("Use Random Offset", Float) = 0
	_HandDrawnFixedOffset ("Fixed Offset", Range(1, 10)) = 1
	_HandDrawnSpeed ("Speed", Range(1, 15)) = 5
	_HandDrawnAmount ("Amount", Range(1, 20)) = 3
	[IntRange] _HandDrawnFramerate ("Animation Framerate", Range(1, 120)) = 8
	_HandDrawnFrequency ("Hand Drawn Frequency", Range(1, 20)) = 4

	[Toggle] _BLACKHOLEEnabled ("Black Hole On/Off", Float) = 0
	_BlackHoleAmount ("Amount", Range(1, 20)) = 10
	_BlackHoleSpeed ("Speed", Range(1, 15)) = 5
	_BlackHoleThickness ("Line Thickness", Range(1, 10)) = 1
	[HDR] _BlackHoleColor ("Line Color", Color) = (0, 0, 0, 1)

	[Toggle] _SKETCHOUTLINEEnabled ("Sketch Outline On/Off", Float) = 0
	[Enum(Sketch,0, Shadow,1, MixText, 2)] _SketchOutlineType ("Sketch Outline Type", Float) = 0
	_SketchOutlineColor ("Sketch Outline Color", Color) = (0, 0, 0, 1)
	[NoScaleOffset] _SketchOutlineTexture ("Sketch Outline Texture", 2D) = "white" {}
	_SketchOutlineScale ("Sketch Outline Scale", Range(1, 20)) = 10
	_SketchOutlineStrength ("Sketch Outline Strength", Range(0, 100)) = 1
	_SketchOutlineThickness ("Sketch Outline Thickness", Range(0, 10)) = 1
	_SketchOutlineDirection ("Sketch Outline Direction", Range(0, 360)) = 45

	[Toggle] _TEXTBOXEnabled ("Text Box On/Off", Float) = 0
	[Enum(Letter,0, Square,1, polygon,2)] _TextBoxType ("Text Box Type", Float) = 0
	_TextBoxOffsetX ("Text Box Offset X", Range(-1, 1)) = 0
	_TextBoxOffsetY ("Text Box Offset Y", Range(-1, 1)) = 0
	_TextBoxWidth ("Text Box Width", Range(0, 10)) = 1
	_TextBoxHeight ("Text Box Height", Range(0, 10)) = 1
	[Toggle] _TextBoxRandomEnabled ("Text Box Random", Float) = 0
	[Toggle] _UseTextBoxTexture ("Use Text Box Texture", Float) = 0
	[NoScaleOffset] _TextBoxTexture ("Text Box Texture", 2D) = "white" {}
	_TextBoxTextureScale ("Texture Scale", Range(0.1, 10)) = 1
	_TextBoxColor ("Text Box Color", Color) = (1, 1, 1, 1)
	_PolygonSlope ("Polygon Slope", Range(0, 1)) = 0.3
	[IntRange] _TextBoxFramerate ("Animation Framerate", Range(1, 120)) = 8

	[Toggle] _SCRIBBLENOISEEnabled ("Scribble Effect On/Off", Float) = 0
	_ScribbleSpeed ("Scribble Speed", Range(0, 10)) = 1
	_ScribbleAmplitude ("Scribble Amplitude", Range(0, 0.1)) = 0.005
	_ScribbleFrequency ("Scribble Frequency", Range(0, 1000)) = 10
	_ScribbleThickness ("Scribble Thickness", Range(0, 1)) = 0.5
	_ScribbleFramerate ("Scribble Framerate", Range(0, 120)) = 8

	[Toggle] _DOODLEEnabled ("Doodle Effect On/Off", Float) = 0
	[Enum(Line,0, Circle,1)] _DoodleType ("Doodle Type", Float) = 0
	_DoodleSpeed ("Doodle Speed", Range(0, 10)) = 1
	_DoodleIntensity ("Doodle Intensity", Range(0, 10)) = 1
	_DoodleScale ("Doodle Scale", Range(0.1, 100)) = 1
	[IntRange] _DoodleFramerate ("Animation Framerate", Range(1, 120)) = 8
	[HDR] _DoodleColor ("Doodle Color", Color) = (1, 1, 1, 1)

	[Header(Distortion Effect)]
	[Toggle] _DISTORTIONEnabled ("Distortion On/Off", Float) = 0
	_DistortionAmount ("Distortion Amount", Range(0, 0.1)) = 0.1
	_DistortionSpeed ("Distortion Speed", Range(0, 10)) = 1
	_DistortionFrequency ("Distortion Frequency", Range(0, 50)) = 10
	_DistortionNoiseScale ("Noise Scale", Range(0, 10)) = 1
	[IntRange] _DistortionFramerate ("Animation Framerate", Range(0, 120)) = 8

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
		#pragma shader_feature __ OUTLINE_ON

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

		float _PAPER_NOISEEnabled;
		float _UseOriginalTextureColor;
		sampler2D _PaperNoiseTexture;
		float _NoiseScale;
		float _NoiseUVSource;
		float _NoiseOffset;
		float _NoiseFramerate;

		float _HANDDRAWNEnabled;
		float _HandDrawnSpeed;
		float _HandDrawnAmount;
		float _HandDrawnRandomEnabled;
		float _HandDrawnFixedOffset;
		float _HandDrawnFramerate;
		float _HandDrawnFrequency;

		float _BLACKHOLEEnabled;
		float _BlackHoleAmount;
		float _BlackHoleSpeed;
		float _BlackHoleThickness;
		float4 _BlackHoleColor;

		float _SKETCHOUTLINEEnabled;
		float _SketchOutlineType;
		float4 _SketchOutlineColor;
		sampler2D _SketchOutlineTexture;
		float _SketchOutlineScale;
		float _SketchOutlineStrength;
		float _SketchOutlineThickness;
		float _SketchOutlineDirection;

		float _TEXTBOXEnabled;
		float _TextBoxType;
		float _TextBoxRandomEnabled;
		float4 _TextBoxColor;
		float _TextBoxOffsetX;
		float _TextBoxOffsetY;
		float _TextBoxWidth;
		float _TextBoxHeight;
		float _PolygonSlope;
		sampler2D _TextBoxTexture;
		float _UseTextBoxTexture;
		float _TextBoxTextureScale;
		float _TextBoxFramerate;

		float2 _CharacterCenter;

		float _SCRIBBLENOISEEnabled;
		float _ScribbleSpeed;
		float _ScribbleAmplitude;
		float _ScribbleFrequency;
		float _ScribbleThickness;
		float _ScribbleFramerate;

		float _DOODLEEnabled;
		float _DoodleSpeed;
		float _DoodleIntensity;
		float _DoodleScale;
		float _DoodleFramerate;
		float4 _DoodleColor;
		float _DoodleType;

		float _DISTORTIONEnabled;
		float _DistortionAmount;
		float _DistortionSpeed;
		float _DistortionFrequency;
		float _DistortionNoiseScale;
		float _DistortionFramerate;

		float GetScreenSpaceNoise(float4 faceColor, float2 screenUV, float time);

		float Random(float2 p);
		float Random2(float2 p, float scale);

		float GetTextBoxDistance(float2 uv, float2 size, int boxType, float c, float cornerSlope);

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
			float c = tex2D(_MainTex, centeredUV).a;

			float4 textBoxColorWithAlpha = float4(0, 0, 0, 0);
			float textBoxOutline = 0;

			if (_HANDDRAWNEnabled > 0) {
				half2 uvCopy = centeredUV; 
				float offset;
				
				// 프레임레이트에 따른 시간 계산
				float frameTime;
				if (_HandDrawnFramerate > 0) {
					frameTime = floor(_Time.y * _HandDrawnFramerate) / _HandDrawnFramerate;
				} else {
					frameTime = _Time.y;
				}
				
				if (_HandDrawnRandomEnabled > 0) {
					// 랜덤 오프셋 사용
					offset = Random(floor(frameTime * _HandDrawnSpeed) * float2(12.9898, 78.233));
				} else {
					// 고정된 오프셋 사용
					offset = _HandDrawnFixedOffset;
				}
					       
				_HandDrawnSpeed = (floor(frameTime * _HandDrawnSpeed) / _HandDrawnSpeed) * _HandDrawnSpeed;
				
				float angle = (uvCopy.x * _HandDrawnAmount + _HandDrawnSpeed + offset);
				uvCopy.x = sin(angle * _HandDrawnFrequency);
				
				angle = (uvCopy.y * _HandDrawnAmount + _HandDrawnSpeed + offset);
				uvCopy.y = cos(angle * _HandDrawnFrequency);
					
				centeredUV = lerp(centeredUV, centeredUV + uvCopy, 0.0005 * _HandDrawnAmount);
					
				c = tex2D(_MainTex, centeredUV).a;
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
			if (_SCRIBBLENOISEEnabled > 0) {
				float2 uv = input.originalUV;
				
				// 프레임이트 용
				float frameTime;
				if (_ScribbleFramerate > 0) {
					frameTime = floor(_Time.y * _ScribbleFramerate) / _ScribbleFramerate;
				} else {
					frameTime = _Time.y;
				}
				float time = frameTime * _ScribbleSpeed;
				
				// UV 표 변환
				uv -= 0.5;
				
				// 각 선마다 다른 불규칙한 왜곡 생성
				float lineIndex = floor(uv.x * _ScribbleFrequency + time);
				float lineNoise = Random2(float2(lineIndex, frameTime), 1.0);
				float distortion = sin(uv.y * 10.0 + lineNoise * 10.0) * _ScribbleAmplitude;
				
				// 왜곡된 UV 계산
				float2 stretchedUV = float2(
					(uv.x + distortion) * _ScribbleFrequency + time,
					uv.y * 0.2
				);
				
				// 세로로 길쭉한 패턴 생성
				float verticalStretch = smoothstep(0.0, 0.2, abs(uv.y));
				float pattern = step(Random2(floor(stretchedUV * 10), 1.0) * verticalStretch, _ScribbleThickness);
				
				// 수직선 효과와 결합
				float verticalLines = step(frac(stretchedUV.x), _ScribbleThickness);
				
				// 최종 색상에 적용
				faceColor *= pattern;
			}
			faceColor.rgb *= input.color.rgb;

			faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
			outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

			if (_DISTORTIONEnabled > 0) {
				float2 uv = input.originalUV;
				
				// 프레임레이트 적용
				float frameTime;
				if (_DistortionFramerate > 0) {
					frameTime = floor(_Time.y * _DistortionFramerate) / _DistortionFramerate;
				} else {
					frameTime = _Time.y;
				}
				
				// 노이즈 기반 왜곡 계산
				float2 noiseUV = uv * _DistortionNoiseScale;
				float noise1 = sin(noiseUV.x * _DistortionFrequency + frameTime * _DistortionSpeed) * _DistortionAmount * 0.1;
				float noise2 = cos(noiseUV.y * _DistortionFrequency + frameTime * _DistortionSpeed) * _DistortionAmount * 0.1;
				
				// UV 왜곡을 모든 텍스처 샘플링에 적용
				float2 distortedUV = input.atlas + float2(noise1, noise2);
				c = tex2D(_MainTex, distortedUV).a;
				
				// face 텍스처에도 왜곡 적용
				float2 distortedFaceUV = input.textures.xy + float2(noise1, noise2) + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y;
				faceColor *= tex2D(_FaceTex, distortedFaceUV);
				
				// outline 텍스처에도 왜곡 적용
				float2 distortedOutlineUV = input.textures.zw + float2(noise1, noise2) + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y;
				outlineColor *= tex2D(_OutlineTex, distortedOutlineUV);
				
				// sd(signed distance) 재계산
				sd = (bias - c) * scale;
			}

			// 이후 faceColor 계산
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
			
			if (_PAPER_NOISEEnabled == 1)
			{
				float2 screenUV = input.position.xy / _ScreenParams.xy;
				if (_NoiseUVSource == 0)
				{
					screenUV = input.atlas.xy;
				}
				
				float noise = GetScreenSpaceNoise(faceColor, screenUV, _Time.y);
				if (_UseOriginalTextureColor == 1)
				{
					float4 paperColor = tex2D(_PaperNoiseTexture, screenUV * _NoiseScale);
					faceColor.rgb *= paperColor.rgb * noise;
				}
				else
				{
					faceColor.rgb *= noise;
				}
			}

			if (_BLACKHOLEEnabled > 0) {
				float2 uv = input.originalUV;
				float time = _Time.y * _BlackHoleSpeed;
				
				// Create wobble effect
				float xWobble = sin(uv.y * _BlackHoleAmount + time) * 0.003 * _BlackHoleThickness;
				float yWobble = sin(uv.x * _BlackHoleAmount + time) * 0.003 * _BlackHoleThickness;
				
				// Sample colors with offset
				float4 xOffset = tex2D(_MainTex, uv + float2(xWobble, 0));
				float4 yOffset = tex2D(_MainTex, uv + float2(0, yWobble));
				
				// Create hand drawn effect
				float outline = saturate(
					abs(faceColor.a - xOffset.a) +
					abs(faceColor.a - yOffset.a)
				);
				
				// Apply effect
				faceColor.rgb *= lerp(faceColor.rgb, _BlackHoleColor.rgb * _BlackHoleColor.a, outline);
			}


			if (_TEXTBOXEnabled > 0) {
				float2 boxUV = input.originalUV - _CharacterCenter - float2(0.5 + _TextBoxOffsetX, 0.5 + _TextBoxOffsetY);
				
				// Random offset
				if (_TextBoxRandomEnabled > 0) {
					float frameTime = floor(_Time.y * _TextBoxFramerate);
					
					boxUV += (float2(
						Random(frameTime * float2(12.9898, 78.233)),
						Random(frameTime * float2(78.233, 12.9898))
					) - 0.5) * 0.02;
				}
				
				float boxDist = GetTextBoxDistance(boxUV, float2(_TextBoxWidth, _TextBoxHeight), _TextBoxType, c, _PolygonSlope);
				float boxMask = 1 - step(0, boxDist);
				
				float4 boxColor = _TextBoxColor;
				if (_UseTextBoxTexture > 0) {
					float frameTime = floor(_Time.y * _TextBoxFramerate) / _TextBoxFramerate;
					float2 textureUV = frac((input.originalUV - 0.5) * _TextBoxTextureScale + 0.5 + 
						float2(Random(float2(frameTime, 0)), Random(float2(0, frameTime))) * 0.1);
					boxColor *= tex2D(_TextBoxTexture, textureUV);
				}
				
				boxColor *= boxMask;
				
				faceColor.rgb = boxColor.rgb * (1 - faceColor.a) + faceColor.rgb * faceColor.a;
				faceColor.a = max(boxColor.a, faceColor.a);
			}

			if(_SKETCHOUTLINEEnabled > 0) {
				float outline = 0;
				
				if (_SketchOutlineType == 1 || _SketchOutlineType == 2) { // Shadow 타입 또는 MixText 타입
					float textAlpha = c;
					// 방향 각도를 라디안으로 변환
					float dirRad = _SketchOutlineDirection * 3.14159 / 180.0;
					float2 dirVector = float2(cos(dirRad), sin(dirRad));
					
					float2 texelSize = float2(1.0/_TextureWidth, 1.0/_TextureHeight);
					
					// 그림자 생성
					float mainShadow = 0;
					float2 mainOffset = dirVector * _SketchOutlineThickness * texelSize * 2.0;
					float2 mainSampleUV = input.originalUV + mainOffset;
					mainShadow = tex2D(_MainTex, mainSampleUV).a;
					mainShadow = saturate(mainShadow - textAlpha);
					
					// 진 엣지 추가
					float edgeShadow = 0;
					float2 edgeOffset = dirVector * texelSize * _SketchOutlineThickness * 0.5;
					float edgeAlpha = tex2D(_MainTex, input.originalUV + edgeOffset).a;
					edgeShadow = saturate(edgeAlpha - textAlpha) * 2.0;
					
					// 그림자 합성
					outline = max(mainShadow * 0.8, edgeShadow);
					outline = pow(outline, 0.7) * _SketchOutlineStrength;
					
					// 매우 미세한 노이즈
					float2 noiseUV = input.originalUV * 50.0;
					float noise = frac(sin(dot(noiseUV, float2(12.9898, 78.233))) * 43758.5453);
					outline *= (0.9 + noise * 0.1);
				}
				else { // Sketch 타입
					float2 texelSize = float2(1.0/_TextureWidth, 1.0/_TextureHeight);
					
					// 방향 각도를 라디안으로 변환
					float dirRad = _SketchOutlineDirection * 3.14159 / 180.0;
					float2 dirVector = float2(cos(dirRad), sin(dirRad));
					
					float2 offset = dirVector * texelSize * _SketchOutlineThickness;
					
					float sketchAlpha = step(0.5, tex2D(_MainTex, input.originalUV + offset).a);
					float2 sketchUV = input.originalUV * _SketchOutlineScale;
					float sketchTexture = step(0.5, tex2D(_SketchOutlineTexture, sketchUV).r);
					
					// 추가적 선을 그려서 마감
					float2 endOffset = dirVector * texelSize * (_SketchOutlineThickness * 0.5);
					float endAlpha = step(0.5, tex2D(_MainTex, input.originalUV + endOffset).a);
					
					outline = (sketchAlpha + endAlpha) * sketchTexture * _SketchOutlineStrength;
				}
				
				float4 outlineColor = float4(_SketchOutlineColor.rgb, outline);
				if (_SketchOutlineType == 2) { // MixText 타입
					// 글자 표면에 효과 적용
					faceColor.rgb = lerp(faceColor.rgb, outlineColor.rgb, outline * faceColor.a);
				} else {
					// 기존 Shadow와 Sketch 타입의 합성 식 유지
					faceColor.rgb = outlineColor.rgb * outline * (1 - faceColor.a) + faceColor.rgb * faceColor.a;
					faceColor.a = max(outline * (1 - faceColor.a), faceColor.a);
				}
			}

			if (_DOODLEEnabled > 0) {
				float2 uv = input.originalUV;
				
				// 프레임레이트 적용
				float frameTime;
				if (_DoodleFramerate > 0) {
					frameTime = floor(_Time.y * _DoodleFramerate) / _DoodleFramerate;
				} else {
					frameTime = _Time.y;
				}
				
				// outline 영역 제외하고 face 부분에만 효과 적용
				float sd = (bias - c) * scale;
				float outline_area = 1 - saturate((sd + outline * 0.5) / (outline * 0.5));
				
				if (_DoodleType == 0) { // Line type
					// 기존 낙서 효과
					float2 noiseUV = uv * _DoodleScale;
					float noise1 = sin(noiseUV.x * 10 + noiseUV.y * 8 + frameTime * _DoodleSpeed) * 0.5 + 0.5;
					float noise2 = cos(noiseUV.y * 12 + noiseUV.x * 7 + frameTime * _DoodleSpeed * 1.2) * 0.5 + 0.5;
					
					float blendFactor = noise1 * noise2 * _DoodleIntensity * faceColor.a * outline_area;
					float3 doodleColor = faceColor.rgb * (1 - blendFactor) + _DoodleColor.rgb * blendFactor;
					
					faceColor.rgb = doodleColor;
				}
				else { // Circle type
					// 원형 낙서 효과 구현
					float2 noiseUV = uv * _DoodleScale;
					
					// 여러 개의 원을 생성하기 위한 격자 좌표 계산
					float2 gridPos = floor(noiseUV * 3.0);
					float2 gridUV = frac(noiseUV * 3.0) - 0.5;
					
					// 각 격자 위치에 대한 랜덤 값 생성
					float randomOffset = Random2(gridPos, 1.0);
					float randomSize = Random2(gridPos + 1.234, 1.0) * 0.4 + 0.1;
					float randomSpeed = Random2(gridPos + 5.678, 1.0);
					
					// 시간에 따른 원의 크기 변화
					float t = frameTime * _DoodleSpeed * (0.5 + randomSpeed);
					float size = randomSize * (0.8 + 0.2 * sin(t + randomOffset * 6.283));
					
					// 원형 패턴 생성
					float circle = length(gridUV);
					float pattern = smoothstep(size, size - 0.1, circle);
					
					// 전체 효과의 강도 조절
					float intensity = pattern * _DoodleIntensity * faceColor.a * outline_area;
					
					// 최종 색상 블렌딩
					float3 doodleColor = lerp(faceColor.rgb, _DoodleColor.rgb, intensity);
					faceColor.rgb = doodleColor;
				}
			}


			return faceColor * input.color.a;
		}

		float GetScreenSpaceNoise(float4 faceColor, float2 screenUV, float time)
		{
			float2 scaledUV = screenUV * _NoiseScale;
			
			if(_NoiseFramerate > 0)
			{
				float frameTime = floor(time * _NoiseFramerate) / _NoiseFramerate;
				scaledUV += frameTime;
			}
			
			if (_UseOriginalTextureColor == 1)
			{
				float4 paperColor = tex2D(_PaperNoiseTexture, scaledUV);
				return saturate(length(paperColor.rgb) / 1.732051 + _NoiseOffset);
			}
			else
			{
				float noise = tex2D(_PaperNoiseTexture, scaledUV).r;
				return saturate(noise + _NoiseOffset);
			}
		}

		float Random(float2 p)
		{
			return frac(sin(dot(p, float2(12.9898f, 78.233f))) * 43758.5453f);
		}

		float Random2(float2 p, float scale)
		{
			p = p * scale;
			return frac(sin(dot(p, float2(12.9898f, 78.233f))) * 43758.5453f);
		}
		float GetTextBoxDistance(float2 uv, float2 size, int boxType, float c, float cornerSlope)
		{
			float2 absUV = abs(uv);
			float2 halfSize = size * 0.5;
			
			if (boxType == 0) { // Letter
				float2 bounds = float2(size.x * c, size.y) * 0.5;
				float2 d = absUV - bounds;
				return length(max(d, 0)) + min(max(d.x, d.y), 0);
			}
			else if (boxType == 1) { // Square
				float2 d = absUV - halfSize;
				return length(max(d, 0)) + min(max(d.x, d.y), 0);
			}
			else { // Polygon
				float2 d = absUV - halfSize;
				
				// 경사도를 적용한 사각형 거리 계산
				float slopedY = d.y + abs(d.x) * cornerSlope;
				float base = max(d.x, slopedY);
				
				// Hand-drawn 효과를 위한 노이즈 생성
				float noise = sin(absUV.x * 50 + absUV.y * 30) * 0.02 
					       + cos(absUV.y * 40 + absUV.x * 20) * 0.02;
				
				// 테두리 주변에만 노이즈 적용
				float distFromBorder = abs(base);
				float noiseWeight = saturate(1 - distFromBorder * 10);
				
				return base + noise * noiseWeight;
			}
		}

		ENDCG
	}
}

Fallback "TextMeshPro/Mobile/Distance Field"
CustomEditor "StellaRabbitStudio.TMPVFXDrawer"
}
