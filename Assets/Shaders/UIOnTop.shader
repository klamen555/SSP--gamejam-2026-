Shader "TextMeshPro/UIOnTop"
{
    Properties
    {
        _FaceColor ("Text Color", Color) = (1,1,1,1)
        _FaceDilate ("Face Dilate", Range(-1,1)) = 0

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,1)) = 0
        _OutlineSoftness ("Outline Softness", Range(0,1)) = 0

        _UnderlayColor ("Underlay Color", Color) = (0,0,0,0.5)
        _UnderlayOffsetX ("Underlay Offset X", Range(-1,1)) = 0
        _UnderlayOffsetY ("Underlay Offset Y", Range(-1,1)) = 0
        _UnderlayDilate ("Underlay Dilate", Range(-1,1)) = 0
        _UnderlaySoftness ("Underlay Softness", Range(0,1)) = 0

        _WeightNormal ("Weight Normal", Float) = 0
        _WeightBold ("Weight Bold", Float) = 0.5

        _ShaderFlags ("Flags", Float) = 0
        _ScaleRatioA ("Scale RatioA", Float) = 1

        _MainTex ("Font Atlas", 2D) = "white" {}
        _TextureWidth ("Texture Width", Float) = 512
        _TextureHeight ("Texture Height", Float) = 512
        _GradientScale ("Gradient Scale", Float) = 5
        _ScaleX ("Scale X", Float) = 1
        _ScaleY ("Scale Y", Float) = 1
        _PerspectiveFilter ("Perspective Correction", Range(0, 1)) = 0.875
        _Sharpness ("Sharpness", Range(-1,1)) = 0

        _VertexOffsetX ("Vertex Offset X", Float) = 0
        _VertexOffsetY ("Vertex Offset Y", Float) = 0

        _MaskSoftnessX ("Mask SoftnessX", Float) = 0
        _MaskSoftnessY ("Mask SoftnessY", Float) = 0
        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _CullMode ("Cull Mode", Float) = 0
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+100"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
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
        Lighting Off
        ZWrite Off
        ZTest Always // This ensures the "Always On Top" behavior
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "TMP_UIOnTop"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            // TMP Keywords
            #pragma shader_feature_local _OUTLINE_ON
            #pragma shader_feature_local _UNDERLAY_ON
            #pragma shader_feature_local _UNDERLAY_INNER
            #pragma shader_feature_local _MASK_HARD
            #pragma shader_feature_local _MASK_SOFT
            #pragma shader_feature_local _CLIP_RECT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float4 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 worldPosition : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // TMP Properties
            float4 _FaceColor;
            float _FaceDilate;
            
            float4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineSoftness;
            
            float4 _UnderlayColor;
            float _UnderlayOffsetX;
            float _UnderlayOffsetY;
            float _UnderlayDilate;
            float _UnderlaySoftness;
            
            float _WeightNormal;
            float _WeightBold;
            float _ShaderFlags;
            float _ScaleRatioA;
            
            float _TextureWidth;
            float _TextureHeight;
            float _GradientScale;
            float _ScaleX;
            float _ScaleY;
            float _PerspectiveFilter;
            float _Sharpness;
            
            float4 _ClipRect;
            float _MaskSoftnessX;
            float _MaskSoftnessY;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float2 UnpackUV(float uv)
            {
                float2 output;
                output.x = floor(uv) / 4096;
                output.y = uv - floor(uv) * 4096;
                return output * 255.0 / 16.0;
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.worldPosition = float4(TransformObjectToWorld(v.vertex.xyz), 1.0);
                o.vertex = TransformWorldToHClip(o.worldPosition.xyz);
                
                o.texcoord0.xy = TRANSFORM_TEX(v.texcoord0.xy, _MainTex);
                o.texcoord1 = UnpackUV(v.texcoord1.x);
                o.texcoord2 = UnpackUV(v.texcoord1.y);
                
                o.color = v.color * _FaceColor;
                
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float dist = tex2D(_MainTex, i.texcoord0.xy).a;
                
                float faceDilate = _FaceDilate * _ScaleRatioA;
                float outlineWidth = _OutlineWidth * _ScaleRatioA;
                float outlineSoftness = _OutlineSoftness * _ScaleRatioA;
                
                // Base Face Calculation
                float face = saturate((dist - (0.5 - faceDilate)) / max(0.0001, outlineSoftness + 0.002));
                
                float4 finalColor = i.color;
                
                #ifdef _OUTLINE_ON
                float outline = saturate((dist - (0.5 - faceDilate - outlineWidth)) / max(0.0001, outlineSoftness + 0.002));
                float4 outlineColor = _OutlineColor;
                outlineColor.rgb *= i.color.rgb;
                finalColor = lerp(outlineColor, i.color, face);
                finalColor.a = i.color.a * outline;
                #else
                finalColor.a *= face;
                #endif
                
                #if defined(_UNDERLAY_ON) || defined(_UNDERLAY_INNER)
                float2 underlayUV = i.texcoord0.xy - float2(_UnderlayOffsetX, _UnderlayOffsetY) * _GradientScale / _TextureWidth;
                float underlayDist = tex2D(_MainTex, underlayUV).a;
                float underlayFactor = saturate((underlayDist - (0.5 - _UnderlayDilate)) / max(0.0001, _UnderlaySoftness + 0.002));
                
                float4 underlayCol = _UnderlayColor;
                underlayCol.a *= underlayFactor;
                finalColor = lerp(underlayCol, finalColor, finalColor.a);
                #endif

                return finalColor;
            }
            ENDHLSL
        }
    }
    
    CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}