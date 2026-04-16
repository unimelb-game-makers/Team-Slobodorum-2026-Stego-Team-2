Shader "URP/BurnMarkShader"
{
    Properties
    {
        _BurnColor("Burn Color", Color) = (0.05, 0.05, 0.05, 1)
        [HDR] _EmberColor("Ember Color", Color) = (1, 0.3, 0, 1)
        _Smoothness("Edge Smoothness", Range(0.01, 0.5)) = 0.1
        _NoiseScale("Noise Scale", Range(1, 50)) = 20.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionOS   : TEXCOORD1; // 傳遞物件局部座標
            };

            CBUFFER_START(UnityPerMaterial)
                // float4 _BaseMap_ST;
                float4 _BurnColor;
                float4 _EmberColor;
                float _Smoothness;
                float _NoiseScale;
                float4 _BurnCenters[20]; 
                int _BurnCount;
            CBUFFER_END

            float hash(float3 p) {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz; 
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float totalBurnMask = 0;
                float accumulatedEmber = 0;

                float3 p = floor(input.positionOS * _NoiseScale) / _NoiseScale;
                float noise = hash(p) * 0.1;

                for (int j = 0; j < _BurnCount; j++)
                {
                    float3 center = _BurnCenters[j].xyz;
                    float radius = _BurnCenters[j].w + noise;

                    float3 warpedPos = input.positionOS + (noise * 5);
                    float dist = distance(warpedPos, center);

                    float burn = 1.0 - smoothstep(radius - _Smoothness, radius, dist);
                    totalBurnMask = max(totalBurnMask, burn);

                    float ember = smoothstep(radius, radius - 0.05, dist) * (1.0 - burn);
                    accumulatedEmber = max(accumulatedEmber, ember);
                }

                float finalEmberMask = accumulatedEmber * (1.0 - totalBurnMask);

                half4 charredColor = _BurnColor * totalBurnMask;
                half4 finalColor = charredColor + (_EmberColor * finalEmberMask * 2.0);

                return finalColor;
            }
            ENDHLSL
        }
    }
}
