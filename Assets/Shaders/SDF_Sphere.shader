Shader "Unlit/SDF_Sphere"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)

        _Radius("Radius", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="4.5"}
        LOD 100

        Pass
        {
            Name "Sphere"

            HLSLPROGRAM
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START (UnityPerMaterial)
                half4 _BaseColor;
                float _Radius;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                //float2 texCoord   : TEXCOORD0;
            };

            struct VertexOut
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                //float2 uv     : TEXCOORD0;
            };

            VertexOut vert (Attributes input)
            {
                VertexOut output = (VertexOut)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            float sdSphere(float3 p, float s)
            {
                return length(p) - s;
            }

            // Taken from https://iquilezles.org/www/articles/distfunctions/distfunctions.htm
            float sdSmoothUnion(float d1, float d2, float k) 
            {
                float h = saturate(0.5 + 0.5 * (d2 - d1) / k);
                return lerp(d2, d1, h) - k * h * (1.0 - h); 
            }

            half4 frag (VertexOut input) : SV_Target
            {
                half4 finalColor = _BaseColor;
                float sdv = sdSphere(input.positionOS, _Radius);
                clip(-sdv);               

                return finalColor;
            }
            ENDHLSL
        }
    }
}
