Shader "Hidden/Debug/ClusterGrid"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="5.0"}
        LOD 100

        Cull Front

        Pass
        {
            Name "ClusterGrid"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #pragma target 5.0

            #include "./ShaderLibrary/ClusterInput.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VERTEXID;
                //float2 uv : TEXCOORD0;
            };

            struct VertexOut
            {
                uint id : VERTEXID;
            };

            struct GeometryOutput
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            VertexOut vert (Attributes input)
            {
                VertexOut output;
                output.id = input.vertexID;
                return output;
            }

            [maxvertexcount(6)]
            void geom(point VertexOut input[1], inout LineStream<GeometryOutput> outStream)
            {
                float3 center = float3(input[0].id % 100 % 10, input[0].id % 100 / 10, input[0].id / 100);
                //center = 0;
                GeometryOutput output[4];
                output[0].vertex = TransformObjectToHClip(center + float3(-0.5, -0.5, 0));
                output[1].vertex = TransformObjectToHClip(center + float3( 0.5, -0.5, 0));
                output[2].vertex = TransformObjectToHClip(center + float3(-0.5,  0.5, 0));
                output[3].vertex = TransformObjectToHClip(center + float3( 0.5,  0.5, 0));
                output[0].uv = float3(0, 0, center.z * 0.1);
                output[1].uv = float3(1, 0, center.z * 0.1);
                output[2].uv = float3(0, 1, center.z * 0.1);
                output[3].uv = float3(1, 1, center.z * 0.1);

                outStream.Append(output[2]);
                outStream.Append(output[0]);
                outStream.Append(output[1]);
                outStream.Append(output[2]);
                outStream.Append(output[3]);
                outStream.Append(output[1]);
            }

            half4 frag (GeometryOutput input) : SV_Target
            {
                half4 finalColor = half4(input.uv.x, input.uv.y, input.uv.z, 1);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
