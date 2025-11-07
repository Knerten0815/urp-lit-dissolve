Shader "Custom/SimpleDissolve"
{
    Properties
    {
        [Header(Base Settings)] [Space]
        _BaseColor("Base Color", Color)                     = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D)                    = "white" {}
        [Header(Dissolve Settings)] [Space]
        _DissolveColor("Color", Color)             = (0, 1, 1)
        _DissolveEmission("Emission", float)       = 100
        _DissolveRadius("Radius", float)           = 0.5
        _DissolveArea("Area", float)               = 0.1
        _DissolveOrigin("Origin", Vector)          = (0,0,0,0)
        _NoiseTexture("Noise Texture", 2D)         = "black" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "AlphaTest"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseTexture_ST;
            CBUFFER_END

            // unity macros
            TEXTURE2D(_BaseTexture);
            SAMPLER(sampler_BaseTexture);

            #include "Assets/Shaders/Dissolve/src/NoiseDissolve.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);

                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return ApplyNoiseDissolve(i.positionWS, i.uv);
            }

            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite on
            ColorMask R     // Restricts rendering to a ColorChannel. Depth Texture only uses red color channel

            HLSLPROGRAM

            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Shaders/Dissolve/src/BasicDissolve.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
            };

            v2f depthOnlyVert(appdata v)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);

                return o;
            }

            float depthOnlyFrag(v2f i) : SV_TARGET
            {
                ApplyDissolve(i.positionWS);

                return i.positionCS.z;
            }
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite on

            HLSLPROGRAM

            #pragma vertex depthNormalVert
            #pragma fragment depthNormalFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Shaders/Dissolve/src/BasicDissolve.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            v2f depthNormalVert(appdata v)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.normalWS = NormalizeNormalPerVertex(normalWS);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);

                return o;
            }

            float4 depthNormalFrag(v2f i) : SV_TARGET
            {
                ApplyDissolve(i.positionWS);

                float3 normalWS = NormalizeNormalPerPixel(i.normalWS);

                return float4(normalWS, 0.0f);
            }
            ENDHLSL
        }
    }
}