Shader "EE/OceanSurface"
{
    Properties
    {
        _BaseColor ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeLine" = "UniversalRenderPipeline" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
        CBUFFER_END

        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Unity Keywords
            #pragma multi_compile_fog

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                
                #ifdef _ADDITIONAL_LIGHTS_VERTEX
                    half4 fogFactorAndVertexLight : TEXCOORD6;
                #else
                    half fogFactor : TEXCOORD6;
                #endif
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                
                // Fog
                half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                #ifdef _ADDITIONAL_LIGHTS_VERTEX
                    OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
                #else
                    OUT.fogFactor = fogFactor;
                #endif

                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = _BaseColor * IN.color;

                InputData inputData = (InputData)0;

                // Fog
                #ifdef _ADDITIONAL_LIGHTS_VERTEX
                    inputData.fogCoord = IN.fogFactorAndVertexLight.x;
                    inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
                #else
                    inputData.fogCoord = IN.fogFactor;
                    inputData.vertexLighting = half3(0, 0, 0);
                #endif

                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                return color;
            }
            ENDHLSL
        }
    }
}